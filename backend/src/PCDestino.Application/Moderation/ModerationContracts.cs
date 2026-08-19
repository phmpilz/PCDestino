namespace PCDestino.Application.Moderation;

public sealed record ModerationQueueItemDto(
    Guid Id,
    string Type,
    string Title,
    string SubmittedBy,
    DateTimeOffset SubmittedAt);

public sealed record ModeratePlaceCommand(bool Approve, int AccessibilityScore, bool Verified);

public sealed record ModerateReviewCommand(bool Approve);

public interface IModerationRepository
{
    Task<IReadOnlyList<ModerationQueueItemDto>> GetQueueAsync(int take, CancellationToken cancellationToken);
    Task ModeratePlaceAsync(Guid id, ModeratePlaceCommand command, CancellationToken cancellationToken);
    Task ModerateReviewAsync(Guid id, ModerateReviewCommand command, CancellationToken cancellationToken);
}
