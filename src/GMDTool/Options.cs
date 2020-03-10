using CommandLine;

namespace GMDTool
{
    [Verb("print")]
    class PrintOptions
    {
        [Option('f', "file", Required = true)]
        public string Path { get; set; }

        [Option('a', "all", HelpText = "Include values without a key")]
        public bool IncludeAll { get; set; }

        [Option("json", HelpText = "Output JSON")]
        public bool OutputJson { get; set; }
    }
}
