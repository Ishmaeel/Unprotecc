using System;

namespace Unprotecc
{
    internal class Logger
    {
        internal static void Write(string message = null)
        {
            Console.WriteLine(message);
        }
    }
}