using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Options;
using SC.SenseTower.Admin.Cqrs;
using SC.SenseTower.Admin.Cqrs.TowerEvent;
using SC.SenseTower.Admin.Cqrs.TowerEventsPage;
using SC.SenseTower.Admin.Dto.TowerEvents;
using SC.SenseTower.Admin.Settings;
using SC.SenseTower.Common.Data;
using SC.SenseTower.Common.Models;
using SC.SenseTower.Common.Services;

namespace SC.SenseTower.Admin.Services
{
    public class TowerEventsService : BaseHttpService
    {
        private readonly ServiceEndpointsSettings endpointsSettings;

        public TowerEventsService(
            ILogger<TowerEventsService> logger,
            IMapper mapper,
            HttpClient httpClient,
            IOptions<ServiceEndpointsSettings> options) : base(logger, mapper, httpClient)
        {
            endpointsSettings = options.Value;
        }

        public async Task<PagedDataDto<TowerEventItemResponseDto>> GetPagedList(string accessToken, TowerEventsPageFilter filter, QuerySorting[] sorting, int page, int pageSize, CancellationToken cancellationToken)
        {
            var request = new PagedDataRequest<TowerEventsPageFilter>
            {
                Filters = filter,
                Sorting = sorting,
                Page = page,
                PageSize = pageSize
            };
            var result = await PostAsJson<PagedDataDto<TowerEventItemResponseDto>>(accessToken, endpointsSettings.TowerEventListUrl, request, cancellationToken);
            return result ?? new PagedDataDto<TowerEventItemResponseDto>();
        }

        public async Task<bool> Exists(string? accessToken, Guid id, CancellationToken cancellationToken)
        {
            var result = await Get<bool>(accessToken, string.Format(endpointsSettings.TowerEventExistsUrl, id), null, cancellationToken);
            return result;
        }

        public async Task<TowerEventResponseDto?> Get(string? accessToken, Guid eventId, CancellationToken cancellationToken)
        {
            var result = await Get<TowerEventResponseDto>(accessToken, string.Format(endpointsSettings.TowerEventGetUrl, eventId), null, cancellationToken);
            return result;
        }

        public async Task Update(string accessToken, string refreshToken, TowerEventUpdateRequestDto towerEvent, CancellationToken cancellationToken)
        {
            await PutAsJson<bool>(accessToken, endpointsSettings.TowerEventUpdateUrl, towerEvent, cancellationToken);
        }

        public async Task<Guid> Create(string accessToken, string refreshToken, TowerEventCreateRequestDto towerEvent, CancellationToken cancellationToken)
        {
            var result = await PostAsJson<Guid>(accessToken, endpointsSettings.TowerEventCreateUrl, towerEvent, cancellationToken);
            return result;
        }

        public async Task Delete(string accessToken, string refreshToken, Guid eventId, CancellationToken cancellationToken)
        {
            _ = await Delete<object>(accessToken, refreshToken, string.Format(endpointsSettings.TowerEventDeleteUrl, eventId), cancellationToken);
        }
    }
}
