using HRVault.SharedKernel.Interfaces;

namespace HRVault.SharedKernel.Common;

public abstract class BaseEntity : IEntity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
}