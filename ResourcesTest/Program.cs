using System;
using System.Text;

namespace ResourcesTest
{
    class Program
    {
        private static void Usage()
        {
            const string header =
@"
Win32 resources manipulations test app
Usage:
";
            const string indent = "    ";

            var lines = ArgsParser.GetUsageLines();
            var usage = new StringBuilder(header.Length + (lines.Length > 0 ?
                (lines[0].Length + Environment.NewLine.Length + indent.Length) * lines.Length : 0));

            usage.Append(header);
            foreach (var line in lines)
                usage.Append(indent).AppendLine(line);

            Console.Write(usage);
        }

        private static void Main(string[] arguments)
        {
            var args = ArgsParser.Parse(arguments);
            TypeSwitch.Do(args,
                TypeSwitch.Case<InsertArgs>(Operations.Insert),
                TypeSwitch.Case<ExtractArgs>(Operations.Extract),
                TypeSwitch.Case<CleanArgs>(Operations.Clean),
                TypeSwitch.Case<ReplaceArgs>(Operations.Replace),
                TypeSwitch.Default(Usage));
        }
    }
}
