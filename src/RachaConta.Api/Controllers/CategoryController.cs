using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RachaConta.Application.DTOs.Request;
using RachaConta.Application.UseCases;

namespace RachaConta.Api.Controllers;

[ApiController]
[Route("api/categories")]
[Authorize]
public class CategoryController : ControllerBase
{
    private readonly CreateCategoryUseCase _createCategoryUseCase;
    private readonly ListCategoriesUseCase _listCategoriesUseCase;

    public CategoryController(
        CreateCategoryUseCase createCategoryUseCase,
        ListCategoriesUseCase listCategoriesUseCase)
    {
        _createCategoryUseCase = createCategoryUseCase;
        _listCategoriesUseCase = listCategoriesUseCase;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request)
    {
        try
        {
            var response = await _createCategoryUseCase.ExecuteAsync(request);
            // Returning 201 Created with the response body
            return StatusCode(201, response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            // 409 Conflict if already exists? Or 400? UseCase throws InvokeOp for duplicate.
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Ocorreu um erro ao processar sua solicitação.", details = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        try
        {
            var response = await _listCategoriesUseCase.ExecuteAsync();
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Ocorreu um erro ao processar sua solicitação.", details = ex.Message });
        }
    }
}
