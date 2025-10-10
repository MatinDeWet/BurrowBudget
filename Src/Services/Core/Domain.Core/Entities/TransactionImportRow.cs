using System.Security.Cryptography;
using System.Text;
using Domain.Core.Enums;
using Domain.Core.Extensions;

namespace Domain.Core.Entities;

/// <summary>
/// A single raw line/record captured from the uploaded statement file,
/// plus progressively computed, normalized fields and final import outcome.
/// </summary>
public sealed class TransactionImportRow
{
    public Guid Id { get; private set; } = Guid.CreateVersion7();
    public Guid ImportBatchId { get; private set; }
    public TransactionImportBatch? ImportBatch { get; private set; }

    public int RawLineNumber { get; private set; }
    public string RawRecordJson { get; private set; } = null!;
    public DateOnly? RawDate { get; private set; }
    public string? RawAmountText { get; private set; }
    public string? RawDescription { get; private set; }
    public string? RawType { get; private set; }
    public string? RawBalanceText { get; private set; }
    public string? RawCurrency { get; private set; }
    public string? RawCounterparty { get; private set; }
    public string? RawReference { get; private set; }
    public string? RawFitId { get; private set; }

    /// <summary>
    /// Stable hash over salient raw attributes used for cross-file/batch de-duplication.
    /// Stored as lowercase hex SHA-256 (64 chars).
    /// </summary>
    public string RawHash { get; private set; } = null!;

    /// <summary>Signed amount in minor units; debits negative, credits positive. Example: ZAR -12345 = -R123.45.</summary>
    public long? SignedMinor { get; private set; }

    /// <summary>ISO-4217 currency code (e.g., ZAR). Required when SignedMinor is set.</summary>
    public string? Currency { get; private set; }

    /// <summary>Normalized (posting) date (file dates may be value/posted—choose your canonical).</summary>
    public DateOnly? NormalizedDate { get; private set; }

    public string? Payee { get; private set; }
    public string? Counterparty { get; private set; }
    public string? Memo { get; private set; }
    public string? Reference { get; private set; }
    public string? ExternalId { get; private set; }

    public Guid? SuggestedCategoryId { get; private set; }
    public double? MatchConfidence { get; private set; }

    public TransactionImportRowStatusEnum Status { get; private set; } = TransactionImportRowStatusEnum.Unprocessed;

    /// <summary>When row transitioned to its current status.</summary>
    public DateTimeOffset StatusAtUtc { get; private set; } = DateTimeOffset.UtcNow;

    public string? ErrorCode { get; private set; }
    public string? ErrorMessage { get; private set; }
    public string? ErrorLogJson { get; private set; }

    /// <summary>If marked as a duplicate, points to the canonical row (could be earlier batch).</summary>
    public Guid? DuplicateOfRowId { get; private set; }

    public static TransactionImportRow Create(
        Guid importBatchId,
        int rawLineNumber,
        string rawRecordJson,
        DateOnly? rawDate = null,
        string? rawAmountText = null,
        string? rawDescription = null,
        string? rawType = null,
        string? rawBalanceText = null,
        string? rawCurrency = null,
        string? rawCounterparty = null,
        string? rawReference = null,
        string? rawFitId = null)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(rawLineNumber, 1);
        ArgumentException.ThrowIfNullOrWhiteSpace(rawRecordJson);

        var row = new TransactionImportRow
        {
            ImportBatchId = importBatchId,
            RawLineNumber = rawLineNumber,
            RawRecordJson = rawRecordJson,
            RawDate = rawDate,
            RawAmountText = rawAmountText,
            RawDescription = rawDescription,
            RawType = rawType,
            RawBalanceText = rawBalanceText,
            RawCurrency = rawCurrency,
            RawCounterparty = rawCounterparty,
            RawReference = rawReference,
            RawFitId = rawFitId,
            RawHash = ComputeRawHash(
                rawLineNumber, rawRecordJson, rawDate, rawAmountText, rawDescription, rawType, rawCurrency, rawCounterparty, rawReference, rawFitId)
        };

        return row;
    }

    /// <summary>Called by parser after basic type parsing succeeded.</summary>
    public void MarkParsed(DateOnly? parsedDate, long? signedMinor, string? currency, string? payeeHint = null)
    {
        EnsureNotTerminal();
        NormalizedDate = parsedDate ?? NormalizedDate ?? RawDate;
        SignedMinor = signedMinor ?? SignedMinor;
        Currency = currency ?? Currency;
        if (!string.IsNullOrWhiteSpace(payeeHint))
        {
            Payee = payeeHint;
        }

        TransitionTo(TransactionImportRowStatusEnum.Parsed);
    }

    /// <summary>Apply normalization results (payee cleanup, reference mapping, category suggestion, etc.).</summary>
    public void ApplyNormalization(
        DateOnly? date = null,
        long? signedMinor = null,
        string? currency = null,
        string? payee = null,
        string? counterparty = null,
        string? memo = null,
        string? reference = null,
        string? externalId = null,
        Guid? suggestedCategoryId = null,
        double? matchConfidence = null)
    {
        EnsureNotTerminal();
        NormalizedDate = date ?? NormalizedDate ?? RawDate;
        SignedMinor = signedMinor ?? SignedMinor;
        Currency = currency ?? Currency;
        Payee = CoalesceNonEmpty(payee, Payee);
        Counterparty = CoalesceNonEmpty(counterparty, Counterparty);
        Memo = CoalesceNonEmpty(memo, Memo);
        Reference = CoalesceNonEmpty(reference, Reference);
        ExternalId = CoalesceNonEmpty(externalId, ExternalId) ?? RawFitId;
        SuggestedCategoryId = suggestedCategoryId ?? SuggestedCategoryId;
        MatchConfidence = matchConfidence ?? MatchConfidence;

        TransitionTo(TransactionImportRowStatusEnum.Normalized);
    }

    public void MarkDuplicate(Guid canonicalRowId, double? confidence = 1.0, string? reason = null)
    {
        EnsureNotTerminal();
        DuplicateOfRowId = canonicalRowId;
        MatchConfidence = confidence ?? MatchConfidence;
        ErrorCode = string.IsNullOrWhiteSpace(ErrorCode) ? "DUPLICATE" : ErrorCode;
        ErrorMessage = reason ?? ErrorMessage;
        TransitionTo(TransactionImportRowStatusEnum.Duplicate);
    }

    public void MarkRejected(string code, string message, string? errorLogJson = null)
    {
        EnsureNotTerminal();
        ErrorCode = code;
        ErrorMessage = message;
        if (!string.IsNullOrWhiteSpace(errorLogJson))
        {
            ErrorLogJson = errorLogJson;
        }

        TransitionTo(TransactionImportRowStatusEnum.Rejected);
    }

    public void MarkSkipped(string? reason = null)
    {
        EnsureNotTerminal();
        ErrorCode ??= "SKIPPED";
        ErrorMessage = reason ?? ErrorMessage;
        TransitionTo(TransactionImportRowStatusEnum.Skipped);
    }

    public void MarkImported(Guid ledgerTransactionId)
    {
        EnsureNotTerminal();
        TransitionTo(TransactionImportRowStatusEnum.Imported);
    }

    private void TransitionTo(TransactionImportRowStatusEnum newStatus)
    {
        if (Status != newStatus && !TransactionImportRowStatusPolicyExtension.CanTransition(Status, newStatus))
        {
            throw new InvalidOperationException($"Invalid transition {Status} -> {newStatus}");
        }

        Status = newStatus;
        StatusAtUtc = DateTimeOffset.UtcNow;
    }

    private void EnsureNotTerminal()
    {
        if (TransactionImportRowStatusPolicyExtension.IsTerminal(Status))
        {
            throw new InvalidOperationException($"Row is terminal in state {Status} and cannot be mutated.");
        }
    }

    private static string? CoalesceNonEmpty(string? a, string? b)
        => !string.IsNullOrWhiteSpace(a) ? a : (!string.IsNullOrWhiteSpace(b) ? b : null);

    private static string ComputeRawHash(
        int line, string rawJson, DateOnly? rawDate, string? rawAmount, string? rawDesc, string? rawType,
        string? rawCurrency, string? rawCounterparty, string? rawRef, string? rawFitId)
    {
        string payload = $"{line}|{rawDate:yyyy-MM-dd}|{rawAmount}|{rawDesc}|{rawType}|{rawCurrency}|{rawCounterparty}|{rawRef}|{rawFitId}|{rawJson}";
        byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(payload));
        return Convert.ToHexString(bytes).ToUpperInvariant();
    }
}
