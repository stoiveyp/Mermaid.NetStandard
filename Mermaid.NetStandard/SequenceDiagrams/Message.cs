using System;
using System.Collections.Generic;
using System.Text;

namespace Mermaid.NetStandard.SequenceDiagrams
{
    public class Message
    {
        private const char Char = '-';

        public string Originator { get; set; }
        public string Recipient { get; set; }
        public ArrowEnding Ending { get; set; }
        public ArrowLine Line { get; set; }

        private static Dictionary<char, ArrowEnding> Endings = new()
        {
            { '>', ArrowEnding.None },
            { 'X', ArrowEnding.Cross },
            { ')', ArrowEnding.Open }
        };

        public static bool IsStart(char candidate)
        {
            return candidate == Char;
        }

        private static bool ParseArrowEnding(SequenceContext context, Message msg)
        {
            var peek = context.Parser.Peek();

            if (!peek.HasValue)
            {
                return false;
            }

            if (peek.Value == Char)
            {
                msg.Line = ArrowLine.Dotted;
                context.Parser.Next();
                peek = context.Parser.Peek();
            }

            if (peek.HasValue && Endings.ContainsKey(peek.Value))
            {
                context.Parser.Next();
                msg.Ending = Endings[context.Parser.Current];
            }
            else
            {
                return false;
            }

            var secondPeek = context.Parser.Peek();
            if (msg.Ending == SequenceDiagrams.ArrowEnding.None && secondPeek.HasValue &&
                secondPeek.Value == '>')
            {
                context.Parser.Next();
                msg.Ending = SequenceDiagrams.ArrowEnding.Arrowhead;
            }

            return true;
        }

        public static Message Parse(SequenceContext context)
        {
            if (context.Parser.Current != Char)
            {
                return null;
            }

            var msg = new Message
            {
                Originator = context.CurrentActor
            };

            if (!ParseArrowEnding(context, msg))
            {
                return null;
            }

            context.Parser.Next();

            context.AddMessage(msg);
            return msg;
        }
    }
}
