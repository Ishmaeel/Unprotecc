using System;

namespace Unprotecc
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    Logger.Write("Huh?");
                }
                else
                {
                    Worker.Run(args);
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex.Message);
            }

            Logger.Write();
            Logger.Write("Press any key to exit.");

            Console.ReadKey();
        }
    }
}