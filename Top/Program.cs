using System.Diagnostics;

namespace Top
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Process[] processlist = Process.GetProcesses();

            Console.Clear();
            foreach (Process p in processlist)
            {
                Console.WriteLine($"Process: {p.ProcessName} ID: {p.Id}");
            }
        }
    }
}
