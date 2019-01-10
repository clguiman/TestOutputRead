using System;
using System.IO;
using System.Threading;

namespace child
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                var pingStr = $"{DateTime.Now} Ping";
                Console.WriteLine(pingStr);
                File.AppendAllLines("log.txt", new string[] { pingStr });
                Thread.Sleep(TimeSpan.FromSeconds(10));
            }
        }
    }
}
