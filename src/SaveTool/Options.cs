using CommandLine;

namespace SaveTool
{
    [Verb("decrypt", HelpText = "Decrypt save.")]
    public class DecryptOptions
    {
        [Option('i', "in", HelpText = "Savefile to decrypt", Required = true)]
        public string InFile { get; set; }

        [Option('o', "out", HelpText = "Decrypted filename", Default = "SAVEDATA1000_dec")]
        public string OutFile { get; set; }
    }

    [Verb("encrypt", HelpText = "Encrypt save.")]
    public class EncryptOptions
    {
        [Option('i', "in", HelpText = "Savefile to encrypt", Required = true)]
        public string InFile { get; set; }

        [Option('o', "out", HelpText = "Encrypted filename", Default = "SAVEDATA1000_enc")]
        public string OutFile { get; set; }

        [Option("fix-checksum", HelpText = "Fix checksum (needed when data changed)", Default = true)]
        public bool FixChecksum { get; set; }
    }

    [Verb("iceborn_decrypt", HelpText = "Iceborn test stuff.")]
    public class IcebornDecryptOptions
    {
        [Option('i', "in", HelpText = "Savefile to decrypt", Required = true)]
        public string InFile { get; set; }

        [Option('o', "out", HelpText = "Decrypted filename", Default = "SAVEDATA1000_dec")]
        public string OutFile { get; set; }
    }
}
