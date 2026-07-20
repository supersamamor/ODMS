using FBSC.Common.Core.Base.Models;

namespace FBSC.ODMS.Core.ODMS;

public record UploadProcessorState : BaseEntity
{
    public string FileType { get; init; } = "";
    public string Path { get; init; } = "";
    public string Status { get; set; } = "";
    public DateTime? StartDateTime { get; private set; }
    public DateTime? EndDateTime { get; private set; }
    public string Module { get; init; } = "";
    public string UploadType { get; init; } = "";
    public string? TargetEntityId { get; init; }
    public string Remarks { get; private set; } = "";
    public string ExceptionFilePath { get; private set; } = "";
    public void SetStart()
    {
        this.StartDateTime = DateTime.UtcNow;
    }
    public void SetDone()
    {
        this.Status = Constants.FileUploadStatus.Done;
        this.EndDateTime = DateTime.UtcNow;
    }
    public void SetFailed(string exceptionFilePath,string remarks)
    {
        this.Status = Constants.FileUploadStatus.Failed;
        this.EndDateTime = DateTime.UtcNow;
        this.ExceptionFilePath = exceptionFilePath;
        this.Remarks = remarks;
    }
    public string Duration
    {
        get
        {
            if (!StartDateTime.HasValue || !EndDateTime.HasValue)
            {
                return "";
            }

            TimeSpan duration = EndDateTime.Value - StartDateTime.Value;
            return $"{duration.Minutes} Minute(s) and {duration.Seconds:D2} Second(s)";
        }
    }
    public string FormattedModule
    {
        get
        {
            if (string.IsNullOrWhiteSpace(this.Module))
                return this.Module;
            // Define the suffix to remove
            string suffixToRemove = "State";
            // Check if the input ends with "State" (case insensitive)
            if (this.Module.EndsWith(suffixToRemove, StringComparison.OrdinalIgnoreCase))
            {
                // Remove the length of "State" from the end of the string
                return this.Module[..^suffixToRemove.Length].Trim();
            }
            // Return the input if it does not end with "State"
            return this.Module;
        }
    }
}