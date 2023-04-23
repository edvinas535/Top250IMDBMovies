using Top250Movies.Repository;
using Top250Movies.Interfaces;

namespace Top250Movies.Services
{
    public class MyHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public MyHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using (IServiceScope scope = _serviceProvider.CreateScope())
            {
                IDBTables movieService = scope.ServiceProvider.GetRequiredService<IDBTables>();
                await movieService.CreateDBAsync();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
