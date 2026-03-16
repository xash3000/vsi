using System;
using System.IO;
using Vsi.Core;

namespace Vsi.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.Error.WriteLine("please provide a file");
                Console.Error.WriteLine("example: vsi filename.vsi");
                Environment.Exit(1);
            }

            string filename = args[0];
            try
            {
                string content = File.ReadAllText(filename);
                Api.Interpret(content);
            }
            catch (FileNotFoundException)
            {
                Console.Error.WriteLine($"No such file or directory: '{filename}'");
                Environment.Exit(1);
            }
        }
    }
}
