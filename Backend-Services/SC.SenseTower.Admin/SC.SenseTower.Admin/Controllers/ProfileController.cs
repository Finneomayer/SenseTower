using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SC.SenseTower.Admin.Cqrs.SetPassword;
using SC.SenseTower.Admin.Models.Profile;

namespace SC.SenseTower.Admin.Controllers
{
    [Authorize]
    public class ProfileController : BaseMvcController
    {
        protected override string ActiveMenuItem => "";

        public ProfileController(
            IMediator mediator,
            ILogger<ProfileController> logger,
            IConfiguration configuration) : base(mediator, logger, configuration)
        {
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new ProfileViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> SetPassword(PasswordViewModel model, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                var command = new SetPasswordCommand
                {
                    CurrentPassword = model.CurrentPassword,
                    Password = model.Password,
                    UserId = GetUserId()
                };
                try
                {
                    await Mediator.Send(command, cancellationToken);
                    ViewBag.Message = "Пароль успешно изменён!";
                    return PartialView("_SetPassword", new PasswordViewModel());
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            return PartialView("_SetPassword", model);
        }
    }
}
