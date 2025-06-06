namespace HotelBookingSystem.RoomManagementService.Domain.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IRoomTypeRepository RoomTypes { get; }
        
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        int SaveChanges();
        
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    }
} 