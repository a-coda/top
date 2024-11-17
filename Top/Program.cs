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
            times
                .Select(Generate)
                .Select(CollectProcessInfo)
                .Subscribe(Display);
            times.Wait();
        }

        private static IEnumerable<Process> Generate(long _)
        {
            return Process.GetProcesses().OrderByDescending(p => Safe(() => p.TotalProcessorTime)).Take(20);
        }

        private static IEnumerable<ProcessInfo> CollectProcessInfo(IEnumerable<Process> processes)
        {
            foreach (var p in processes)
            {
                yield return new ProcessInfo(
                    p.Id.ToString(),
                    p.ProcessName,
                    Safe(() => p.TotalProcessorTime).ToString(),
                    p.NonpagedSystemMemorySize64.ToString());
            }
        }

        private static void Display(IEnumerable<ProcessInfo> processes)
        {
            List<ProcessInfo> processList = processes.ToList();
            Console.Clear();
            Console.WriteLine(cFormat, "ID", "Name", "Time (HH:MM:SS)", "Memory (bytes)");
            foreach (ProcessInfo p in processList)
            {
                Console.WriteLine(cFormat, p.Id, p.Name, p.TotalProcessorTime, p.NonpagedSystemMemorySize64);
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

        private record ProcessInfo(string Id, string Name, string TotalProcessorTime, string NonpagedSystemMemorySize64);
    }
}
