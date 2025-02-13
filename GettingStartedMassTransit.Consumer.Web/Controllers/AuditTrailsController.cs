using GettingStartedMassTransit.Common.EventBus.Entity.Application;
using GettingStartedMassTransit.Common.EventBus.Entity.AuditTrail;
using GettingStartedMassTransit.Consumer.Web.Models;
using GettingStartedMassTransit.Consumer.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace GettingStartedMassTransit.Consumer.Web.Controllers;

[ApiController]
[Route("api/audittrails")]
public class AuditTrailsController : ControllerBase
{
    private readonly AuditTrailsService<ApplicationBetaEntity> _auditTrailsService;
    public AuditTrailsController(AuditTrailsService<ApplicationBetaEntity> auditTrailsService)
    {
        _auditTrailsService = auditTrailsService;
    }

    [HttpGet]
    public async Task<List<AuditTrailEntity<ApplicationBetaEntity>>> Get()
    {
        return await _auditTrailsService.GetAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AuditTrailEntity<ApplicationBetaEntity>>> Get(string id)
    {
        AuditTrailEntity<ApplicationBetaEntity>? entity = await _auditTrailsService.GetAsync(id);

        if (entity is null)
        {
            return NotFound();
        }

        return entity;
    }

    [HttpPost]
    public async Task<IActionResult> Post(AuditTrailEntity<ApplicationBetaEntity> newEntity)
    {
        await _auditTrailsService.CreateAsync(newEntity);

        return CreatedAtAction(nameof(Get), new { id = newEntity.Id }, newEntity);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, AuditTrailEntity<ApplicationBetaEntity> updatedEntity)
    {
        AuditTrailEntity<ApplicationBetaEntity>? entity = await _auditTrailsService.GetAsync(id);

        if (entity is null)
        {
            return NotFound();
        }

        updatedEntity.Id = entity.Id;

        await _auditTrailsService.UpdateAsync(id, updatedEntity);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        AuditTrailEntity<ApplicationBetaEntity>? entity = await _auditTrailsService.GetAsync(id);

        if (entity is null)
        {
            return NotFound();
        }

        await _auditTrailsService.RemoveAsync(id);

        return NoContent();
    }
}
