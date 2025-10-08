using Domain.Core.Entities;
using Repository.Base;

namespace WebApi.Application.Repositories.Query;
public interface ITransactionImportRowQueryRepo : ISecureQueryRepo
{
    IQueryable<TransactionImportRow> TransactionImportRows { get; }
}
