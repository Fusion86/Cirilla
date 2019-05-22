using System;
using System.Diagnostics;
using Cirilla.Core.Models;
using CommandLine;

namespace SaveTool
{
    class Program
    {
        static int Main(string[] args)
        {
            int res = Parser.Default.ParseArguments<DecryptOptions, EncryptOptions>(args)
                .MapResult(
                (DecryptOptions opts) => RunDecryptAndReturnExitCode(opts),
                (EncryptOptions opts) => RunEncryptAndReturnExitCode(opts),
                errs => 1);

            if (Debugger.IsAttached)
            {
                Console.WriteLine("Press enter to exit...");
                Console.ReadLine();
            }

            return res;
        }

        private static int RunDecryptAndReturnExitCode(DecryptOptions opts)
        {
            try
            {
                SaveData saveData = new SaveData(opts.InFile);
                saveData.Save(opts.OutFile, false);
                Console.WriteLine($"Saved decrypted savedata in '{opts.OutFile}'");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return 1;
            }

            return 0;
        }

        private static int RunEncryptAndReturnExitCode(EncryptOptions opts)
        {
            try
            {
                SaveData saveData = new SaveData(opts.InFile);
                saveData.Save(opts.OutFile, true, opts.FixChecksum);
                Console.WriteLine($"Saved encrypted savedata in '{opts.OutFile}'");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return 1;
            }

            return 0;
        }
    }
}
