using System;

namespace Keebee.AAT.GenerateVideos
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string path = null;

            if (args.Length > 0)
            {
                path = args[0];
            }
            else
            {
                Console.WriteLine("Enter path to videos:");
                path = Console.ReadLine();
            }

            if (string.IsNullOrEmpty(path))
            {
                Console.WriteLine("Invalid path");
            }
            var engine = new Engine();
            engine.GenerateVideos(path);
        }
    }
}
