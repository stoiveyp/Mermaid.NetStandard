using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;

namespace Mermaid.NetStandard.SequenceDiagrams
{
    public class Box
    {
        public Color Color { get; set; } = Color.Transparent;
        public string? Label { get; set; }
        public List<Participant> Participants { get; set; } = new();


        private static Regex RgbFunction = new (@"^\((\s?(?<ref>\d{1,3})\s?,?){3}\)", RegexOptions.Compiled);

        public static bool Parse(SequenceContext context)
        {
            var box = new Box();
            context.Diagram.Boxes.Add(box);
            context.CurrentBox = box;
            var colorWord = context.Parser.NextWord();

            if(colorWord == null)
            {
                return true;
            }

            if (colorWord.Equals("rgb", StringComparison.InvariantCultureIgnoreCase))
            {
                var match = RgbFunction.Match(context.Parser.RestOfLine());
                if (match.Success)
                {
                    var pieces = match.Groups["ref"].Captures;
                    var r = int.Parse(pieces[0].Value);
                    var g = int.Parse(pieces[1].Value);
                    var b = int.Parse(pieces[2].Value);
                    box.Color = System.Drawing.Color.FromArgb(r, g, b);
                    context.Parser.MoveForward(match.Length);
                    box.Label = context.Parser.RestOfLine();
                    return true;
                }
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
