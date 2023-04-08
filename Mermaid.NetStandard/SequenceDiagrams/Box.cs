using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Mermaid.NetStandard.SequenceDiagrams
{
    public class Box:MessageContainer
    {
        public Color Color { get; set; } = Color.Transparent;
        public string? Label { get; set; }

        public static bool Parse(SequenceContext context)
        {
            var box = new Box();
            context.Containers.Push(box);

            var colorWord = context.Parser.NextWord();

            if(colorWord == null)
            {
                return true;
            }

            if(Enum.IsDefined(typeof(KnownColor), colorWord))
            {
                box.Color = Color.FromKnownColor(Enum.Parse<KnownColor>(colorWord, true));
                box.Label = context.Parser.RestOfLine();
            }
            else{
                box.Label = colorWord + " " + context.Parser.RestOfLine();
            }

            return true;
        }
    }
}
