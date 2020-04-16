using System;
using System.Collections.Generic;
using System.Text;

namespace BDOTranslator.Models
{
    public struct TextLine
    {
        public uint Type { get; set; }
        public uint Addr1 { get; set; }
        public uint Addr2 { get; set; }
        public uint Addr3 { get; set; }
        public uint Addr4 { get; set; }

        public string Text { get; set; }
        public TextLine(uint type, uint a1, uint a2, uint a3, uint a4, string text)
        {
            Type = type;
            Addr1 = a1;
            Addr2 = a2;
            Addr3 = a3;
            Addr4 = a4;
            Text = text;
        }

        public new string ToString()
        {
            StringBuilder sb = new StringBuilder(Text.Length + 2);
            sb.Append("\"");
            sb.Append(Text);
            sb.Append("\"");
            return string.Join("\t", Type, Addr1, Addr2, Addr3, Addr4, sb.ToString());
        }

        public TextLine Clone()
        {
            return new TextLine(Type, Addr1, Addr2, Addr3, Addr4, Text);
        }

        public bool HasSameAddress(TextLine other)
        {
            return this.Type == other.Type && this.Addr1 == other.Addr1 && this.Addr2 == other.Addr2
                    && this.Addr3 == other.Addr3 && this.Addr4 == other.Addr4;
        }
    }
}
