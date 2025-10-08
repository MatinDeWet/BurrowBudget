using System.Collections.Immutable;
using Domain.Core.Enums;

namespace Domain.Core.Extensions;
public static class TransactionJobStatusPolicyExtension
{
    private static readonly ImmutableDictionary<TransactionImportBatchStatusEnum, TransactionImportBatchStatusEnum[]> Allowed =
        new Dictionary<TransactionImportBatchStatusEnum, TransactionImportBatchStatusEnum[]>
        {
            [TransactionImportBatchStatusEnum.PendingFileUpload] = [TransactionImportBatchStatusEnum.FileUploaded, TransactionImportBatchStatusEnum.Canceled],
            [TransactionImportBatchStatusEnum.FileUploaded] = [TransactionImportBatchStatusEnum.Queued, TransactionImportBatchStatusEnum.Canceled, TransactionImportBatchStatusEnum.Duplicate],
            [TransactionImportBatchStatusEnum.Queued] = [TransactionImportBatchStatusEnum.Processing, TransactionImportBatchStatusEnum.Canceled],
            [TransactionImportBatchStatusEnum.Processing] = [TransactionImportBatchStatusEnum.Completed, TransactionImportBatchStatusEnum.Failed, TransactionImportBatchStatusEnum.Canceled, TransactionImportBatchStatusEnum.Superseded],
        }.ToImmutableDictionary();

    private static readonly HashSet<TransactionImportBatchStatusEnum> Terminal = [TransactionImportBatchStatusEnum.Completed, TransactionImportBatchStatusEnum.Failed, TransactionImportBatchStatusEnum.Canceled, TransactionImportBatchStatusEnum.Duplicate, TransactionImportBatchStatusEnum.Superseded];

    public static bool CanTransition(TransactionImportBatchStatusEnum current, TransactionImportBatchStatusEnum next) =>
        Allowed.TryGetValue(current, out TransactionImportBatchStatusEnum[]? nexts) && nexts.Contains(next);

    public static bool IsTerminal(TransactionImportBatchStatusEnum s) => Terminal.Contains(s);

    public static IReadOnlyList<TransactionImportBatchStatusEnum> AllowedNext(TransactionImportBatchStatusEnum current) =>
        Allowed.TryGetValue(current, out TransactionImportBatchStatusEnum[]? nexts) ? nexts : [];
}
