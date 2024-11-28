using GettingStartedMassTransit.Common.EventBus.Abstractions;
using GettingStartedMassTransit.Common.EventBus.Entity;
using GettingStartedMassTransit.Common.EventBus.Events;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.Json.Nodes;

namespace GettingStartedMassTransit.Web.Controllers;

[Route("api/messageevent")]
[ApiController]
public class MessageEventController : ControllerBase
{
    private readonly IEventBus _eventBus;
    public MessageEventController(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    [HttpGet("jsonobject")]
    public async Task<Results<Ok, BadRequest>> PublishMessageEventWithJsonObjectAsync()
    {
        var applicationEntity = new ApplicationEntity
        {
            PropertyA = 1,
            PropertyB = "text",
            PropertyC = true
        };

        var jsonObject = new JsonObject
        {
            ["PropertyA"] = applicationEntity.PropertyA,
            ["PropertyB"] = applicationEntity.PropertyB,
            ["PropertyC"] = applicationEntity.PropertyC
        };

        var messageEvent = new MessageEventWithJsonObject
        {
            EventTime = DateTime.UtcNow,
            EventSource = "Publisher",
            EventType = "MessageEvent",
            Domain = "Message",
            UserId = Guid.NewGuid(),
            UserFirstName = "John",
            UserLastName = "Doe",
            Data = jsonObject
        };

        await _eventBus.PublishAsync(messageEvent);
        return TypedResults.Ok();
    }

    [HttpGet("json")]
    public async Task<Results<Ok, BadRequest>> PublishMessageEventWithJsonAsync()
    {
        var applicationEntity = new ApplicationEntity
        {
            PropertyA = 1,
            PropertyB = "text",
            PropertyC = true
        };

        var messageEvent = new MessageEventWithJson
        {
            EventTime = DateTime.UtcNow,
            EventSource = "Publisher",
            EventType = "MessageEvent",
            Domain = "Message",
            UserId = Guid.NewGuid(),
            UserFirstName = "John",
            UserLastName = "Doe",
            Data = JsonConvert.SerializeObject(applicationEntity)
        };

        await _eventBus.PublishAsync<MessageEventWithJson>(messageEvent);
        return TypedResults.Ok();
    }

    [HttpGet()]
    public async Task<Results<Ok, BadRequest>> PublishMessageEventOtherTypeAsync(string text)
    {
        var messageEvent = new MessageEventOtherType
        {
            Text = text
        };

        await _eventBus.PublishAsync<MessageEventOtherType>(messageEvent);
        return TypedResults.Ok();
    }
}
