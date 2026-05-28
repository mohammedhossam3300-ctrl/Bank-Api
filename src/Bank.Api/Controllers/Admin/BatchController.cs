using Bank.Application.Interfaces;
using Bank.Domain.Enums;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

namespace Bank.Api.Controllers.Admin;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BatchController : ControllerBase
{
    private readonly IBatchService _batchService;
    private readonly IBackgroundJobClient _backgroundJobClient;

    public BatchController(IBatchService batchService, IBackgroundJobClient backgroundJobClient)
    {
        _batchService = batchService;
        _backgroundJobClient = backgroundJobClient;
    }

    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadBatch(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        // Simulate file parsing (In a real app, parse CSV/JSON)
        using var rng = RandomNumberGenerator.Create();
        var randomBytes = new byte[4];
        rng.GetBytes(randomBytes);
        var totalRecords = Math.Abs(BitConverter.ToInt32(randomBytes, 0) % 90) + 10; // 10-100 range
        var job = await _batchService.CreateBatchJobAsync(file.FileName, totalRecords);

        // Mock some transactions for the batch
        var mockTransactions = Enumerable.Range(0, totalRecords).Select(_ => {
            rng.GetBytes(randomBytes);
            var amount = Math.Abs(BitConverter.ToInt32(randomBytes, 0) % 990) + 10; // 10-1000 range
            return new TransactionRequest(
                Guid.NewGuid(), Guid.NewGuid(), amount, TransactionType.ACH, "Batch processing"
            );
        });

        // Enqueue background job
        _backgroundJobClient.Enqueue<IBatchService>(x => x.ProcessBatchAsync(job.Id, mockTransactions));

        return Ok(new { Message = "Batch file uploaded and processing initiated.", JobId = job.Id });
    }

    [HttpGet("status/{jobId}")]
    public async Task<IActionResult> GetStatus(Guid jobId)
    {
        var job = await _batchService.GetBatchJobStatusAsync(jobId);
        if (job == null) return NotFound();
        return Ok(job);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllBatches()
    {
        var jobs = await _batchService.GetAllBatchJobsAsync();
        return Ok(jobs);
    }
}
