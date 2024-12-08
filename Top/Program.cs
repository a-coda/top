using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Top
{
    internal class Program
    {
        private const string cFormat = "{0,-8} {1,-40} {2,-20} {3,-10}";

        static void Main(string[] args)
        {
            IObservable<long> whenTimerGoesOff = Observable.Interval(TimeSpan.FromSeconds(5))
                .StartWith(0);
            ISubject<char> whenAnyKeyPressed = new Subject<char>();
            whenTimerGoesOff
                .CombineLatest(whenAnyKeyPressed)
                .Subscribe(Update);
            whenAnyKeyPressed
                .OnNext(' ');
            ListenForKeyPresses(whenAnyKeyPressed);
        }

        private static void ListenForKeyPresses(ISubject<char> whenAnyKeyPressed)
        {
            ConsoleKeyInfo key;
            while (true)
            {
                key = Console.ReadKey();
                whenAnyKeyPressed.OnNext(key.KeyChar);
            }
        }

        private static void Update((long, char) eventInfo)
        {
            Process.GetProcesses()
                .OrderByDescending(ProcessInfoKey(eventInfo.Item2))
                .Take(20)
                .Select(ConvertToProcessInfo)
                .Prepend(ProcessInfo.HEADER)
                .ToList()
                .ForEach(DisplayInfo);
        }

        private static Func<Process, object> ProcessInfoKey(char indexSelection)
        {
            return indexSelection switch
            {
                '1' => p => p.Id,
                '2' => p => p.ProcessName,
                '3' => p => Safe(() => p.TotalProcessorTime, TimeSpan.Zero),
                '4' => p => p.NonpagedSystemMemorySize64,
                _ => p => p.Id,
            };
        }


        private static ProcessInfo ConvertToProcessInfo(Process p)
        {
            return new ProcessInfo(
                    p.Id.ToString(),
                    p.ProcessName,
                    Safe(() => p.TotalProcessorTime, TimeSpan.Zero).ToString(),
                    p.NonpagedSystemMemorySize64.ToString()
                    );
        }

        private static void DisplayInfo (ProcessInfo p)
        {
            if (p.Equals(ProcessInfo.HEADER))
            {
                Console.Clear();
            }
            Console.WriteLine(cFormat, p.Id, p.Name, p.TotalProcessorTime, p.NonpagedSystemMemorySize64);
        }

        private static T Safe<T>(Func<T> func, T theDefault)
        {
            try
            {
                return func();
            }
            catch
            {
                return theDefault;
            }
        }

        private record ProcessInfo(string Id, string Name, string TotalProcessorTime, string NonpagedSystemMemorySize64)
        {
            public static readonly ProcessInfo HEADER = new("ID", "Name", "Time (HH:MM:SS)", "Time (HH:MM:SS)");

        };
    }
}
