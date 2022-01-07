using CommandLine;
using System.Collections.Generic;

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

    [Verb("search")]
    class SearchOptions
    {
        [Option('i', "in", Required = true, HelpText = "Folder containing source files")]
        public string Input { get; set; }

        [Option('s', "search", Required = true, HelpText = "What string to search for")]
        public IEnumerable<string> Search { get; set; }
    }

    [Verb("replace")]
    class ReplaceOptions
    {
        [Option('i', "in", Required = true, HelpText = "Folder containing source files")]
        public string Input { get; set; }

        [Option('o', "output", Required = true, HelpText = "Where to place the patches files")]
        public string Output { get; set; }

        [Option('s', "search", Required = true, HelpText = "What string to search for")]
        public string Search { get; set; }

        [Option('r', "replace", Required = true, HelpText = "Replace found string with this string")]
        public string ReplaceWith { get; set; }

        [Option('p', "preview", HelpText = "Preview changes, but don't apply them")]
        public bool Preview { get; set; }
    }
}
