using Top250Movies.Interfaces;

namespace Top250Movies.Services
{
    public class MyBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public MyBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (IServiceScope scope = _serviceProvider.CreateScope())
            {
                IUpdateData updateData = scope.ServiceProvider.GetRequiredService<IUpdateData>();

                while (!stoppingToken.IsCancellationRequested)
                {
                    if (DateTime.Now.DayOfWeek == DayOfWeek.Monday && DateTime.Now.Hour == 0 && DateTime.Now.Minute == 0) // Replace Day, hour and minute for testing
                    {
                        await updateData.UpdateDataAsync();
                    }
                    else
                    {
                        await Task.Delay(TimeSpan.FromMinutes(1));
                    }
                }
            }
        }
    }
}
