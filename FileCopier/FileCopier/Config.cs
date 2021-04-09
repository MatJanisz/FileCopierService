using System;

namespace FileCopier
{
	public class Config : IConfig
	{
		public TimeSpan Interval { get; set; }
		public string CopiedFilePath { get; set; }
		public string FinalDirectory { get; set; }
		public string FinalExtension { get; set; }

	}
}
