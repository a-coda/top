using System.Diagnostics;
using System.Reactive.Linq;

namespace Top
{
    internal class Program
    {
        private const string cFormat = "{0,-8} {1,-40} {2,-20} {3,-10}";

        static void Main(string[] args)
        {
            IObservable<long> times = Observable.Interval(TimeSpan.FromSeconds(5)).StartWith(0);
            IObservable<IEnumerable<Process>> processes = times.Select(_ => Generate());
            processes.Subscribe(processes => Display(processes));
            processes.Wait();
        }

        private static IEnumerable<Process> Generate()
        {
            return Process.GetProcesses().OrderByDescending(p => Safe(() => p.TotalProcessorTime)).Take(20);
        }

        private static void Display(IEnumerable<Process> processes)
        {
            Console.Clear();
            Console.WriteLine(cFormat, "ID", "Name", "Time (HH:MM:SS)", "Memory (bytes)");
            foreach (Process p in processes)
            {
                Console.WriteLine(cFormat, p.Id.ToString(), p.ProcessName, Safe(() => p.TotalProcessorTime).ToString(), p.NonpagedSystemMemorySize64.ToString());
            }
        }

        private static T Safe<T>(Func<T> func)
        {
            try
            {
                return func();
            }
            catch
            {
                return default(T);
            }
        }
    }
}
