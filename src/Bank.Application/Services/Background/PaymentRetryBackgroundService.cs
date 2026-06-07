using Bank.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Bank.Application.Services;

/// <summary>
/// Background service for processing payment retries and health checks
/// </summary>
public class PaymentRetryBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<PaymentRetryBackgroundService> _logger;
    private readonly TimeSpan _retryProcessingInterval = TimeSpan.FromMinutes(5);
    private readonly TimeSpan _healthCheckInterval = TimeSpan.FromMinutes(15);
    private readonly TimeSpan _maxRetriesProcessingInterval = TimeSpan.FromHours(1);

    public PaymentRetryBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<PaymentRetryBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Payment Retry Background Service started");

        var retryProcessingTask = ProcessRetryPaymentsAsync(stoppingToken);
        var healthCheckTask = ProcessHealthChecksAsync(stoppingToken);
        var maxRetriesTask = ProcessMaxRetriesReachedAsync(stoppingToken);

        await Task.WhenAll(retryProcessingTask, healthCheckTask, maxRetriesTask);
    }

    private async Task ProcessRetryPaymentsAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var paymentRetryService = scope.ServiceProvider.GetRequiredService<IPaymentRetryService>();

                var results = await paymentRetryService.ProcessRetryPaymentsAsync();
                
                if (results.Any())
                {
                    var successCount = results.Count(r => r.Success);
                    var failureCount = results.Count - successCount;
                    
                    _logger.LogInformation("Processed {Total} retry payments: {Success} successful, {Failed} failed", 
                        results.Count, successCount, failureCount);
                }

                await Task.Delay(_retryProcessingInterval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Expected when cancellation is requested
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing retry payments");
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); // Wait before retrying
            }
        }
    }

    private async Task ProcessHealthChecksAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var billerIntegrationService = scope.ServiceProvider.GetRequiredService<IBillerIntegrationService>();
                var billerRepository = scope.ServiceProvider.GetRequiredService<Bank.Domain.Interfaces.IBillerRepository>();

                // Get all active billers
                var activeBillers = await billerRepository.GetActiveBillersAsync();
                
                var healthCheckTasks = activeBillers.Select(async biller =>
                {
                    try
                    {
                        var healthCheck = await billerIntegrationService.CheckBillerHealthAsync(biller.Id);
                        return new { BillerId = biller.Id, BillerName = biller.Name, IsHealthy = healthCheck.IsHealthy };
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error checking health for biller {BillerId}", biller.Id);
                        return new { BillerId = biller.Id, BillerName = biller.Name, IsHealthy = false };
                    }
                });

                var healthResults = await Task.WhenAll(healthCheckTasks);
                
                var healthyCount = healthResults.Count(r => r.IsHealthy);
                var unhealthyCount = healthResults.Length - healthyCount;
                
                _logger.LogInformation("Health check completed for {Total} billers: {Healthy} healthy, {Unhealthy} unhealthy", 
                    healthResults.Length, healthyCount, unhealthyCount);

                if (unhealthyCount > 0)
                {
                    var unhealthyBillers = healthResults.Where(r => !r.IsHealthy).Select(r => r.BillerName);
                    _logger.LogWarning("Unhealthy billers: {UnhealthyBillers}", string.Join(", ", unhealthyBillers));
                }

                await Task.Delay(_healthCheckInterval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Expected when cancellation is requested
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing health checks");
                await Task.Delay(TimeSpan.FromMinutes(2), stoppingToken); // Wait before retrying
            }
        }
    }

    private async Task ProcessMaxRetriesReachedAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var paymentRetryService = scope.ServiceProvider.GetRequiredService<IPaymentRetryService>();

                var processedPaymentIds = await paymentRetryService.ProcessMaxRetriesReachedAsync();
                
                if (processedPaymentIds.Any())
                {
                    _logger.LogWarning("Processed {Count} payments that reached maximum retry attempts", 
                        processedPaymentIds.Count);
                }

                await Task.Delay(_maxRetriesProcessingInterval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Expected when cancellation is requested
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing max retries reached payments");
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken); // Wait before retrying
            }
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Payment Retry Background Service is stopping");
        await base.StopAsync(stoppingToken);
    }
}