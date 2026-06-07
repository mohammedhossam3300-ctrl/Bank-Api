using Bank.Application.DTOs;
using Bank.Application.Interfaces;
using Bank.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bank.Api.Controllers.Payment;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentTemplateController : ControllerBase
{
    private readonly IPaymentTemplateService _templateService;

    public PaymentTemplateController(
        IPaymentTemplateService templateService)
    {
        _templateService = templateService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateTemplate([FromBody] CreatePaymentTemplateRequest request)
    {
        var template = await _templateService.CreateTemplateAsync(request);
        return CreatedAtAction(nameof(GetTemplate), new { id = template.Id }, template);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTemplate(Guid id)
    {
        var template = await _templateService.GetTemplateAsync(id);
        if (template == null)
            return NotFound();

        return Ok(template);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserTemplates(Guid userId)
    {
        var templates = await _templateService.GetUserTemplatesAsync(userId);
        return Ok(templates);
    }

    [HttpGet("account/{accountId}")]
    public async Task<IActionResult> GetAccountTemplates(Guid accountId)
    {
        var templates = await _templateService.GetAccountTemplatesAsync(accountId);
        return Ok(templates);
    }

    [HttpGet("user/{userId}/category/{category}")]
    public async Task<IActionResult> GetTemplatesByCategory(Guid userId, PaymentTemplateCategory category)
    {
        var templates = await _templateService.GetTemplatesByCategoryAsync(userId, category);
        return Ok(templates);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTemplate(Guid id, [FromBody] UpdatePaymentTemplateRequest request)
    {
        var success = await _templateService.UpdateTemplateAsync(id, request);
        if (!success)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTemplate(Guid id)
    {
        var success = await _templateService.DeleteTemplateAsync(id);
        if (!success)
            return NotFound();

        return NoContent();
    }

    [HttpPost("{id}/activate")]
    public async Task<IActionResult> ActivateTemplate(Guid id)
    {
        var success = await _templateService.ActivateTemplateAsync(id);
        if (!success)
            return NotFound();

        return NoContent();
    }

    [HttpPost("{id}/deactivate")]
    public async Task<IActionResult> DeactivateTemplate(Guid id)
    {
        var success = await _templateService.DeactivateTemplateAsync(id);
        if (!success)
            return NotFound();

        return NoContent();
    }

    [HttpPost("{id}/execute")]
    public async Task<IActionResult> ExecuteTemplate(Guid id, [FromBody] ExecuteTemplateRequest request)
    {
        var transaction = await _templateService.ExecuteTemplateAsync(id, request);
        return Ok(transaction);
    }

    [HttpGet("user/{userId}/most-used")]
    public async Task<IActionResult> GetMostUsedTemplates(Guid userId, [FromQuery] int count = 10)
    {
        var templates = await _templateService.GetMostUsedTemplatesAsync(userId, count);
        return Ok(templates);
    }

    [HttpGet("user/{userId}/recently-used")]
    public async Task<IActionResult> GetRecentlyUsedTemplates(Guid userId, [FromQuery] int count = 10)
    {
        var templates = await _templateService.GetRecentlyUsedTemplatesAsync(userId, count);
        return Ok(templates);
    }

    [HttpGet("user/{userId}/search")]
    public async Task<IActionResult> SearchTemplates(Guid userId, [FromQuery] string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return BadRequest("Search term is required");

        var templates = await _templateService.SearchTemplatesAsync(userId, searchTerm);
        return Ok(templates);
    }

    [HttpGet("user/{userId}/tags")]
    public async Task<IActionResult> GetTemplatesByTags(Guid userId, [FromQuery] string[] tags)
    {
        if (tags == null || tags.Length == 0)
            return BadRequest("At least one tag is required");

        var templates = await _templateService.GetTemplatesByTagsAsync(userId, tags);
        return Ok(templates);
    }
}