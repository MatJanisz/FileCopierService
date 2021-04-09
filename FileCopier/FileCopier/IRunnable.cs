using System.Threading.Tasks;

namespace FileCopier
{
	public interface IRunnable
	{
		Task Run();
	}
}
