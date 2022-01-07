using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Cirilla.Core.Models;
using CommandLine;

namespace GMDTool
{
    class Program
    {
        public record UpdatedEntry(string Key, string OldValue, string NewValue);

        static int Main(string[] args)
        {
            int res = Parser.Default.ParseArguments<PrintOptions, SearchOptions, ReplaceOptions>(args)
                .MapResult(
                    (PrintOptions opts) => Print(opts),
                    (SearchOptions opts) => Search(opts),
                    (ReplaceOptions opts) => Replace(opts),
                    errs => 1);

            if (Debugger.IsAttached)
            {
                Console.WriteLine("Debugger detected. Press enter to exit...");
                Console.ReadLine();
            }

            return res;
        }

        private static void PrintEntry(IGMD_Entry entry)
        {
            string key = "(NO_KEY)";

            if (entry as GMD_Entry is var x && x != null)
                key = x.Key;

            Colorful.Console.Write(key, Color.Cyan);
            Colorful.Console.Write(":");
            Colorful.Console.WriteLine(entry.Value, Color.Orange);
        }

        private static int Print(PrintOptions opts)
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
                PrintEntry(entry);
            }

            return 0;
        }

        private static int Search(SearchOptions opts)
        {
            string[] files = Directory.GetFiles(opts.Input, "*.gmd", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                var matches = new List<IGMD_Entry>();

                try
                {
                    var gmd = new GMD(file);

                    foreach (var entry in gmd.Entries)
                    {
                        if (opts.Search.Any(x => Regex.IsMatch(entry.Value, x)))
                            matches.Add(entry);
                    }

                    if (matches.Count > 0)
                    {
                        Colorful.Console.WriteLine(file, Color.Green);
                        foreach (var entry in matches)
                            PrintEntry(entry);
                        Console.WriteLine();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex);
                }
            }

            return 0;
        }

        private static int Replace(ReplaceOptions opts)
        {

            string[] files = Directory.GetFiles(opts.Input, "*.gmd", SearchOption.AllDirectories);

            int itemCount = files.Length;
            int processedCount = 0;
            int changedFilesCount = 0;
            int changedValuesCount = 0;
            var sw = Stopwatch.StartNew();

            Console.WriteLine("Total files: " + itemCount);
            Console.WriteLine("Preview mode: " + opts.Preview);

            foreach (var file in files)
            {
                try
                {
                    var changes = new List<UpdatedEntry>();
                    var gmd = new GMD(file);

                    foreach (var entry in gmd.Entries)
                    {
                        string val = Regex.Replace(entry.Value, opts.Search, opts.ReplaceWith);
                        if (entry.Value != val)
                        {
                            string key = "(NO_KEY)";

                            if (entry as GMD_Entry is var x && x != null)
                                key = x.Key;

                            changes.Add(new UpdatedEntry(key, entry.Value, val));
                            entry.Value = val;
                        }
                    }

                    if (changes.Count > 0)
                    {
                        string relPath = Path.GetRelativePath(opts.Input, file);
                        string dest = Path.Join(opts.Output, relPath);

                        Colorful.Console.Write(file, Color.Red);
                        Console.Write(" -> ");
                        Colorful.Console.WriteLine(dest, Color.LightGreen);
                        Colorful.Console.WriteLine("Changes: " + changes.Count, Color.Yellow);

                        foreach (var change in changes)
                        {
                            Colorful.Console.WriteLine(change.Key, Color.Cyan);
                            Colorful.Console.Write(change.OldValue, Color.Orange);
                            Console.Write(" -> ");
                            Colorful.Console.WriteLine(change.NewValue, Color.Green);
                            Console.WriteLine();
                        }

                        if (!opts.Preview)
                        {
                            string? dir = Path.GetDirectoryName(dest);
                            if (dir != null) Directory.CreateDirectory(dir);
                            gmd.Save(dest);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error {ex} in '{file}'");
                }

                Interlocked.Increment(ref processedCount);
            }

            sw.Stop();

            Console.WriteLine();
            Console.WriteLine("Finished!");
            Console.WriteLine("Processed: " + processedCount);
            Console.WriteLine("Changed files: " + changedFilesCount);
            Console.WriteLine("Changed values: " + changedValuesCount);
            Console.WriteLine("Elapsed: " + sw.Elapsed);

            return 0;
        }
    }
}
