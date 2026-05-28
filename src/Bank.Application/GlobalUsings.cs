// Global using directives - fixes all missing type errors across the application layer
// These replace the broad 'using Bank.Application.DTOs;' in interfaces and services

// Account DTOs
global using Bank.Application.DTOs.Account.Core;
global using Bank.Application.DTOs.Account.JointAccount;
global using Bank.Application.DTOs.Account.Lockout;
global using Bank.Application.DTOs.Account.Profile;
global using Bank.Application.DTOs.Account.Transfer;
global using Bank.Application.DTOs.Account.Validation;

// Auth DTOs
global using Bank.Application.DTOs.Auth.Core;
global using Bank.Application.DTOs.Auth.Security;
global using Bank.Application.DTOs.Auth.Session;
global using Bank.Application.DTOs.Auth.TwoFactor;

// Card DTOs
global using Bank.Application.DTOs.Card.Activation;
global using Bank.Application.DTOs.Card.Advanced;
global using Bank.Application.DTOs.Card.Core;
global using Bank.Application.DTOs.Card.Fees;
global using Bank.Application.DTOs.Card.Operations;
global using Bank.Application.DTOs.Card.Transactions;

// Deposit DTOs
global using Bank.Application.DTOs.Deposit.Core;
global using Bank.Application.DTOs.Deposit.FixedDeposit;
global using Bank.Application.DTOs.Deposit.Interest;
global using Bank.Application.DTOs.Deposit.Maturity;
global using Bank.Application.DTOs.Deposit.Withdrawal;

// Loan DTOs
global using Bank.Application.DTOs.Loan.Analytics;
global using Bank.Application.DTOs.Loan.Application;
global using Bank.Application.DTOs.Loan.Approval;
global using Bank.Application.DTOs.Loan.Configuration;
global using Bank.Application.DTOs.Loan.Core;
global using Bank.Application.DTOs.Loan.Disbursement;
global using Bank.Application.DTOs.Loan.Repayment;

// Payment DTOs
global using Bank.Application.DTOs.Payment.Batch;
global using Bank.Application.DTOs.Payment.Beneficiary;
global using Bank.Application.DTOs.Payment.Biller;
global using Bank.Application.DTOs.Payment.Core;
global using Bank.Application.DTOs.Payment.Receipt;
global using Bank.Application.DTOs.Payment.Recurring;
global using Bank.Application.DTOs.Payment.Routing;
global using Bank.Application.DTOs.Payment.Template;

// Shared DTOs
global using Bank.Application.DTOs.Shared.Audit;
global using Bank.Application.DTOs.Shared.Notification;
global using Bank.Application.DTOs.Shared.RateLimit;

// Statement DTOs
global using Bank.Application.DTOs.Statement.Analytics;
global using Bank.Application.DTOs.Statement.Core;
global using Bank.Application.DTOs.Statement.Delivery;
global using Bank.Application.DTOs.Statement.Search;
global using Bank.Application.DTOs.Statement.Summary;
global using Bank.Application.DTOs.Statement.Transaction;

// Transaction DTOs
global using Bank.Application.DTOs.Transaction.Analytics;
global using Bank.Application.DTOs.Transaction.Core;
global using Bank.Application.DTOs.Transaction.Fraud;
global using Bank.Application.DTOs.Transaction.Search;

// Domain
global using Bank.Domain.Common;
global using Bank.Domain.Entities;
global using Bank.Domain.Enums;
global using Bank.Application.Interfaces.Payment;

global using Bank.Application.DTOs;

