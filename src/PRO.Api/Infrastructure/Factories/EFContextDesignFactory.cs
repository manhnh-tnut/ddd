using MediatR;

using Microsoft.EntityFrameworkCore;
using PRO.Infrastructure.Data;

namespace PRO.Api.Infrastructure.Factories;

public class EFContextDesignFactory : Microsoft.EntityFrameworkCore.Design.IDesignTimeDbContextFactory<EFContext>
{
    private string connectionString = "Server=127.0.0.1,1433;Database=ddd;User Id=sa;Password=Pa$$w0rd;TrustServerCertificate=True;Integrated Security=false;MultipleActiveResultSets=true;";
    public EFContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<EFContext>()
            .UseSqlServer(connectionString);

        return new EFContext(optionsBuilder.Options, new NoMediator());
    }

    class NoMediator : IMediator
    {
        public IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            return default(IAsyncEnumerable<TResponse>);
        }

        public IAsyncEnumerable<object> CreateStream(object request, CancellationToken cancellationToken = default)
        {
            return default(IAsyncEnumerable<object>);
        }

        public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification
        {
            return Task.CompletedTask;
        }

        public Task Publish(object notification, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<TResponse>(default(TResponse));
        }

        public Task<object> Send(object request, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(default(object));
        }
    }
}
