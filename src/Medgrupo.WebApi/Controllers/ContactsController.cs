using Medgrupo.Business.Dtos;
using Medgrupo.Business.Notifications.Abstractions;
using Medgrupo.Business.Services.Abstractions;
using Medgrupo.Data.Pagination;
using Medgrupo.WebApi.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Medgrupo.WebApi.Controllers;

/// <summary>
/// Gerencia os contatos (criação, listagem, detalhes, atualização, desativação e exclusão).
/// </summary>
public class ContactsController(IContactService service, INotificationContext notifications)
    : ApiController(notifications)
{
    private readonly IContactService _service = service;

    /// <summary>Lista contatos ativos com paginação.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ContactResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
    {
        var result = await _service.GetPagedAsync(new PageQuery(page, pageSize), ct);
        return Respond(result);
    }

    /// <summary>Obtém os detalhes de um contato ativo.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ContactResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        var result = await _service.GetByIdAsync(id, ct);
        return Respond(result);
    }

    /// <summary>Cria um novo contato.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ContactResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] ContactCreateDto dto, CancellationToken ct)
    {
        var result = await _service.CreateAsync(dto, ct);
        if (Notifications.HasNotifications) return Respond();
        return CreatedAtAction(nameof(Get), new { id = result!.Id }, result);
    }

    /// <summary>Atualiza um contato existente.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ContactResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] ContactUpdateDto dto, CancellationToken ct)
    {
        var result = await _service.UpdateAsync(id, dto, ct);
        return Respond(result);
    }

    /// <summary>Desativa um contato.</summary>
    [HttpPatch("{id:guid}/deactivate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken ct)
    {
        await _service.DeactivateAsync(id, ct);
        if (Notifications.HasNotifications) return Respond();
        return NoContent();
    }

    /// <summary>Exclui permanentemente um contato.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _service.DeleteAsync(id, ct);
        if (Notifications.HasNotifications) return Respond();
        return NoContent();
    }
}
