using GettingStartedMassTransit.Consumer.Web.Models;
using GettingStartedMassTransit.Consumer.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace GettingStartedMassTransit.Consumer.Web.Controllers;

[ApiController]
[Route("api/messageevents")]
public class MessageEventsController : ControllerBase
{
    private readonly MessageEventsService _messageEventsService;    
    public MessageEventsController(MessageEventsService messageEventsService)
    {
        _messageEventsService = messageEventsService;
    }

    [HttpGet]
    public async Task<List<AuditTrailEntity>> Get() =>
        await _messageEventsService.GetAsync();

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<AuditTrailEntity>> Get(string id)
    {
        var entity = await _messageEventsService.GetAsync(id);

        if (entity is null)
        {
            return NotFound();
        }

        return entity;
    }

    [HttpPost]
    public async Task<IActionResult> Post(AuditTrailEntity newEntity)
    {
        await _messageEventsService.CreateAsync(newEntity);

        return CreatedAtAction(nameof(Get), new { id = newEntity.Id }, newEntity);
    }

    [HttpPut("{id:length(24)}")]
    public async Task<IActionResult> Update(string id, AuditTrailEntity updatedEntity)
    {
        var book = await _messageEventsService.GetAsync(id);

        if (book is null)
        {
            return NotFound();
        }

        updatedEntity.Id = book.Id;

        await _messageEventsService.UpdateAsync(id, updatedEntity);

        return NoContent();
    }

    [HttpDelete("{id:length(24)}")]
    public async Task<IActionResult> Delete(string id)
    {
        var book = await _messageEventsService.GetAsync(id);

        if (book is null)
        {
            return NotFound();
        }

        await _messageEventsService.RemoveAsync(id);

        return NoContent();
    }
}
