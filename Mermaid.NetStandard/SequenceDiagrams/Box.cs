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

            if (Enum.TryParse(typeof(KnownColor), colorWord, true, out object validWord))
            {
                box.Color = System.Drawing.Color.FromKnownColor((KnownColor)validWord);
                box.Label = context.Parser.RestOfLine();
            }
            else{
                box.Label = colorWord + " " + context.Parser.RestOfLine();
            }

            return true;
        }
    }
}
