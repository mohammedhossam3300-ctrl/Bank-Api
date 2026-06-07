using Bank.Application.DTOs;
using Bank.Application.DTOs.Statement.Core;
using Bank.Application.DTOs.Statement.Delivery;
using Bank.Application.DTOs.Statement.Analytics;
using Bank.Domain.Entities;
using Bank.Domain.Enums;

namespace Bank.Application.Interfaces;

/// <summary>
/// Interface for generating statement files in different formats
/// </summary>
public interface IStatementGenerator
{
    /// <summary>
    /// Generate statement in PDF format
    /// </summary>
    Task<(byte[] Content, string FileName)> GeneratePdfStatementAsync(AccountStatement statement, StatementTemplate? template = null);
    
    /// <summary>
    /// Generate statement in CSV format
    /// </summary>
    Task<(byte[] Content, string FileName)> GenerateCsvStatementAsync(AccountStatement statement);
    
    /// <summary>
    /// Generate statement in Excel format
    /// </summary>
    Task<(byte[] Content, string FileName)> GenerateExcelStatementAsync(AccountStatement statement);
    
    /// <summary>
    /// Generate statement in HTML format
    /// </summary>
    Task<(byte[] Content, string FileName)> GenerateHtmlStatementAsync(AccountStatement statement, StatementTemplate? template = null);
    
    /// <summary>
    /// Generate statement in JSON format
    /// </summary>
    Task<(byte[] Content, string FileName)> GenerateJsonStatementAsync(AccountStatement statement);
    
    /// <summary>
    /// Generate consolidated statement for multiple accounts
    /// </summary>
    Task<(byte[] Content, string FileName)> GenerateConsolidatedStatementAsync(List<AccountStatement> statements, ConsolidatedStatementRequest request, StatementTemplate? template = null);
    
    /// <summary>
    /// Get content type for statement format
    /// </summary>
    string GetContentType(StatementFormat format);
    
    /// <summary>
    /// Get file extension for statement format
    /// </summary>
    string GetFileExtension(StatementFormat format);
}
