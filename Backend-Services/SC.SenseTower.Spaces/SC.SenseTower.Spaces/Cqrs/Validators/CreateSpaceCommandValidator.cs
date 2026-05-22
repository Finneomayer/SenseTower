using FluentValidation;
using SC.SenseTower.Spaces.Cqrs.Commands;
using SC.SenseTower.Spaces.Services;

namespace SC.SenseTower.Spaces.Cqrs.Validators
{
    public class CreateSpaceCommandValidator : AbstractValidator<CreateSpaceCommand>
    {
        public CreateSpaceCommandValidator(
            ISpacesService spacesService,
            AccountsService accountsService,
            ImagesService imagesService)
        {
            RuleFor(x => x.Id)
                .MustAsync(async (c, id, cancellationToken) =>
                {
                    if (id == default)
                        return true;
                    var space = await spacesService.Get(id, cancellationToken);
                    return space == null;
                }).WithMessage("Пространство с таким идентификатором уже существует");
            RuleFor(x => x.SpaceName).NotEmpty().WithMessage("Название пространства не задано");
            RuleFor(x => x.SceneName).NotEmpty().WithMessage("Название сцены не задано");
            RuleFor(x => x.Ip)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Не задан IP-адрес для соединения с сервером")
                .Must((c, ip) =>
                {
                    var segments = ip.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    if (segments.Length != 4)
                        return false;
                    foreach (var segment in segments)
                    {
                        if (!byte.TryParse(segment, out var number))
                            return false;
                    }
                    return true;
                }).WithMessage("Неверный формат IP-адреса");
            RuleFor(x => x.Port).NotEmpty().WithMessage("Не задан порт для соединения с сервером");
            RuleFor(x => x.RemoteSceneName)
                .Must((c, remoteSceneName) =>
                {
                    return (string.IsNullOrWhiteSpace(remoteSceneName) && string.IsNullOrWhiteSpace(c.RemoteCatalogName) && string.IsNullOrWhiteSpace(c.RemoteFolderName))
                        || (!string.IsNullOrWhiteSpace(remoteSceneName) && !string.IsNullOrWhiteSpace(c.RemoteCatalogName) && !string.IsNullOrWhiteSpace(c.RemoteFolderName));
                }).WithMessage("Параметры удаленной сцены должны быть заполнены");
            RuleFor(x => x.SpaceOwnerId)
                .MustAsync(async (c, spaceOwnerId, cancellationToken) =>
                {
                    if (spaceOwnerId == null)
                        return true;
                    var owner = await accountsService.GetInfo(c.AccessToken, spaceOwnerId.Value, cancellationToken);
                    return owner != null;
                }).WithMessage("Владелец пространства не найден");
            RuleFor(x => x.DoorImageId)
                .MustAsync(async (c, doorImageId, cancellationToken) =>
                {
                    if (doorImageId == null)
                        return true;
                    var image = await imagesService.GetInfo(c.AccessToken, doorImageId.Value, cancellationToken);
                    return image != null;
                }).WithMessage("Изображение не найдено");
        }
    }
}
