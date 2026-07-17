using AutoMapper;
using FBSC.ODMS.Infrastructure.Data;
using LanguageExt;
using LanguageExt.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static LanguageExt.Prelude;
using FBSC.ODMS.Core.Identity;

namespace FBSC.ODMS.Web.Areas.Admin.Commands.Entities;

public record AddOrEditEntityCommand : IRequest<Validation<Error, Entity>>
{
    public string? Id { get; set; }
    public string? Name { get; set; }
}

public class AddOrEditEntityCommandHandler(IdentityContext context, IMapper mapper) : IRequestHandler<AddOrEditEntityCommand, Validation<Error, Entity>>
{
    public async Task<Validation<Error, Entity>> Handle(AddOrEditEntityCommand request, CancellationToken cancellationToken) =>
        await Optional(await context.Entities.FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken))
        .MatchAsync(
            Some: async entity => await ValidateName(request, cancellationToken)
            .MapAsync(async valid => await valid.MatchAsync(
                SuccAsync: async request =>
                {
                    mapper.Map(request, entity);
                    context.Update(entity!);
                    await context.SaveChangesAsync(cancellationToken);
                    return Success<Error, Entity>(entity!);
                },
                Fail: errors => Validation<Error, Entity>.Fail(errors))),
            None: async () =>
            {
                var entity = mapper.Map<Entity>(request);
                context.Add(entity);
                await context.SaveChangesAsync(cancellationToken);
                return Success<Error, Entity>(entity);
            });

    async Task<Validation<Error, AddOrEditEntityCommand>> ValidateName(AddOrEditEntityCommand request, CancellationToken cancellationToken) =>
        Optional(await context.Entities.FirstOrDefaultAsync(m => m.Name == request.Name && m.Id != request.Id, cancellationToken))
            .Match(
            e => Fail<Error, AddOrEditEntityCommand>($"Entity with name {request.Name} already exists."),
            () => request);
}
