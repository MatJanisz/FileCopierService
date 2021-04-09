using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Topshelf;

namespace FileCopier
{
	public class WindowsService
	{
		private string _serviceName { get; }
		private IServiceCollection _services { get; }
		private IConfiguration _configuration { get; }
		private IServiceProvider _serviceProvider { get; set; }

		private WindowsService(
            string serviceName,
            IConfiguration configuration
        )
        {
            _serviceName = serviceName;
            _configuration = configuration;
            _services = new ServiceCollection();
        }

		public static WindowsService Create<TConfig, TRunnable>(
            string serviceName,
            IConfiguration configuration
        )
            where TRunnable : class, IRunnable
            where TConfig : class, IConfig
        {
            var result = new WindowsService(serviceName, configuration);

            result.ConfigureInternalServices<TConfig>();

            result._services.AddScoped<IRunnable, TRunnable>();

            return result;
        }

        public WindowsService AddScoped<T>() where T : class
        {
            _services.AddScoped<T>();
            return this;
        }

        public WindowsService AddSingleton<T>() where T : class
        {
            _services.AddSingleton<T>();
            return this;
        }

        public void Start()
        {
            _serviceProvider = _services.BuildServiceProvider();

            HostFactory.Run(windowsService =>
            {
                windowsService.Service<IntervalRunner>(s =>
                {
                    s.ConstructUsing(service => _serviceProvider.GetRequiredService<IntervalRunner>());
                    s.WhenStarted(service => service.Start());
                    s.WhenStopped(service => service.Stop());
                });

                windowsService.RunAsLocalSystem();
                windowsService.StartAutomatically();

                windowsService.SetDescription(_serviceName);
                windowsService.SetDisplayName(_serviceName);
                windowsService.SetServiceName(_serviceName);
            });
        }

        private void ConfigureInternalServices<TConfig>() where TConfig : class, IConfig
        {

            _services.AddOptions();

            _services.Configure<TConfig>(_configuration);

            _services.Configure<Config>(_configuration);

            _services.AddSingleton<IntervalRunner>();
        }

    }
}
