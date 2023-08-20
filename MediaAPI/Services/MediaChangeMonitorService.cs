namespace MediaAPI.Services
{
    public class MediaChangeMonitorService: BackgroundService
    {
        private readonly string _uploadPath;

        public MediaChangeMonitorService(IConfiguration config)
        {
            _uploadPath = config["uploadPath"];
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var watcher = new FileSystemWatcher(_uploadPath);
            watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;

            watcher.Changed += (sender, e) =>
            {
                Console.WriteLine($"File {e.Name} has been changed.");
            };

            watcher.Created += (sender, e) =>
            {
                Console.WriteLine($"File {e.Name} has been created.");
            };

            watcher.EnableRaisingEvents = true;

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}
