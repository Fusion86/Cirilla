using System;
using System.Diagnostics;
using Cirilla.Core.Models;
using CommandLine;

namespace GMDTool
{
    class Program
    {
        static int Main(string[] args)
        {
            int res = Parser.Default.ParseArguments<PrintOptions>(args)
                .MapResult(
                    (PrintOptions opts) => RunPrintAndReturnExitCode(opts),
                errs => 1);

            if (Debugger.IsAttached)
                Console.ReadLine();

            return res;
        }

        private static int RunPrintAndReturnExitCode(PrintOptions opts)
        {
            GMD gmd;

            Console.WriteLine("File: " + opts.Path);

            try
            {
                gmd = new GMD(opts.Path);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return 1;
            }

            foreach (var entry in gmd.Entries)
            {
                string key = "\"\"";
                string value = '"' + entry.Value + '"';

                if (entry as GMD_Entry is var x && x != null)
                    key = '"' + x.Key + '"';
                else if (opts.IncludeAll == false)
                    continue; // If entry has no key and IncludeAll is false then don't print it

                Console.WriteLine(key + " = " + value);
            }

            return 0;
        }
    }
}
