using System.Diagnostics;

namespace Top
{
    internal class Program
    {
        const string cFormat = "{0,-8} {1,-40} {2,-20} {3,-10}";

        static void Main(string[] args)
        {
            Process[] processlist = Process.GetProcesses();

            Console.Clear();
            Console.WriteLine(cFormat, "ID", "Name", "Time", "Memory");
            foreach (Process p in processlist)
            {
                Console.WriteLine(cFormat, p.Id.ToString(), p.ProcessName, Safe(() => p.TotalProcessorTime).ToString(), p.NonpagedSystemMemorySize64.ToString());
            }
        }

        private static T Safe<T>(Func<T> property)
        {
            try
            {
                return property();
            }
            catch
            {
                return default(T);
            }
        }
    }
}
