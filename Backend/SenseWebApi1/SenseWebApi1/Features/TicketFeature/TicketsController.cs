using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SC.Internship.Common.ScResult;
using SenseWebApi1.Features.TicketFeature.AddFreeTickets;
using SenseWebApi1.Features.TicketFeature.CheckTicketForUser;
using SenseWebApi1.Features.TicketFeature.GetTicket;
using SenseWebApi1.Features.TicketFeature.GiveTicketForUser;
using SenseWebApi1.Features.TicketFeature.SellTicketForUser;

namespace SenseWebApi1.Features.TicketFeature;

[ApiController]
[Authorize]
[Route("tickets")]

public class TicketsController : ControllerBase
{
    // ReSharper disable once NotAccessedField.Local
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    public TicketsController(IMapper mapper, IMediator mediator)
    {
        _mapper = mapper;
        _mediator = mediator;
    }
    /// <summary>
    /// Добавить  билеты
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// Пример входных данных:
    ///
    ///     POST /tickets
    ///     {
    ///        "eventId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",Тип данных:Guid
    ///        "countoftickets": "10",Тип данных:int
    ///       
    ///     }
    ///
    /// </remarks>
    /// <response code="200">Возвращает bool добавленны билеты </response>
    /// <response code="401">Возвращает unauthorized </response>
    /// <response code="500">Ошибка сервера </response>
    [HttpPost("")]
    public async Task<ScResult<bool>> AddTickets(Guid eventId, int countOfTickets)
    {

        var result = await _mediator.Send(new AddTicketsCommand { EventId = eventId, Countoftickets = countOfTickets });
        return new ScResult<bool>
        {
            Result = result
        };
    }

    /// <summary>
    /// Дать пользователю билет
    /// </summary>
    /// <returns />
    /// <remarks>
    /// Пример входных данных:
    /// 
    ///     POST /tickets/{ticketId}
    ///     {
    ///         "ticketId": "8299f9f5-6176-4595-9004-bd01beafeb25",
    ///         "ownerId": "f4d26a57-0725-4796-82bf-2be457bbfcd4",
    ///         "place": "4"
    ///     }
    /// 
    /// </remarks>
    /// <response code="200">Возвращает bool добавленны билеты </response>
    /// <response code="401">Возвращает unauthorized </response>
    /// <response code="500">Ошибка сервера </response>
    [HttpPut("/{ticketId:guid}")]

    public async Task<ScResult<Guid>> GiveTicketForUser( Guid ticketId, Guid ownerId)
    {
            
        var result = await _mediator.Send(new GiveTicketForUserCommand { TicketId = ticketId, OwnerId = ownerId});
        return new ScResult<Guid>
        {
            Result = result
        };
    }
    /// <summary>
    /// Продать пользователю билет
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// Пример входных данных:
    ///
    ///     POST /tickets/{ticketId}
    ///     {
    ///         "ticketId": "8299f9f5-6176-4595-9004-bd01beafeb25",
    ///         "ownerId": "f4d26a57-0725-4796-82bf-2be457bbfcd4",
    ///         "place": "4"
    ///     }
    ///
    /// </remarks>
    /// <response code="200">Возвращает bool добавленны билеты </response>
    /// <response code="401">Возвращает unauthorized </response>
    /// <response code="500">Ошибка сервера </response>
    [HttpPut("/sell/{ticketId:guid}")]

    public async Task<ScResult<Ticket>> SellTicketForUser( Guid ticketId, Guid ownerId)
    {
            
        var result = await _mediator.Send(new SellTicketForUserCommand { TicketId = ticketId, OwnerId = ownerId});
        return new ScResult<Ticket>
        {
            Result = result
        };
    }
    /// <summary>
    /// Проверить билет у пользователя
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// Пример входных данных:
    ///
    ///     POST /tickets
    ///     {
    ///        "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",Тип данных:Guid
    ///        "ticketId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",Тип данных:Guid
    ///       
    ///     }
    ///
    /// </remarks>
    /// <response code="200">Возвращает bool  </response>
    /// <response code="401">Возвращает unauthorized </response>
    /// <response code="500">Ошибка сервера </response>
    [HttpGet("")]

    public async Task<ScResult<bool>> CheckTicketForUser(Guid eventId, Guid userId)
    {
        var result = await _mediator.Send(new CheckTicketForUserCommand { EventId = eventId, OwnerId = userId });
        return new ScResult<bool>
        {
            Result = result
        };
    }

    [HttpGet("/gettickets")]

    public async Task<ScResult<List<Ticket>>> GetTickets(Guid eventId)
    {
        var result = await _mediator.Send(new GetTicketsCommand { EventId = eventId });
        return new ScResult<List<Ticket>>
        {
            Result = result
        };
    }
}