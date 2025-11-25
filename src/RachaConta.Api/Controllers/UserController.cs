using Microsoft.AspNetCore.Mvc;
using RachaConta.Application.DTOs;
using RachaConta.Application.UseCases;

namespace RachaConta.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly RegisterUserUseCase _registerUserUseCase;

    public UserController(RegisterUserUseCase registerUserUseCase)
    {
        _registerUserUseCase = registerUserUseCase;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
    {
        try
        {
            var response = await _registerUserUseCase.ExecuteAsync(request);
            return CreatedAtAction(nameof(Register), new { id = response.Id }, response);
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
}
