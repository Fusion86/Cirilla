using Cirilla.Core.Helpers;
using Cirilla.Core.Models;
using Sharprompt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace ShoutoutReset
{
    class Program
    {
        // Kinda shitty but who cares.
        private static readonly string listAll = "List all shoutouts";
        private static readonly string resetSelected = "Reset the selected shoutouts";
        private static readonly string resetAll = "Reset all shoutouts";
        private static readonly string saveAndExit = "Save and Exit";
        private static readonly string[] actions = new[] { listAll, resetSelected, resetAll, saveAndExit };

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("No savedata given!");
                Console.WriteLine("USAGE: ShoutoutReset.exe C:/path/to/SAVEDATA1000");
                Console.WriteLine("Or drop the SAVEDATA1000 file on the ShoutoutReset.exe executable.");
            }
            else
            {
                InteractiveMenu(args[0]);
            }

            if (ConsoleWillBeDestroyedAtTheEnd())
            {
                Console.WriteLine("Press any key to close this window...");
                Console.ReadKey();
            }
        }

        private static void InteractiveMenu(string savepath)
        {
            Console.WriteLine("ShoutoutReset v" + Assembly.GetExecutingAssembly().GetName().Version);

            Console.Write("Loading save... ");
            SaveData savedata = new(savepath);
            Console.WriteLine("OK\n");

            Console.WriteLine("Use the up/down arrows to make a selection, and press enter to confirm.");

            var characterSelectOptions = savedata.SaveSlots.Select(x => $"{x.HunterName} (HR {x.HunterRank})").ToList();
            var selectedCharacter = Prompt.Select("Select a character", characterSelectOptions);

            var charIdx = characterSelectOptions.IndexOf(selectedCharacter);
            if (charIdx == -1)
            {
                Console.WriteLine("Invalid character selected!");
                return;
            }

            var saveslot = savedata.SaveSlots[charIdx];

            while (true)
            {
                Console.WriteLine();
                var action = Prompt.Select("What do you want to do?", actions);

                if (action == listAll)
                {
                    Console.WriteLine("Shoutouts:");
                    for (int i = 0; i < saveslot.Native.Shoutouts.Length; i++)
                    {
                        Console.WriteLine(ShoutoutToString(saveslot, i));
                    }
                }
                else if (action == resetSelected)
                {
                    var options = new List<string>();

                    for (int i = 0; i < saveslot.Native.Shoutouts.Length; i++)
                    {
                        options.Add(ShoutoutToString(saveslot, i));
                    }

                    var choices = Prompt.MultiSelect("Select which shoutouts you want to select.", options, pageSize: 24);

                    foreach (var choice in choices)
                    {
                        int idx = options.IndexOf(choice);
                        if (idx == -1)
                        {
                            Console.WriteLine("Invalid options: " + choice);
                        }
                        else
                        {
                            ResetShoutout(saveslot, idx);
                        }
                    }
                }
                else if (action == resetAll)
                {
                    bool confirm = Prompt.Confirm("This will rest all shoutouts to their default value. Continue?");
                    if (!confirm) return;

                    for (int i = 0; i < saveslot.Native.Shoutouts.Length; i++)
                    {
                        ResetShoutout(saveslot, i);
                    }

                    Console.WriteLine("Done!");
                    Console.WriteLine("Make sure to choose 'Save and Exit' in the menu before closing this program.");
                }
                else if (action == saveAndExit)
                {
                    bool confirm = Prompt.Confirm("This will overwrite your original savedata. It is your responsibility to create a backup. Continue?");
                    if (confirm)
                    {
                        savedata.Save(savepath);
                        return;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid choice!");
                }
            }
        }

        private static string ShoutoutToString(SaveSlot saveslot, int index)
        {
            var defaultValue = MhwDefaultValues.GetDefaultShoutoutValue(saveslot.SaveSlotIndex);
            string str;
            if (saveslot.Native.Shoutouts[index].Value.SequenceEqual(defaultValue))
                str = "(default value)";
            else
                str = Encoding.UTF8.GetString(saveslot.Native.Shoutouts[index].Value);
            return $"{index + 1} - {str}";
        }

        private static void ResetShoutout(SaveSlot saveslot, int index)
        {
            var defaultValue = MhwDefaultValues.GetDefaultShoutoutValue(saveslot.SaveSlotIndex);
            saveslot.Native.Shoutouts[index].Length = 0; // Might not be needed.
            saveslot.Native.Shoutouts[index].Value = defaultValue;
        }

        private static bool ConsoleWillBeDestroyedAtTheEnd()
        {
            var processList = new uint[1];
            return GetConsoleProcessList(processList, 1) == 1;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern uint GetConsoleProcessList(uint[] processList, uint processCount);
    }
}
