using System.Diagnostics;

public static class StepRunner
{
    public static StepResult Run(string fileName, string arguments, string workingDirectory)
    {
        Stopwatch sw = Stopwatch.StartNew();

        ProcessStartInfo psi = new()
        {
            FileName = fileName,
            Arguments = arguments,
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };

        System.Text.StringBuilder output = new();

        using Process process = new() { StartInfo = psi };
        process.OutputDataReceived += (_, e) => { if (e.Data != null) output.AppendLine(e.Data); };
        process.ErrorDataReceived += (_, e) => { if (e.Data != null) output.AppendLine(e.Data); };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        process.WaitForExit();
        sw.Stop();

        return new StepResult(output.ToString(), process.ExitCode, sw.Elapsed.TotalSeconds);
    }
}

public sealed record StepResult(string Output, int ExitCode, double DurationSec);
