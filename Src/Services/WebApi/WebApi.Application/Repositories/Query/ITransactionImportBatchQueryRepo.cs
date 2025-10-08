using Domain.Core.Entities;
using Repository.Base;

namespace WebApi.Application.Repositories.Query;
public interface ITransactionImportBatchQueryRepo : ISecureQueryRepo
{
    IQueryable<TransactionImportBatch> TransactionImportBatches { get; }
}
