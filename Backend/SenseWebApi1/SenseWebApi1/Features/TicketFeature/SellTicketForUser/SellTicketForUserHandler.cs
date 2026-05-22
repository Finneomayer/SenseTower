using System.Net;
using MediatR;
using MongoDB.Driver;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using SC.Internship.Common.Exceptions;
using SC.Internship.Common.ScResult;
using SenseWebApi1.Features.EventFeature;
using SenseWebApi1.MongoDB;
using SenseWebApi1.Services;

namespace SenseWebApi1.Features.TicketFeature.SellTicketForUser;

// ReSharper disable once UnusedType.Global
public class SellTicketForUserHandler:IRequestHandler<SellTicketForUserCommand,Ticket>
{
    
    private readonly IHttpService _httpService;
    private readonly RetryPolicy _retryPolicy=Policy.Handle<Exception>().Retry(2);
    private readonly IMongoDbContext _databaseContext;

    public SellTicketForUserHandler(IHttpService httpService,IMongoDbContext databaseContext)
    {
        
        _httpService = httpService;
        _databaseContext = databaseContext;

    }
    public async Task<Ticket> Handle(SellTicketForUserCommand request, CancellationToken cancellationToken)
    {
        Ticket? result;
        Guid id;
        
        
        var response=await _retryPolicy.Execute(() =>_httpService.CreateTransaction(request.OwnerId, cancellationToken));
        
        
        if (response.StatusCode == HttpStatusCode.OK)
        {
            result= await _retryPolicy.Execute(()=>SellTicketForUser(request.OwnerId, request.TicketId));
            var httpResult =await response.Content.ReadAsStringAsync(cancellationToken);
#pragma warning disable CS8602
            id = JsonConvert.DeserializeObject<ScResult<Guid>>(httpResult).Result;
#pragma warning restore CS8602
        }
        else
        {
            throw new ScException("Проблема с сервисом оплаты");
        }
        
#pragma warning disable CS8602
         
#pragma warning restore CS8602
        
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (result != null)
        {
            await _retryPolicy.Execute(()=>_httpService.ConfirmTransaction(id, cancellationToken));
        }
        else
        {
            await _retryPolicy.Execute(()=>_httpService.CancelTransaction(id, cancellationToken));
        }

        return result!;
        
    }
    private async Task<Ticket> SellTicketForUser(Guid userId, Guid ticketId)
    {
        var filterForSearch = Builders<Event>.Filter.ElemMatch(p => p.Tickets, t => t.TicketId == ticketId);
        var mongoCollection = _databaseContext.GetMongoDatabase().GetCollection<Event>("Events");
        var eventObj = await mongoCollection.Find(filterForSearch).FirstOrDefaultAsync();
#pragma warning disable CS8604
        var ticket = eventObj.Tickets.FirstOrDefault(p => p.TicketId==ticketId);
        if (ticket != null)
        {
                
            ticket.OwnerId = userId;
                
#pragma warning restore CS8604
            await UpdateEvent(eventObj);
        }
        else
        {
            throw new ScException("Такой билет не найден");
        }

        return ticket;
    }
    private  async Task UpdateEvent(Event @event)
    {
        var mongoCollection = _databaseContext.GetMongoDatabase().GetCollection<Event>("Events");
        var filter = Builders<Event>.Filter
            .Where(p=>p.EventId==@event.EventId);
            
        var updateWithTick = Builders<Event>.Update
                    
            .Set(p => p.Beginning, @event.Beginning)
            .Set(p => p.End, @event.End)
            .Set(p => p.AreaId, @event.AreaId)
            .Set(p => p.Description, @event.Description)
            .Set(p => p.ImageId, @event.ImageId)
            .Set(p => p.Tickets, @event.Tickets)
            .Set(p => p.IsHavePlaces, @event.IsHavePlaces)
            .Set(p=>p.Cost,@event.Cost);
        await mongoCollection.UpdateOneAsync(filter, updateWithTick);
            
            
           
    }
}