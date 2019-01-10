using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace parentNet
{
    class Program
    {
        static void Main(string[] args)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "child.exe",
                Arguments = "",
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            var childProcess = new Process
            {
                StartInfo = startInfo,
                EnableRaisingEvents = true
            };

            var outputMonitoringCancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            childProcess.OutputDataReceived += ProcessDataHandler;
            childProcess.ErrorDataReceived += ProcessDataHandler;


            childProcess.Start();

            childProcess.BeginOutputReadLine();
            childProcess.BeginErrorReadLine();

            var forget = Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(-1, outputMonitoringCancellationTokenSource.Token).ConfigureAwait(false);
                }
                catch (TaskCanceledException)
                {
                }

                Console.WriteLine("Cancel output read");

                childProcess.CancelErrorRead();
                childProcess.CancelOutputRead();
            });

            while (true)
            {
                while (_outputData.TryDequeue(out var line))
                {
                    Console.WriteLine(line);
                }
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
        }

        private static void ProcessDataHandler(object sender, DataReceivedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Data))
            {
                return;
            }

            _outputData.Enqueue(e.Data);

        }

        private static readonly ConcurrentQueue<string> _outputData = new ConcurrentQueue<string>();
    }
}
