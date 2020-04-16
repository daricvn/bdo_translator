using BDOTranslator.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BDOTranslator_WPF.Models
{
    public readonly struct LineTrace
    {
        public readonly TextLine Line;
        public readonly int Index;

        public LineTrace(int index, TextLine line)
        {
            Line = line.Clone();
            Index = index;
        }
    }
}
