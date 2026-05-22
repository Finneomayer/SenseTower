using AutoMapper;
using Microsoft.Extensions.Options;
using SC.SenseTower.Accounts.Settings;
using SC.SenseTower.Common.Services;

namespace SC.SenseTower.Accounts.Services
{
    public class PlacesService : BaseHttpService
    {
        private readonly ServiceEndpointsSettings endpoints;

        public PlacesService(
            ILogger<PlacesService> logger,
            IMapper mapper,
            HttpClient httpClient,
            IOptions<ServiceEndpointsSettings> options) : base(logger, mapper, httpClient)
        {
            endpoints = options.Value;
        }

        /// <summary>
        /// Отправка запроса сервису MyPlaces на удаление всех помещений пользователя.
        /// </summary>
        /// <param name="accessToken">Административный токен доступа.</param>
        /// <param name="ownerId">Идентификатор владельца помещений.</param>
        /// <param name="cancellationToken">Токен прерывания выполнения.</param>
        /// <returns>Признак успешного завершения операции.</returns>
        public async Task<bool> DeleteUserPlaces(string? accessToken, Guid? ownerId, CancellationToken cancellationToken)
        {
            var result = await Post<bool>(accessToken, endpoints.DeletePlacesUrl, new { ownerId }, cancellationToken).ConfigureAwait(false);
            return result;
        }
    }
}
