using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Mermaid.NetStandard.SequenceDiagrams
{
    public class Box:MessageContainer
    {
        public Color Color { get; set; }
        public string Label { get; set; }

        public static bool Parse(SequenceContext context)
        {
            var box = new Box();
            context.Containers.Push(box);
            return true;

            if (context.Parser.EndOfLine)
            {
                return false;
            }

            return false;
        }
    }
}
