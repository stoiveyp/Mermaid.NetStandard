using System;
using System.Collections.Generic;
using System.Text;

namespace Mermaid.NetStandard.SequenceDiagrams
{
    public class Participant
    {
        public string Name { get; }
        public string Alias { get; }
        public ParticipantType Type { get; }

        public Participant(string name):this(name, name)
        {
        }

        public Participant(string name, string alias, ParticipantType type = ParticipantType.Participant)
        {
            Name = name;
            Alias = alias;
            Type = type;
        }

        public static string? Next(SequenceContext context, bool asBreaks = false)
        {
            if (Message.IsStart(context.Parser.Current))
            {
                return null;
            }

            var identifier = context.Parser.NextWord();

            while (!context.Parser.EndOfLine && !Message.IsStart(context.Parser.Current))
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

        public static bool Parse(SequenceContext context, ParticipantType type)
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

            if (identifier == null)
            {
                return false;
            }

            if (context.Parser.EndOfLine)
            {
                context.EnsureParticipant(identifier, identifier, type);
                return true;
            }

            context.EnsureParticipant(identifier,context.Parser.RestOfLine(), type);
            return true;
        }
    }
}
