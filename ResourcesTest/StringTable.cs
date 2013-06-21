using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResourcesTest
{
    static class StringTable
    {
        public const int StringsPerGroup = 16;


        public static Dictionary<ushort, byte[]> GetGroups(IEnumerable<string> texts)
        {
            if (texts == null)
                throw new ArgumentNullException("texts");

            var result = new Dictionary<ushort, byte[]>();
            var strings = texts.ToArray();

            var groupCount = strings.Length / StringsPerGroup + 1;

            for (var g = 0; g < groupCount; g++)
            {
                var textsGroup = strings.Select(s => s ?? string.Empty).
                    Skip(StringsPerGroup * g).Take(StringsPerGroup).ToList();
                textsGroup.AddRange(Enumerable.Repeat(string.Empty, StringsPerGroup - textsGroup.Count));

                var groupBytesCount = StringsPerGroup * sizeof(ushort) +
                    textsGroup.Sum(s => s.Length) * sizeof(char);
                var groupBytes = new byte[groupBytesCount];

                for (int s = 0, offset = 0; s < StringsPerGroup; s++)
                {
                    var text = textsGroup[s];
                    BitConverter.GetBytes((ushort)text.Length).CopyTo(groupBytes, offset);
                    Encoding.Unicode.GetBytes(text).CopyTo(groupBytes, offset + sizeof(ushort));
                    offset += sizeof(ushort) + text.Length * sizeof(char);
                }

                result.Add((ushort)(g + 1), groupBytes);
            }

            return result;
        }
    }
}
