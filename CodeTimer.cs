using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Rezun.Timer
{
    /// <summary>
    /// Provides code execution time logging.
    /// </summary>
    public struct CodeTimer : IDisposable
    {
        /// <summary>
        /// Globally enables or disables all CodeTimers. (Default: true)
        /// </summary>
        public static bool Enabled { get; set; } = true;

        /// <summary>
        /// Global default log target. (Default: Console.WriteLine)
        /// </summary>
        public static Action<string> LogAction { get; set; } = Console.WriteLine;

        /// <summary>
        /// Global default whether the short time formats with units seconds or smaller should be used. (Default: true)
        /// </summary>
        public static bool UseShortTimeFormat { get; set; } = true;

        private readonly CodeTimerInternal codeTimer;

        /// <summary>
        /// Creates an instance with values.
        /// </summary>
        /// <param name="name">The name of the timer.</param>
        /// <param name="logAction">The action to call to output the time logs. (Default: value of <see cref="LogAction"/>)</param>
        /// <param name="useShortTimeFormat">When true, only the small units seconds, milliseconds and microseconds will be output.
        /// (Default: value of <see cref="UseShortTimeFormat"/>)</param>
        /// <param name="startImmediately">When true, immediately starts this timer. (Default: true)</param>
        /// <param name="enabled">Overrides the global static <see cref="Enabled"/> value to disable or enable this timer</param>
        public static CodeTimer Create(
            [CallerMemberName] string name = null,
            Action<string> logAction = null,
            bool? useShortTimeFormat = null,
            bool startImmediately = true,
            bool? enabled = null)
        {
            bool enabledLocal = (enabled != null && enabled.Value) || (enabled == null && Enabled);
            var instance =
                enabledLocal
                ? new CodeTimer(new CodeTimerInternal(
                    name ?? string.Empty,
                    logAction ?? LogAction ?? throw new InvalidOperationException("Missing log action"),
                    useShortTimeFormat ?? UseShortTimeFormat,
                    startImmediately))
                : new CodeTimer();

            return instance;
        }

        private CodeTimer(CodeTimerInternal codeTimer) => this.codeTimer = codeTimer;

        /// <summary>
        /// Starts this timer if it hasn't been started at creation.
        /// </summary>
        public void Start() => codeTimer?.Start();

        /// <summary>
        /// Outputs a message with the time since the last call to LogStep or the timer has been started.
        /// </summary>
        /// <param name="stepName">The name of this step to identify it in the log.
        /// If null, outputs a step number that is increased with each call to this function.</param>
        public void LogStep(string stepName = null) => codeTimer?.LogStep(stepName);

        /// <summary>
        /// Disposes this instance and outputs the total time since this timer was started.
        /// </summary>
        public void Dispose() => codeTimer?.Dispose();
    }
}

