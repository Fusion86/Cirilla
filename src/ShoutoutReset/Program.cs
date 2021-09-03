using Cirilla.Core.Models;
using Sharprompt;
using System;
using System.Linq;
using System.Text;

namespace ShoutoutReset
{
    class Program
    {
        private static byte[] defaultValue = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x10, 0xFC, 0xFA, 0x06, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xB9, 0x32, 0xAB, 0x42, 0x01, 0x00, 0x00, 0x00, 0xAD, 0x4D, 0x3C, 0x42 };

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("No savedata given!");
                Console.WriteLine("USAGE: ShoutoutReset.exe C:/path/to/SAVEDATA1000");
                Console.WriteLine("Or drop the SAVEDATA1000 file on the ShoutoutReset.exe executable.");
                return;
            }

            SaveData savedata = new SaveData(args[0]);

            var characterSelectOptions = savedata.SaveSlots.Select(x => $"{x.HunterName} (HR {x.HunterRank})").ToList();
            var selectedCharacter = Prompt.Select("Select a character", characterSelectOptions);

            var charIdx = characterSelectOptions.IndexOf(selectedCharacter);
            if (charIdx == -1)
            {
                Console.WriteLine("Invalid character selected!");
                return;
            }

            var saveslot = savedata.SaveSlots[charIdx];

            var listAll = "List all shoutouts";
            var resetSelected = "Reset the selected shoutouts";
            var resetAll = "Reset all shoutouts";
            var saveAndExit = "Save and Exit";

            var actions = new[] { listAll, resetSelected, resetAll, saveAndExit };

            while (true)
            {
                var action = Prompt.Select("What do you want to do?", actions);

                if (action == listAll)
                {
                    Console.WriteLine("Listing all shoutouts (ignore any gibberish after the string, this program is just a PoC)");
                    for (int i = 0; i < saveslot.Native.Shoutouts.Length; i++)
                    {
                        string str = null!;

                        if (saveslot.Native.Shoutouts[i].Value.SequenceEqual(defaultValue))
                        {
                            str = "(default value)";
                        }
                        else
                        {
                            str = Encoding.UTF8.GetString(saveslot.Native.Shoutouts[i].Value);
                        }

                        Console.WriteLine($"{i + 1} - {str}");
                    }
                }
                else if (action == resetAll)
                {
                    for (int i = 0; i < saveslot.Native.Shoutouts.Length; i++)
                    {
                        saveslot.Native.Shoutouts[i].Length = 0; // Might not be needed.
                        saveslot.Native.Shoutouts[i].Value = defaultValue;
                    }

                    Console.WriteLine("Reset all shoutouts to their default value. Make sure to choose 'Save and Exit' in the menu before closing this program.");
                }
                else if (action == saveAndExit)
                {
                    bool confirm = Prompt.Confirm("This will overwrite your original savedata. It is your responsibility to create a backup. Continue?");
                    if (confirm)
                    {
                        savedata.Save(args[0]);
                        return;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid choice!");
                }
            }
        }
    }
}
