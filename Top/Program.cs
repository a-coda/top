﻿using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Top
{
    internal class Program
    {
        private const string cFormat = "{0,-8} {1,-40} {2,-20} {3,-10}";
        private static readonly ProcessInfo HEADER = new ProcessInfo("", "", "", "");

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

        private static void Update((long, char) _)
        {
            Process.GetProcesses()
                .OrderByDescending(p => Safe(() => p.TotalProcessorTime))
                .Take(20)
                .Select(ConvertToProcessInfo)
                .Prepend(HEADER)
                .ToList()
                .ForEach(DisplayInfo);
        }

        private static ProcessInfo ConvertToProcessInfo(Process p)
        {
            return new ProcessInfo(
                    p.Id.ToString(),
                    p.ProcessName,
                    Safe(() => p.TotalProcessorTime).ToString(),
                    p.NonpagedSystemMemorySize64.ToString()
                    );
        }

        private static void DisplayInfo (ProcessInfo p)
        {
            if (p.Equals(HEADER))
            {
                Console.Clear();
                Console.WriteLine(cFormat, "ID", "Name", "Time (HH:MM:SS)", "Memory (bytes)");
            }
            else
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
