using Microsoft.Extensions.Configuration;
using System.IO;

namespace FileCopier
{
	class Program
	{
		public const string SERVICE_NAME = "FileCopier";
		private static IConfiguration _configuration => new ConfigurationBuilder()
			.AddJsonFile(Path.Combine("config", "config.json"), optional: false, reloadOnChange: true)
			.Build();
		static void Main(string[] args)
		{
			WindowsService
				.Create<Config, Logic>(SERVICE_NAME, _configuration)
				.Start();
		}
	}
}
