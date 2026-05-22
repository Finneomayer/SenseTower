using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SC.Internship.Common.ScResult;
using Microsoft.AspNetCore.Authorization;
using SenseWebApi1.Features.EventFeature.CreateEvent;
using SenseWebApi1.Features.EventFeature.DeleteEvent;
using SenseWebApi1.Features.EventFeature.GetEvents;
using SenseWebApi1.Features.EventFeature.UpdateEvent;
using SenseWebApi1.Features.EventFeature.CheckPlaceForEvent;

namespace SenseWebApi1.Features.EventFeature;

[ApiController]
[Route("api/events")]
[Authorize]
public class MainController : ControllerBase
{
    private readonly IMediator _mediator;
    // ReSharper disable once NotAccessedField.Local
    private readonly IMapper _mapper;


    public MainController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }
    /// <summary>
    /// Создание события
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// Пример входных данных:
    ///
    ///     POST /events
    ///     {
    ///        
    ///        "beginning": "2023-03-14T16:52:06.478Z",Тип данных:DateTime
    ///        "end": "2023-03-14T16:52:06.478Z",Тип данных:DateTime
    ///        "eventName": "string",Тип данных:String
    ///        "description": "string",Тип данных:String
    ///        "imageId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",Тип данных:Guid
    ///        "areaId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",Тип данных:Guid
    ///     }
    ///
    /// </remarks>
    /// <response code="200">Возвращает id добавленного события </response>
    /// <response code="401">Возвращает unauthorized </response>
    /// <response code="500">Ошибка сервера </response>
    [HttpPost("")]

    public async Task<ScResult<Guid>> CreateEvent(EventCreateCommand eventCreateCommand,CancellationToken cancellationToken)
    {
           
        var result = await _mediator.Send(eventCreateCommand,cancellationToken);
        return new ScResult<Guid>
        {
            Result = result
        };
    }
    /// <summary>
    /// Изменение события
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// Пример входных данных:
    ///
    ///     PUT /events/{eventId}
    ///     {
    ///        "eventId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",Тип данных:Guid
    ///        "beginning": "2023-03-14T16:52:06.478Z",Тип данных:DateTime
    ///        "end": "2023-03-14T16:52:06.478Z",Тип данных:DateTime
    ///        "eventName": "string",Тип данных:String
    ///        "description": "string",Тип данных:String
    ///        "imageId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",Тип данных:Guid
    ///        "areaId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",Тип данных:Guid
    ///     }
    ///
    /// </remarks>
    /// <response code="200">Возвращает данные об измененном событии </response>
    /// <response code="401">Возвращает unauthorized </response>
    /// <response code="500">Ошибка сервера </response>
    [HttpPut("")]

    public async Task<ScResult<EventDto>> ChangeEvent(EventUpdateCommand eventUpdateCommand,CancellationToken cancellationToken)
    {
            
        var result = await _mediator.Send(eventUpdateCommand,cancellationToken);
        return new ScResult<EventDto>
        {
            Result = result
        };
    }
    /// <summary>
    /// Удаление события
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// Пример входных данных:
    ///
    ///     PUT /events/{eventId}
    ///     eventId Тип данных:Guid
    ///
    /// </remarks>
    /// <response code="200">Возвращает id удаленного события</response>
    /// <response code="401">Возвращает unauthorized </response>
    /// <response code="500">Ошибка сервера </response>
    [HttpDelete("{eventId:guid}")]

    public async Task<ScResult> DeleteEvent(Guid eventId,CancellationToken cancellationToken)
    {
        await _mediator.Send(new EventDeleteCommand { EventId = eventId },cancellationToken);
        return new ScResult();
            
    }

    /// <summary>
    /// Получение событий из базы данных
    /// </summary>
    /// <returns></returns>
    /// <response code="200">Возвращает json строку с данными о событиях </response>
    /// <response code="401">Возвращает unauthorized </response>
    /// <response code="500">Ошибка сервера </response>
    [HttpGet("")]
    public async Task<ScResult<IEnumerable<EventDto>>> GetEvents(CancellationToken cancellationToken)
    {


        var result = await _mediator.Send(new GetEventCommand(),cancellationToken);

        return new ScResult<IEnumerable<EventDto>>
        {
            Result = result
        };
    }
    /// <summary>
    /// Проверка на наличие места в мероприятии
    /// </summary>
    /// <returns></returns>
    /// <response code="200">Возвращает bool  </response>
    /// <response code="401">Возвращает unauthorized </response>
    /// <response code="500">Ошибка сервера </response>
        
    [HttpGet("{eventId:guid}")]
    public async Task<ScResult<bool>> CheckPlaceForEvent(Guid eventId,int place,CancellationToken cancellationToken)
    {


        var result = await _mediator.Send(new CheckPlaceForEventCommand
        {
            eventId = eventId,
            place = place
        },cancellationToken);

        return new ScResult<bool>
        {
            Result = result
        };
    }




}