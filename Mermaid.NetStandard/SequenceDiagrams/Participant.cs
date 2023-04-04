using System;
using System.Collections.Generic;
using System.Text;

namespace Mermaid.NetStandard.SequenceDiagrams
{
    public class Participant
    {
        public static string Next(SequenceContext context, bool asBreaks = false)
        {
            if (Message.IsStart(context.Parser.Current))
            {
                return null;
            }

            var identifier = context.Parser.NextWord();
            while (!context.Parser.EndOfLine)
            {
                var next = context.Parser.NextWord();
                if (asBreaks && next == "as")
                {
                    if (!context.Parser.EndOfLine)
                    {
                        break;
                    }
                }

                if (string.IsNullOrEmpty(next))
                {
                    return identifier;
                }

                identifier += " ";
                identifier += next;
            }

            return identifier;
        }

        public static bool Parse(SequenceContext context)
        {
            if (context.Parser.EndOfLine)
            {
                return false;
            }

            if (Message.IsStart(context.Parser.Current))
            {
                return false;
            }

            var identifier = Next(context, true);

            if (context.Parser.EndOfLine)
            {
                context.Diagram.Participants.Add(identifier, identifier);
                return true;
            }

            context.Diagram.Participants.Add(identifier, context.Parser.RestOfLine());
            return true;
        }
    }
}
