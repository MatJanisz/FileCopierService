using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace FileCopier
{
	public class IntervalRunner : IDisposable
	{
		private IServiceProvider _serviceProvider { get; }
		private System.Timers.Timer _timer { get; set; }

		public IntervalRunner(
            IServiceProvider serviceProvider,
            IOptions<Config> options
        )
        {
            _serviceProvider = serviceProvider;
            SetupTimer(options.Value);
        }

        private void SetupTimer(Config config)
        {
            var newInterval = config.Interval.TotalMilliseconds > 0
                ? config.Interval.TotalMilliseconds
                : TimeSpan.FromSeconds(5).TotalMilliseconds;

            if (_timer?.Interval != newInterval)
            {
                _timer?.Dispose();

                _timer = new System.Timers.Timer(newInterval)
                {
                    AutoReset = true
                };

                _timer.Elapsed += async (sender, eventArgs) =>
                {
                    await ExecuteIntervalAction();
                };

                _timer.Start();
            }
        }

        public void Start() { _timer.Start(); }
        public void Stop() { _timer.Stop(); }

        private async Task ExecuteIntervalAction()
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var config = scope.ServiceProvider.GetService<IOptionsSnapshot<Config>>();
                    SetupTimer(config.Value);

                    _timer.Stop();

                    var runnable = scope.ServiceProvider.GetService<IRunnable>();
                    await runnable.Run();
                }
            }
            catch
            {
                //log something here
            }
            finally
            {
                _timer.Start();
            }
        }
        public void Dispose()
		{
            _timer?.Dispose();
        }
    }
}
