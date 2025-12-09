using Microsoft.AspNetCore.Mvc;
using RachaConta.Application.DTOs;
using RachaConta.Application.DTOs.Request;
using RachaConta.Application.DTOs.Response;
using RachaConta.Application.UseCases;

namespace RachaConta.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly RegisterUserUseCase _registerUserUseCase;
    private readonly RequestPasswordRecoveryUseCase _requestPasswordRecoveryUseCase;
    private readonly ResetPasswordUseCase _resetPasswordUseCase;
    private readonly LoginUseCase _loginUseCase;

    public UserController(
        RegisterUserUseCase registerUserUseCase,
        RequestPasswordRecoveryUseCase requestPasswordRecoveryUseCase,
        ResetPasswordUseCase resetPasswordUseCase,
        LoginUseCase loginUseCase)
    {
        _registerUserUseCase = registerUserUseCase;
        _requestPasswordRecoveryUseCase = requestPasswordRecoveryUseCase;
        _resetPasswordUseCase = resetPasswordUseCase;
        _loginUseCase = loginUseCase;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var response = await _loginUseCase.ExecuteAsync(request);
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

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
    {
        try
        {
            var response = await _registerUserUseCase.ExecuteAsync(request);
            return CreatedAtAction(nameof(Register), new { id = response.UserName }, response);
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

    [HttpPost("forgot-password")]
    public async Task<IActionResult> RequestPasswordRecovery([FromBody] RequestPasswordRecoveryRequest request)
    {
        try
        {
            var response = await _requestPasswordRecoveryUseCase.ExecuteAsync(request);
            return Ok(response);
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

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        try
        {
            var response = await _resetPasswordUseCase.ExecuteAsync(request);
            return Ok(response);
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
