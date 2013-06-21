using System;
using System.Linq;
using System.Text;

namespace ResourcesTest
{
    abstract class OperationArgs
    {
        public abstract string Key { get; }
        public abstract string Options { get; }
        public abstract string Description { get; }


        protected abstract bool InnerParse(string[] args);

        public bool Parse(string[] args)
        {
            return args != null && args.Length > 0 && 
                args[0] != null && args[0].Contains(Key) && 
                InnerParse(args.Skip(1).Where(a => a != null).ToArray());
        }
    }


    sealed class InsertArgs : OperationArgs
    {
        private const string key = "i";
        public override string Key
        {
            get { return key; }
        }

        private const string options = "[string1] [string2] ...";
        public override string Options
        {
            get { return options; }
        }

        private const string description = "Inserts [string] resource into executable";
        public override string Description
        {
            get { return description; }
        }

        public string[] Strings { get; set; }


        protected override bool InnerParse(string[] args)
        {
            Strings = args;
            return true;
        }

        public override string ToString()
        {
            return string.Format("\"{0}\" \"{1}\"",
                Key, string.Join("\" \"", Strings ?? new string[0]));
        }
    }


    sealed class ExtractArgs : OperationArgs
    {
        private const string key = "e";
        public override string Key
        {
            get { return key; }
        }

        private const string options = "[id1] [id2] ...";
        public override string Options
        {
            get { return options; }
        }

        private const string description = "Extracts resource with [id] from executable";
        public override string Description
        {
            get { return description; }
        }

        public uint[] Ids { get; set; }


        protected override bool InnerParse(string[] args)
        {
            Ids = args.Select(a =>
                { uint r; return uint.TryParse(a, out r) ? new uint?(r) : null; }).
                Where(ui => ui.HasValue).Select(ui => ui.Value).ToArray();
            return true;
        }

        public override string ToString()
        {
            return string.Format("\"{0}\" \"{1}\"",
                Key, string.Join("\" \"", Ids ?? new uint[0]));
        }
    }


    sealed class CleanArgs : OperationArgs
    {
        private const string key = "c";
        public override string Key
        {
            get { return key; }
        }

        private const string options = "";
        public override string Options
        {
            get { return options; }
        }

        private const string description = "Removes all resources from executable";
        public override string Description
        {
            get { return description; }
        }


        protected override bool InnerParse(string[] args)
        {
            return true;
        }

        public override string ToString()
        {
            return Key;
        }
    }


    sealed class ReplaceArgs : OperationArgs
    {
        private const string key = "r";
        public override string Key
        {
            get { return key; }
        }

        private const string options = "original temp";
        public override string Options
        {
            get { return options; }
        }

        private const string description = "Replaces current executable with temporary one";
        public override string Description
        {
            get { return description; }
        }

        public string OriginalLocation { get; set; }
        public string TempLocation { get; set; }


        protected override bool InnerParse(string[] args)
        {
            if (args.Length < 2)
                return false;

            OriginalLocation = args[0];
            TempLocation = args[1];
            return true;
        }

        public override string ToString()
        {
            return string.Format("\"{0}\" \"{1}\" \"{2}\"",
                Key, OriginalLocation ?? string.Empty, TempLocation ?? string.Empty);
        }
    }


    static class ArgsParser
    {
        private static readonly OperationArgs[] args = new OperationArgs[]
        {
            new InsertArgs(), 
            new ExtractArgs(), 
            new CleanArgs(), 
            new ReplaceArgs()
        };


        public static string[] GetUsageLines()
        {
            var result = new string[args.Length];

            const string format = "/{0} {1} {2}";
            var keyLength = args.Max(a => a.Key.Length);
            var optionsLength = args.Max(a => a.Options.Length);
            var descriptionLength = args.Max(a => a.Description.Length);

            for (var i = 0; i < args.Length; i++)
                result[i] = string.Format(format,
                    args[i].Key.PadRight(keyLength),
                    args[i].Options.PadRight(optionsLength),
                    args[i].Description.PadRight(descriptionLength));

            return result;
        }

        public static string GetUsage()
        {
            var lines = GetUsageLines();

            if (lines.Length == 0)
                return string.Empty;

            var result = new StringBuilder((lines[0].Length + Environment.NewLine.Length) * lines.Length);

            foreach (var line in lines)
                result.AppendLine(line);

            return result.ToString();
        }

        public static OperationArgs Parse(string[] arguments)
        {
            return args.FirstOrDefault(a => a.Parse(arguments));
        }
    }
}
