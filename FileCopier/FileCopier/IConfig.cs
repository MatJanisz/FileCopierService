using System;

namespace FileCopier
{
	public interface IConfig
	{
		TimeSpan Interval { get; set; }
		string CopiedFilePath { get; set; }
		string FinalDirectory { get; set; }
		string FinalExtension { get; set; }

	}
}
