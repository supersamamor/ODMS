using AutoMapper;
using FBSC.Common.Core.Commands;
using FBSC.Common.Data;
using FBSC.Common.Utility.Validators;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using FluentValidation;
using LanguageExt;
using LanguageExt.Common;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using static LanguageExt.Prelude;

namespace FBSC.ODMS.Application.Features.ODMS.DataUploadBatch.Commands;

public record DeleteDataUploadBatchCommand : BaseCommand, IRequest<Validation<Error, DataUploadBatchState>>;

public class DeleteDataUploadBatchCommandHandler(ApplicationContext context,
                                   IMapper mapper,
                                   CompositeValidator<DeleteDataUploadBatchCommand> validator,
                                   IConfiguration configuration) : BaseCommandHandler<ApplicationContext, DataUploadBatchState, DeleteDataUploadBatchCommand>(context, mapper, validator), IRequestHandler<DeleteDataUploadBatchCommand, Validation<Error, DataUploadBatchState>>
{
    public async Task<Validation<Error, DataUploadBatchState>> Handle(DeleteDataUploadBatchCommand request, CancellationToken cancellationToken) =>
        await Validators.ValidateTAsync(request, cancellationToken).BindT(
            async request => await DeleteBatchAndStagingTable(request.Id, cancellationToken));

    /// <summary>
    /// A DataUploadBatchState always has DataUploadColumnState children and a matching
    /// DataSourceSchemaCacheState row set once ingested - ApplicationContext disables cascade
    /// delete on every FK, so the base Delete() (row-only) would throw a FK violation here.
    /// This also drops the physical staging table itself, which the base Delete() would leave
    /// orphaned in the database forever.
    /// </summary>
    private async Task<Validation<Error, DataUploadBatchState>> DeleteBatchAndStagingTable(string id, CancellationToken cancellationToken)
    {
        var batch = await Context.DataUploadBatch
            .Include(b => b.DataUploadColumnList)
            .SingleAsync(b => b.Id == id, cancellationToken);

        var schemaCacheRows = await Context.DataSourceSchemaCache
            .Where(s => s.DataSourceId == batch.DataSourceId && ("uploads." + s.TableName) == batch.StagingTableName)
            .ToListAsync(cancellationToken);
        Context.DataSourceSchemaCache.RemoveRange(schemaCacheRows);

        if (batch.DataUploadColumnList is { Count: > 0 })
        {
            Context.DataUploadColumn.RemoveRange(batch.DataUploadColumnList);
        }

        Context.DataUploadBatch.Remove(batch);
        _ = await Context.SaveChangesAsync(cancellationToken);

        if (!string.IsNullOrEmpty(batch.StagingTableName))
        {
            await DropStagingTableAsync(batch.StagingTableName, cancellationToken);
        }

        return Success<Error, DataUploadBatchState>(batch);
    }

    private async Task DropStagingTableAsync(string stagingTableName, CancellationToken cancellationToken)
    {
        var separatorIndex = stagingTableName.IndexOf('.');
        if (separatorIndex <= 0)
        {
            return;
        }
        var schema = stagingTableName[..separatorIndex];
        var table = stagingTableName[(separatorIndex + 1)..];

        await using var connection = new SqlConnection(configuration.GetConnectionString("ApplicationContext"));
        await connection.OpenAsync(cancellationToken);
        await using var command = new SqlCommand(
            $"IF OBJECT_ID('{schema}.[{table}]', 'U') IS NOT NULL DROP TABLE {schema}.[{table}];",
            connection);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}


public class DeleteDataUploadBatchCommandValidator : AbstractValidator<DeleteDataUploadBatchCommand>
{
    readonly ApplicationContext _context;

    public DeleteDataUploadBatchCommandValidator(ApplicationContext context)
    {
        _context = context;
        RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.Exists<DataUploadBatchState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("DataUploadBatch with id {PropertyValue} does not exists");
    }
}
