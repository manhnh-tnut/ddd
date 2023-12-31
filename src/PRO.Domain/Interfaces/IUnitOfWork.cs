﻿using PRO.Domain.Base;
using System.Threading.Tasks;

namespace PRO.Domain.Interfaces
{
    public interface IUnitOfWork: IDisposable
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
        Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}