using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Mermaid.NetStandard.SequenceDiagrams;

public class SequenceDiagramInterpreter
{
    public static async Task<SequenceDiagram> Interpret(MermaidParser parser)
    {
        var sequenceContext = new SequenceContext(parser);
        await Interpret(sequenceContext);
        return sequenceContext.Diagram;
    }

    private static int NextWord(SequenceContext context)
    {
        while (char.IsLetterOrDigit(context.Parser.Current))
        {
            if (!context.Parser.Next())
            {
                break;
            }
        }

        return context.Parser.CurrentPosition;
    }

    private static async Task Interpret(SequenceContext context)
    {
        if (!await context.Parser.NextInterpeterLine())
        {
            return;
        }

        var actorPos = NextWord(context);
        if (actorPos == 0)
        {
            throw new InvalidDiagramException("No actor found", context.Parser.LineNumber);
        }
        var actor = context.Parser.CurrentLine[..actorPos];

        if (actor.Equals("autonumber", StringComparison.InvariantCultureIgnoreCase))
        {
            context.Diagram.AutoNumber = true;
        }

        if (actor.Equals("participant", StringComparison.InvariantCultureIgnoreCase))
        {
            ParseParticipant(context);
        }
    }

    private static void ParseParticipant(SequenceContext context)
    {
        if (context.Parser.Current != ' ')
        {
            throw new InvalidDiagramException("Name expected after 'participant'", context.Parser.LineNumber);
        }

        context.Parser.Next();
        var currentPos = context.Parser.CurrentPosition;
        NextWord(context);

        if (context.Parser.CurrentPosition > currentPos)
        {
            var name = context.Parser.CurrentLine[currentPos..(context.Parser.CurrentPosition)];
            context.Diagram.Participants.Add(name,name);
        }
    }
}