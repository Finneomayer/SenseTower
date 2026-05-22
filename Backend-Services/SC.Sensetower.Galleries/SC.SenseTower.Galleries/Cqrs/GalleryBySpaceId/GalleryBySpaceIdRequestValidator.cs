using FluentValidation;

namespace SC.SenseTower.Galleries.Cqrs.GalleryBySpaceId
{
    public class GalleryBySpaceIdRequestValidator : AbstractValidator<GalleryBySpaceIdRequest>
    {
        public GalleryBySpaceIdRequestValidator()
        {
            RuleFor(x => x.SpaceId).NotEmpty().WithMessage("Не указан идентификатор пространства");
        }
    }
}
