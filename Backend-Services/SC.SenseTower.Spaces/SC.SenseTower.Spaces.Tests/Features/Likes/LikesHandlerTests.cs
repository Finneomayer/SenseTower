using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using SC.SenseTower.Spaces.Data.Models;
using SC.SenseTower.Spaces.Features.Likes;
using SC.SenseTower.Spaces.RabbitMQ;
using SC.SenseTower.Spaces.Services;

namespace SC.SenseTower.Spaces.Tests.Features.Likes;

[TestFixture]
public sealed class LikesHandlerTests
{
    [Test]
    public async Task AddPositiveVoteTest()
    {
        // prepare
        var spaceId = Guid.Parse("C9B6A7D7-2141-47C0-8AAC-2427818400AB");
        var userId = Guid.Parse("D5B50572-B317-49F6-B94B-F47C76FC4354");

        var logger = new Mock<ILogger<LikeHandler>>(MockBehavior.Loose);
        var mapper = new Mock<IMapper>(MockBehavior.Loose);
        var space = new Space
        {
            Id = spaceId
        };
        var cancellationToken = new CancellationToken();

        var spaceService = new Mock<ISpacesService>(MockBehavior.Strict);
        spaceService.Setup(s => s.Get(spaceId, cancellationToken))
            .Returns(Task.FromResult<Space?>(space));
        spaceService.Setup(s => s.Update(space, cancellationToken))
            .Returns(Task.CompletedTask);

        var rmq = new Mock<IRabbitMQService>(MockBehavior.Strict);
        rmq.Setup(x => x.SendUpdateSpaceMessage(space, cancellationToken))
            .Returns(Task.CompletedTask);

        var handler = new LikeHandler(logger.Object, mapper.Object, spaceService.Object, rmq.Object);
        
        var command = new LikeCommand
        {
            Id = spaceId,
            Like = true,
            UserId = userId
        };

        // action
        await handler.Handle(command, cancellationToken);

        // check
        Assert.IsNotNull(space.Likes);
        Assert.That( space.Likes.Count, Is.EqualTo(1));
        Assert.IsTrue(space.Likes.ContainsKey(userId.ToString()));
        Assert.That(space.Likes[userId.ToString()], Is.True);

    }
}