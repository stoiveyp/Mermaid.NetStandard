using System;
using System.Collections.Generic;
using System.Text;

namespace Mermaid.NetStandard.SequenceDiagrams
{
    public class Activation:SequenceElement
    {
        public ActivationType Type { get; }
        public Participant Participant { get; set; }

        public Activation(ActivationType type, Participant participant)
        {
            Type = type;
            Participant = participant;
        }

        public static bool Parse(SequenceContext context, ActivationType type)
        {
            var key = context.Parser.RestOfLine().Trim();
            var participant = context.EnsureParticipant(key, key);
            context.AddElement(new Activation(type, participant));
            return true;
        }


        public static ActivationType? ParseShortcut(SequenceContext context)
        {
            if (context.Parser.EndOfLine)
            {
                return null;
            }

            if (context.Parser.Current == '+')
            {
                context.Parser.Next();
                return ActivationType.Activate;
            }

            if (context.Parser.Current == '-')
            {
                context.Parser.Next();
                return ActivationType.Deactivate;
            }

            return null;
        }
    }
}
