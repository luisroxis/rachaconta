using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RachaConta.Application.DTOs.Request;
using RachaConta.Application.UseCases;
using RachaConta.Core.Entities;

namespace RachaConta.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InviteController : ControllerBase
{
    private readonly SendInviteUseCase _sendInviteUseCase;
    private readonly ResendInviteUseCase _resendInviteUseCase;
    private readonly DeleteInviteUseCase _deleteInviteUseCase;
    private readonly ListInvitesUseCase _listInvitesUseCase;

    public InviteController(
        SendInviteUseCase sendInviteUseCase,
        ResendInviteUseCase resendInviteUseCase,
        DeleteInviteUseCase deleteInviteUseCase,
        ListInvitesUseCase listInvitesUseCase)
    {
        _sendInviteUseCase = sendInviteUseCase;
        _resendInviteUseCase = resendInviteUseCase;
        _deleteInviteUseCase = deleteInviteUseCase;
        _listInvitesUseCase = listInvitesUseCase;
    }

    private Guid GetAuthenticatedUserId()
    {
        var userId = HttpContext.Items["AuthenticatedUserId"] as Guid?;
        if (userId == null)
        {
            throw new UnauthorizedAccessException("Usuário não autenticado.");
        }
        return userId.Value;
    }

    [HttpPost]
    public async Task<IActionResult> SendInvite([FromBody] SendInviteRequest request)
    {
        try
        {
            var usuarioId = GetAuthenticatedUserId();
            var response = await _sendInviteUseCase.ExecuteAsync(request, usuarioId);
            return CreatedAtAction(nameof(SendInvite), new { id = response.Id }, response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Ocorreu um erro ao processar sua solicitação.", details = ex.Message });
        }
    }

    [HttpPost("{id}/resend")]
    public async Task<IActionResult> ResendInvite(Guid id)
    {
        try
        {
            var usuarioId = GetAuthenticatedUserId();
            var response = await _resendInviteUseCase.ExecuteAsync(id, usuarioId);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Ocorreu um erro ao processar sua solicitação.", details = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteInvite(Guid id)
    {
        try
        {
            var usuarioId = GetAuthenticatedUserId();
            await _deleteInviteUseCase.ExecuteAsync(id, usuarioId);
            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Ocorreu um erro ao processar sua solicitação.", details = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> ListInvites()
    {
        try
        {
            var usuarioId = GetAuthenticatedUserId();
            var response = await _listInvitesUseCase.ExecuteAsync(usuarioId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Ocorreu um erro ao processar sua solicitação.", details = ex.Message });
        }
    }
}
