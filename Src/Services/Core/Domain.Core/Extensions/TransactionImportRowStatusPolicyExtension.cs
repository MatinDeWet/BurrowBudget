using System.Collections.Immutable;
using Domain.Core.Enums;

namespace Domain.Core.Extensions;

public static class TransactionImportRowStatusPolicyExtension
{
    private static readonly ImmutableDictionary<TransactionImportRowStatusEnum, TransactionImportRowStatusEnum[]> Allowed =
        new Dictionary<TransactionImportRowStatusEnum, TransactionImportRowStatusEnum[]>
        {
            [TransactionImportRowStatusEnum.Unprocessed] = [
                TransactionImportRowStatusEnum.Parsed,
                TransactionImportRowStatusEnum.Normalized,
                TransactionImportRowStatusEnum.Duplicate,
                TransactionImportRowStatusEnum.Rejected,
                TransactionImportRowStatusEnum.Skipped
            ],
            [TransactionImportRowStatusEnum.Parsed] = [
                TransactionImportRowStatusEnum.Normalized,
                TransactionImportRowStatusEnum.Duplicate,
                TransactionImportRowStatusEnum.Rejected,
                TransactionImportRowStatusEnum.Skipped
            ],
            [TransactionImportRowStatusEnum.Normalized] = [
                TransactionImportRowStatusEnum.Imported,
                TransactionImportRowStatusEnum.Duplicate,
                TransactionImportRowStatusEnum.Rejected,
                TransactionImportRowStatusEnum.Skipped
            ],
        }.ToImmutableDictionary();

    private static readonly HashSet<TransactionImportRowStatusEnum> Terminal = [
        TransactionImportRowStatusEnum.Duplicate,
        TransactionImportRowStatusEnum.Rejected,
        TransactionImportRowStatusEnum.Skipped,
        TransactionImportRowStatusEnum.Imported
    ];

    public static bool CanTransition(TransactionImportRowStatusEnum current, TransactionImportRowStatusEnum next) =>
        Allowed.TryGetValue(current, out TransactionImportRowStatusEnum[]? nexts) && nexts.Contains(next);

    public static bool IsTerminal(TransactionImportRowStatusEnum status) => Terminal.Contains(status);

    public static IReadOnlyList<TransactionImportRowStatusEnum> AllowedNext(TransactionImportRowStatusEnum current) =>
        Allowed.TryGetValue(current, out TransactionImportRowStatusEnum[]? nexts) ? nexts : [];
}
