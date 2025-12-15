using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RachaConta.Application.DTOs.Request;
using RachaConta.Application.DTOs.Response;
using RachaConta.Application.UseCases;
using RachaConta.Core.Entities;

namespace RachaConta.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GrupoController : ControllerBase
{
    private readonly CreateGrupoUseCase _createGrupoUseCase;
    private readonly ListGruposUseCase _listGruposUseCase;
    private readonly GetGrupoDetailsUseCase _getGrupoDetailsUseCase;
    private readonly AddParticipantesGrupoUseCase _addParticipantesUseCase;
    private readonly PromoteParticipanteUseCase _promoteParticipanteUseCase;
    private readonly DeleteParticipanteGrupoUseCase _deleteParticipanteUseCase;

    public GrupoController(
        CreateGrupoUseCase createGrupoUseCase,
        ListGruposUseCase listGruposUseCase,
        GetGrupoDetailsUseCase getGrupoDetailsUseCase,
        AddParticipantesGrupoUseCase addParticipantesUseCase,
        PromoteParticipanteUseCase promoteParticipanteUseCase,
        DeleteParticipanteGrupoUseCase deleteParticipanteUseCase)
    {
        _createGrupoUseCase = createGrupoUseCase;
        _listGruposUseCase = listGruposUseCase;
        _getGrupoDetailsUseCase = getGrupoDetailsUseCase;
        _addParticipantesUseCase = addParticipantesUseCase;
        _promoteParticipanteUseCase = promoteParticipanteUseCase;
        _deleteParticipanteUseCase = deleteParticipanteUseCase;
    }

    private Guid GetAuthenticatedUserId()
    {
        var userId = HttpContext.Items["AuthenticatedUserId"] as Guid?;
        if (userId == null)
            throw new UnauthorizedAccessException("Usuário não autenticado.");
        return userId.Value;
    }

    [HttpPost]
    public async Task<IActionResult> CreateGrupo([FromForm] CreateGrupoRequest request, IFormFile? imgGrupo)
    {
        try
        {
            var usuarioId = GetAuthenticatedUserId();
            string? imgGrupoPath = null;

            if (imgGrupo != null)
            {
                // TODO: Implementar upload para MinIO
                // imgGrupoPath = await _minioService.UploadAsync(imgGrupo);
            }

            var response = await _createGrupoUseCase.ExecuteAsync(request, usuarioId, imgGrupoPath);
            return CreatedAtAction(nameof(GetGrupoDetails), new { id = response.Id }, response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Ocorreu um erro ao criar o grupo.", details = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> ListGrupos()
    {
        try
        {
            var usuarioId = GetAuthenticatedUserId();
            var response = await _listGruposUseCase.ExecuteAsync(usuarioId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Ocorreu um erro ao listar os grupos.", details = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetGrupoDetails(Guid id)
    {
        try
        {
            var usuarioId = GetAuthenticatedUserId();
            var (grupo, participantes) = await _getGrupoDetailsUseCase.ExecuteAsync(id, usuarioId);
            return Ok(new { grupo, participantes });
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
            return StatusCode(500, new { message = "Ocorreu um erro ao obter detalhes do grupo.", details = ex.Message });
        }
    }

    [HttpPost("{id}/participantes")]
    public async Task<IActionResult> AddParticipantes(Guid id, [FromBody] AddParticipantesGrupoRequest request)
    {
        try
        {
            var usuarioId = GetAuthenticatedUserId();
            request.GrupoId = id;
            var response = await _addParticipantesUseCase.ExecuteAsync(request, usuarioId);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Ocorreu um erro ao adicionar participantes.", details = ex.Message });
        }
    }

    [HttpPost("{id}/participantes/{participanteId}/promote")]
    public async Task<IActionResult> PromoteParticipante(Guid id, Guid participanteId)
    {
        try
        {
            var usuarioId = GetAuthenticatedUserId();
            var request = new PromoteParticipanteRequest { GrupoId = id, ParticipanteId = participanteId };
            var response = await _promoteParticipanteUseCase.ExecuteAsync(request, usuarioId);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Ocorreu um erro ao promover participante.", details = ex.Message });
        }
    }

    [HttpDelete("participantes/{participanteId}")]
    public async Task<IActionResult> DeleteParticipante(Guid participanteId)
    {
        try
        {
            var usuarioId = GetAuthenticatedUserId();
            await _deleteParticipanteUseCase.ExecuteAsync(participanteId, usuarioId);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Ocorreu um erro ao remover participante.", details = ex.Message });
        }
    }
}
