using Moq;
using SC.SenseTower.Spaces.Data.Models;
using SC.SenseTower.Spaces.Features.Likes;
using SC.SenseTower.Spaces.Services;

namespace SC.SenseTower.Spaces.Tests.Features.Likes;

[TestFixture]
public class LikesValidatorTests
{
    [Test]
    public async Task SpaceNotFoundValueTest()
    {
        // prepare
        var spacesService = new Mock<ISpacesService>(MockBehavior.Strict);
        spacesService.Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<Space?>(null));

        var validator = new LikeCommandValidator(spacesService.Object);
        var command = new LikeCommand
        {
            Id = Guid.Parse("1847C152-BFDE-47B5-8998-BCB66E9BB2A6"),
            Like = true,
            UserId = Guid.Parse("C7B3B257-58CC-4E26-ADF5-8CD6D3780391")
        };

        // action
        bool result = (await validator.ValidateAsync(command)).IsValid;

        // check
        Assert.IsFalse(result);
    }

    [Test]
    public async Task CorrectValueTest()
    {
        // prepare
        var spacesService = new Mock<ISpacesService>(MockBehavior.Strict);
        var space = new Space();
        spacesService.Setup(x => x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<Space?>(space));

        var validator = new LikeCommandValidator(spacesService.Object);
        var command = new LikeCommand
        {
            Id = Guid.Parse("1847C152-BFDE-47B5-8998-BCB66E9BB2A6"),
            Like = true,
            UserId = Guid.Parse("C7B3B257-58CC-4E26-ADF5-8CD6D3780391")
        };

        // action
        bool result = (await validator.ValidateAsync(command)).IsValid;

        // check
        Assert.IsTrue(result);
    }
}