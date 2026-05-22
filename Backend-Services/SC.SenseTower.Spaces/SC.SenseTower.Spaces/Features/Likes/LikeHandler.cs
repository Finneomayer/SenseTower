using AutoMapper;
using MediatR;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Spaces.Data.Models;
using SC.SenseTower.Spaces.RabbitMQ;
using SC.SenseTower.Spaces.Services;

namespace SC.SenseTower.Spaces.Features.Likes;

public class LikeHandler : BaseHandler, IRequestHandler<LikeCommand>
{
    private readonly ISpacesService spacesService;
    private readonly IRabbitMQService rabbitMQService;
    public LikeHandler(ILogger<LikeHandler> logger, IMapper mapper, ISpacesService spacesService, IRabbitMQService rabbitMqService): base(logger, mapper)
    {
        this.spacesService = spacesService;
        rabbitMQService = rabbitMqService;
    }

    public async Task<Unit> Handle(LikeCommand request, CancellationToken cancellationToken)
    {
        var space = await spacesService.Get(request.Id, cancellationToken);
        if (space == null)
        {
            throw new Exception($"Space {request.Id} not found");
        }

        string userId = request.UserId.ToString();

        if (GetCurrentLikeValue(space, userId) == request.Like)
        {
            return Unit.Value;
        }

        if (request.Like == null)
        {
            if (space.Likes.ContainsKey(userId))
            {
                space.Likes.Remove(userId);
            } 
        }
        else
        {
            space.Likes[userId] = request.Like.Value;
        }

        await spacesService.Update(space, cancellationToken);
        await rabbitMQService.SendUpdateSpaceMessage(space, cancellationToken);

        return Unit.Value;
    }

    private bool? GetCurrentLikeValue(Space space, string userId)
    {
        if (!space.Likes.ContainsKey(userId))
        {
            return null;
        }
        
        return space.Likes[userId];
    }
}

