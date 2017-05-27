using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockingQueueApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Trace.Listeners.Add(new ConsoleTraceListener());

            if (Debugger.IsAttached)
            {
                Console.WriteLine("Press any key to exit..");
                Console.ReadKey();
            }
        }
    }
}
