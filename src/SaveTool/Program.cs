using System;
using System.Diagnostics;
using System.IO;
using Cirilla.Core.Crypto.BlowFishCS;
using Cirilla.Core.Helpers;
using Cirilla.Core.Models;
using CommandLine;

namespace SaveTool
{
    class Program
    {
        static int Main(string[] args)
        {
            int res = Parser.Default.ParseArguments<DecryptOptions, EncryptOptions, IcebornDecryptOptions>(args)
                .MapResult(
                (DecryptOptions opts) => RunDecryptAndReturnExitCode(opts),
                (EncryptOptions opts) => RunEncryptAndReturnExitCode(opts),
                (IcebornDecryptOptions opts) => RunIcebornDecrypt(opts),
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

        private static int RunIcebornDecrypt(IcebornDecryptOptions opts)
        {
            try
            {
                // Based on Cirilla.Core.Models.SaveData

                BlowFish blowfish = new BlowFish(ExEncoding.ASCII.GetBytes("xieZjoe#P2134-3zmaghgpqoe0z8$3azeq"));
                byte[] bytes = File.ReadAllBytes(opts.InFile);
                bytes = SwapBytes(bytes);
                bytes = blowfish.Decrypt_ECB(bytes);
                bytes = SwapBytes(bytes);
                File.WriteAllBytes(opts.OutFile, bytes);

                Console.WriteLine($"Saved decrypted savedata in '{opts.OutFile}'");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return 1;
            }

            return 0;
        }

        #region Helper stuff

        private static byte[] SwapBytes(byte[] bytes)
        {
            var swapped = new byte[bytes.Length];
            for (var i = 0; i < bytes.Length; i += 4)
            {
                swapped[i] = bytes[i + 3];
                swapped[i + 1] = bytes[i + 2];
                swapped[i + 2] = bytes[i + 1];
                swapped[i + 3] = bytes[i];
            }
            return swapped;
        }

        #endregion
    }
}
