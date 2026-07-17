using FBSC.Common.Core.Base.Models;

namespace FBSC.ApiHub.Models;

public record WebhookLogsState : BaseEntity
{
    public string WebhookEventAssignmentId { get; init; } = string.Empty;
    public string? DataId { get; init; }
    public string? Payload { get; init; }
    public WebhookEventAssignmentState? WebhookEventAssignment { get; init; }
    public DateTime? DateTimeStarted { get; private set; }
    public DateTime? DateTimeEnded { get; private set; }
    public string? Status { get; private set; } = WebhookStatus.Pending;
    public string? Error { get; private set; }
    public string? Response { get; private set; }
    public bool ProcessResponse { get; init; }
    public string? ProcessResponseStatus { get; set; }
    public string? ProcessResponseStatusError { get; private set; }
    public string ParametarizedRoute { get; set; } = string.Empty;
    public string? LockedByInstance { get; set; }
    public void SetError(string? error)
    {
        this.Error = error;
    }
    public void TagAsDone(string? response = null)
    {
        this.Error = null;
        this.Status = WebhookStatus.Done;
        this.DateTimeEnded = DateTime.UtcNow;
        this.SetResponse(response);
    }
    public void SetResponse(string? response)
    {
        this.Response = response;
    }
    public void TagAsPending()
    {
        if (this.Status == WebhookStatus.Failed)
        {
            this.Error = null;
            this.Status = WebhookStatus.Pending;
        }
    }
    public void TagAsFailed(string? error)
    {
        if (this.Status == WebhookStatus.Pending || this.Status == WebhookStatus.Executing)
        {
            this.Error = error;
            this.Status = WebhookStatus.Failed;
            this.DateTimeEnded = DateTime.UtcNow;
        }
    }
    public void TagAsStarted()
    {
        if (this.Status == WebhookStatus.Pending || this.Status == WebhookStatus.Executing)
        {
            this.DateTimeStarted = DateTime.UtcNow;
        }
    }
    public void TagAsCompletedWithError(string? error)
    {
        if (this.Status == WebhookStatus.Pending || this.Status == WebhookStatus.Executing)
        {
            this.Error = error;
            this.Status = WebhookStatus.CompletedWithError;
            this.DateTimeEnded = DateTime.UtcNow;
        }
    }
    public void TagProcessResponseStatusAsFailed(string? error)
    {
        if (this.ProcessResponseStatus == WebhookStatus.Pending || this.ProcessResponseStatus == WebhookStatus.Executing)
        {
            this.ProcessResponseStatusError = error;
            this.ProcessResponseStatus = WebhookStatus.Failed;
        }
    }
    public void TagProcessResponseStatusAsDone()
    {
        this.ProcessResponseStatusError = null;
        this.ProcessResponseStatus = WebhookStatus.Done;
    }
}

public static class WebhookStatus
{
    public const string Pending = "Pending";
    public const string Executing = "Executing"; // Status for locking
    public const string Done = "Done";
    public const string Failed = "Failed";
    public const string CompletedWithError = "Completed with Error";
}