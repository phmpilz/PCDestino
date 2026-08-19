using Microsoft.EntityFrameworkCore;
using PCDestino.Application.Moderation;
using PCDestino.Domain.Places;
using PCDestino.Domain.Users;
using PCDestino.Infrastructure.Persistence;

namespace PCDestino.Infrastructure.Repositories;

internal sealed class ModerationRepository(AppDbContext dbContext, TimeProvider timeProvider) : IModerationRepository
{
    public async Task<IReadOnlyList<ModerationQueueItemDto>> GetQueueAsync(int take, CancellationToken cancellationToken)
    {
        take = Math.Clamp(take, 1, 100);
        var places = await dbContext.Places
            .AsNoTracking()
            .Where(place => place.Status == PublicationStatus.Pending)
            .OrderBy(place => place.CreatedAt)
            .Take(take)
            .Select(place => new ModerationQueueItemDto(place.Id, "place", place.Name, place.CreatedBy, place.CreatedAt))
            .ToListAsync(cancellationToken);
        var remaining = take - places.Count;
        if (remaining <= 0)
        {
            return places;
        }

        var reviews = await dbContext.Reviews
            .AsNoTracking()
            .Where(review => review.Status == PublicationStatus.Pending)
            .OrderBy(review => review.CreatedAt)
            .Take(remaining)
            .Select(review => new ModerationQueueItemDto(review.Id, "review", review.Place.Name, review.UserId, review.CreatedAt))
            .ToListAsync(cancellationToken);

        return places.Concat(reviews).OrderBy(item => item.SubmittedAt).Take(take).ToArray();
    }

    public async Task ModeratePlaceAsync(Guid id, ModeratePlaceCommand command, CancellationToken cancellationToken)
    {
        var place = await dbContext.Places.SingleOrDefaultAsync(item => item.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Local não encontrado.");
        if (place.Status != PublicationStatus.Pending)
        {
            throw new InvalidOperationException("O local já foi moderado.");
        }

        var now = timeProvider.GetUtcNow();
        if (command.Approve)
        {
            place.Publish(command.AccessibilityScore, command.Verified, now);
            dbContext.PointEvents.Add(PointEvent.Create(
                place.CreatedBy,
                place.CityId,
                100,
                PointEventType.ApprovedPlace,
                place.Id,
                now));
        }
        else
        {
            place.Reject(now);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task ModerateReviewAsync(Guid id, ModerateReviewCommand command, CancellationToken cancellationToken)
    {
        var review = await dbContext.Reviews
            .Include(item => item.Place)
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Avaliação não encontrada.");
        if (review.Status != PublicationStatus.Pending)
        {
            throw new InvalidOperationException("A avaliação já foi moderada.");
        }

        var now = timeProvider.GetUtcNow();
        if (command.Approve)
        {
            review.Publish(now);
            dbContext.PointEvents.Add(PointEvent.Create(
                review.UserId,
                review.Place.CityId,
                20,
                PointEventType.ApprovedReview,
                review.Id,
                now));
        }
        else
        {
            review.Reject(now);
        }

        var reviews = await dbContext.Reviews
            .Where(item => item.PlaceId == review.PlaceId)
            .ToListAsync(cancellationToken);
        review.Place.RecalculateRating(reviews);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
