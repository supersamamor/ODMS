using FBSC.ODMS.Core.Constants;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using MediatR;


namespace FBSC.ODMS.Application.Features.ODMS.UploadProcessor.Commands;

public record UploadProcessorCommand : IRequest<int>
{
    public string FilePath { get; set; } = "";
    public string FileType { get; set; } = "";
    public string Module { get; set; } = "";
    public string UploadType { get; set; } = "";
    public string? TargetEntityId { get; set; }
}

public class UploadProcessorCommandHandler(ApplicationContext context) : IRequestHandler<UploadProcessorCommand, int>
{
    private readonly ApplicationContext _context = context;

    public async Task<int> Handle(UploadProcessorCommand request, CancellationToken cancellationToken)
    {
        var uploadProcessor = new UploadProcessorState()
        {
            FileType = request.FileType,
            Path = request.FilePath,
            Status = FileUploadStatus.Pending,
            Module = request.Module,
            UploadType = request.UploadType,
            TargetEntityId = request.TargetEntityId,

        };
        await _context.AddAsync(uploadProcessor, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return 1;
    }
}
