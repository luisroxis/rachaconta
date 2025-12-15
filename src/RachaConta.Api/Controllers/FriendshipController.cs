using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RachaConta.Application.DTOs.Request;
using RachaConta.Application.UseCases;
using RachaConta.Core.Entities;

namespace RachaConta.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FriendshipController : ControllerBase
{
    private readonly ListUsersUseCase _listUsersUseCase;
    private readonly RequestFriendshipUseCase _requestFriendshipUseCase;
    private readonly ListPendingFriendshipsUseCase _listPendingFriendshipsUseCase;
    private readonly ApproveFriendshipUseCase _approveFriendshipUseCase;
    private readonly ListAcceptedFriendshipsUseCase _listAcceptedFriendshipsUseCase;

    public FriendshipController(
        ListUsersUseCase listUsersUseCase,
        RequestFriendshipUseCase requestFriendshipUseCase,
        ListPendingFriendshipsUseCase listPendingFriendshipsUseCase,
        ApproveFriendshipUseCase approveFriendshipUseCase,
        ListAcceptedFriendshipsUseCase listAcceptedFriendshipsUseCase)
    {
        _listUsersUseCase = listUsersUseCase;
        _requestFriendshipUseCase = requestFriendshipUseCase;
        _listPendingFriendshipsUseCase = listPendingFriendshipsUseCase;
        _approveFriendshipUseCase = approveFriendshipUseCase;
        _listAcceptedFriendshipsUseCase = listAcceptedFriendshipsUseCase;
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

    private string GetAuthenticatedUserName()
    {
        var user = HttpContext.Items["AuthenticatedUser"] as User;
        if (user == null)
        {
            throw new UnauthorizedAccessException("Usuário não autenticado.");
        }
        return user.UserName;
    }

    [HttpGet("users")]
    public async Task<IActionResult> ListUsers()
    {
        try
        {
            var userId = GetAuthenticatedUserId();
            var response = await _listUsersUseCase.ExecuteAsync(userId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Ocorreu um erro ao listar usuários.", details = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> RequestFriendship([FromBody] RequestFriendshipRequest request)
    {
        try
        {
            var userId = GetAuthenticatedUserId();
            var response = await _requestFriendshipUseCase.ExecuteAsync(request, userId);
            return CreatedAtAction(nameof(RequestFriendship), new { id = response.Id }, response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Ocorreu um erro ao solicitar amizade.", details = ex.Message });
        }
    }

    [HttpGet("pending")]
    public async Task<IActionResult> ListPending([FromQuery] bool incoming = true)
    {
        try
        {
            var userId = GetAuthenticatedUserId();
            var response = await _listPendingFriendshipsUseCase.ExecuteAsync(userId, incoming);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Ocorreu um erro ao listar solicitações pendentes.", details = ex.Message });
        }
    }

    [HttpPost("{id}/approve")]
    public async Task<IActionResult> ApproveFriendship(Guid id, [FromBody] ApproveFriendshipRequest request)
    {
        try
        {
            var userId = GetAuthenticatedUserId();
            var response = await _approveFriendshipUseCase.ExecuteAsync(id, request, userId);
            
            if (response == null)
            {
                return NoContent(); // Rejected/Deleted
            }

            return Ok(response); // Aproved
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Ocorreu um erro ao responder solicitação de amizade.", details = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> ListAccepted()
    {
        try
        {
            var userId = GetAuthenticatedUserId();
            var response = await _listAcceptedFriendshipsUseCase.ExecuteAsync(userId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Ocorreu um erro ao listar amizades.", details = ex.Message });
        }
    }
}
