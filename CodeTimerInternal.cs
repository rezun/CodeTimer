using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Rezun.Timer
{
    internal class CodeTimerInternal : IDisposable
    {
        private readonly Stopwatch stopWatch;
        private readonly string name;
        private readonly Action<string> logAction;
        private readonly bool useShortTimeFormat;

        private TimeSpan lastStepTime;
        private int stepCounter;

        public CodeTimerInternal(
            string name,
            Action<string> logAction,
            bool useShortTimeFormat,
            bool startImmediately)
        {
            this.name = name;
            this.logAction = logAction;
            this.useShortTimeFormat = useShortTimeFormat;
            stepCounter = 1;
            stopWatch = new Stopwatch();

            if (startImmediately)
                Start();
        }

        public void Start()
        {
            if (stopWatch.IsRunning)
                return;

            stopWatch.Start();

            logAction($"Timer {name}: started");
        }

        public void LogStep(string stepName)
        {
            var currentTotalTime = stopWatch.Elapsed;
            var stepTime = currentTotalTime - lastStepTime;

            logAction($"Timer {name}, step {stepName ?? stepCounter.ToString()}: finished in {GetTime(stepTime)} (total: {GetTime(currentTotalTime)})");

            lastStepTime = currentTotalTime;
            stepCounter++;
        }

        public void Dispose()
        {
            stopWatch.Stop();
            logAction($"Timer {name}: finished in {GetTime(stopWatch.Elapsed)}{(stepCounter > 1 ? $" (last step: {GetTime(stopWatch.Elapsed - lastStepTime)})" : "")}");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private string GetTime(TimeSpan timeSpan)
            => useShortTimeFormat && timeSpan.TotalSeconds < 60 ? timeSpan.ToString(@"ss\.fffffff") : timeSpan.ToString();
    }
}
