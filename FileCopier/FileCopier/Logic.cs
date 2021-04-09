using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FileCopier
{
	public class Logic : IRunnable
	{
		private Config _config { get; }

        public Logic(
            IOptionsSnapshot<Config> options
        )
        {
            _config = options.Value;
        }

        public async Task Run()
		{
            try
			{
                await Task.Run(() =>
                {
                    if (File.Exists(_config.CopiedFilePath))
                    {
                        File.Copy(_config.CopiedFilePath, 
                            Path.Combine(_config.FinalDirectory, DateTime.Now.ToString("yyyy-MM-dd--HH-mm-ss") + Path.GetFileName(_config.CopiedFilePath))
                        );
                    }
                });
            }
            catch
			{

			}
		}

    }
}
