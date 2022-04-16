# CodeTimer

Simple and easy to use timer to time code and functions. It outputs the time it took between its creation and its disposing. It has some sensible defaults that can be overriden.

Available as nuget package [Rezun.CodeTimer](https://www.nuget.org/packages/Rezun.CodeTimer/)

# Usage

The simplest way to use a timer is using it without any arguments. It will start the timer immediately and end it when it's disposed.
It will log to Console.WriteLine, using the calling function name as the name for the timer. By default, the time is logged in seconds, milliseconds, microseconds and ticks.

```csharp
static void LogMeFunction()
{
    using var timer = CodeTimer.Create();
    Thread.Sleep(100);
}
```

This will output the following to the Console:

```
Timer LogMeFunction: started
Timer LogMeFunction: finished in 00.1050198
```

## Logging intermediate steps

You can use the function `LogStep` to log the time at individual points while the timer is running. If no step name is given as an argument, the steps will be named in increasing numbers, starting with 1.

```csharp
static void ComplexFunction()
{
    using var timer = CodeTimer.Create();
    Thread.Sleep(100);
    timer.LogStep();
    Thread.Sleep(42);
    timer.LogStep("special call");
    Thread.Sleep(30);
    timer.LogStep();
    Thread.Sleep(30);
}
```

This will output:

```
Timer ComplexFunction: started
Timer ComplexFunction, step 1: finished in 00.1038428 (total: 00.1038428)
Timer ComplexFunction, step special call: finished in 00.0451807 (total: 00.1490235)
Timer ComplexFunction, step 3: finished in 00.0340042 (total: 00.1830277)
Timer ComplexFunction: finished in 00.2164105 (last step: 00.0333828)
```

## The time format

Assuming that most timed operations take less than one minute, the default is to only output seconds and smaller units. This can be globally changed with the static `CodeTimer.UseShortTimeFormat` property.

Even if the short time format is used, if an operation takes longer then 60 seconds, the full time will be output.

## All timer arguments

```csharp
static void FullTimerArguments()
{
    using(var timer = CodeTimer.Create(
        // The name of the timer
        name: "my timer",
        // The logging target
        logAction: p => logger.Trace(p),
        // Use the full TimeSpan format
        useShortTimeFormat: false,
        // Don't start the timer yet.
        startImmediately: false,
        // Enabled, even if CodeTimer.Enabled is false
        enabled: true));

    {
        // Code that shouldn't be logged
        // ...

        timer.Start();
        Thread.Sleep(100);
        timer.LogStep(stepName: "special call");
        Thread.Sleep(42);
    }

    // ...
}
```

Output:

```
Timer my timer: started
Timer my timer, step special call: finished in 00:00:00.1050205 (total: 00:00:00.1050205)
Timer my timer: finished in 00:00:00.1521179 (last step: 00:00:00.0470974)
```

## All static global options

```csharp
// Globally enables or disables all CodeTimers.
public static bool Enabled { get; set; } = true;

// Global default log target.
public static Action<string> LogAction { get; set; } = Console.WriteLine;

// Global default whether the short time formats with units seconds or smaller should be used.
public static bool UseShortTimeFormat { get; set; } = true;
```

## Enablig / Disabling timers

All timers can be globally disabled with the static property `CodeTimer.Enabled`. This is useful in combination with an `#if DEBUG` directive to only enable them in debug mode. Or it can be set depending on a log level.

Each individual timer also has an optinal bool argument `enabled` that, when being set to a no-null value, will override the global behavior in specific cases.

**If a timer is disabled, all calls to it will be no-ops and it won't allocate any memory on the heap. This allows the timing code to stay in without the need to comment it out for production. Fot this reason `CodeTimer` is a struct that only holds a reference to an internal CodeTimerInternal, not a class.**

# Remarks

CodeTimer is a stuct but its default constructor should not be used. Always use the `CodeTimer.Create` function or it won't do anything.

# License

This project is licensed under the 3 clause BSD License - see the License.md file for details
