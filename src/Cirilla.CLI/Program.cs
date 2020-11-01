using Cirilla.Core.Models;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Threading.Tasks;

namespace Cirilla.CLI
{
    class Program
    {
        //
        // MASSIVE WARNING
        // The arguments in `CommandHandler.Create` **HAVE** to match the Argument/Options given in `new Command() { ... }`
        //

        static async Task<int> Main(string[] args)
        {
            var rootCommand = new RootCommand("Cirilla Command Line Interface");

            // SaveData
            var savedataCommand = new Command("savedata");
            rootCommand.AddCommand(savedataCommand);

            // SaveData / Info
            var savedataInfoCommand = new Command("info")
            {
                new Argument<string>("savepath", "Path to the savedata file."),
            };

            savedataInfoCommand.Handler = CommandHandler.Create<string>((path) =>
            {
                var savedata = new SaveData(path);
                // TODO:
            });

            savedataCommand.AddCommand(savedataInfoCommand);

            // SaveData / Decrypt
            var savedataDecryptCommand = new Command("decrypt")
            {
                new Argument<FileInfo>("savepath", "Path to the savedata file."),
                new Option<string?>(new [] {"--output", "-o"}, "Where to save the decrypted savedata.")
            };

            savedataDecryptCommand.Handler = CommandHandler.Create<FileInfo, string?>((savepath, output) =>
            {
                output ??= savepath.Name + ".dec";

                var savedata = new SaveData(savepath.FullName);
                savedata.Save(output, false);
                Console.WriteLine("Saved decrypted savedata to " + savepath);
            });

            savedataCommand.AddCommand(savedataDecryptCommand);

            return await rootCommand.InvokeAsync(args);
        }
    }
}
