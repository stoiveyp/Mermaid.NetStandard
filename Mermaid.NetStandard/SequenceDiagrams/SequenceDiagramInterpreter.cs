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

    private static async Task Interpret(SequenceContext context)
    {
        if (!await context.Parser.NextInterpeterLine())
        {
            return;
        }

        var actorPosR = context.Parser.NextWord();
        if (!actorPosR.HasValue)
        {
            throw new InvalidDiagramException("No actor found", context.Parser.LineNumber);
        }
        var actor = context.Parser.CurrentLine[actorPosR.Value];

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
        var participantNext = context.Parser.NextWord();
        if (!participantNext.HasValue)
        {
            throw new InvalidDiagramException("Name expected after 'participant'", context.Parser.LineNumber);
        }

        var name = context.Parser.CurrentLine[participantNext.Value];
        context.Diagram.Participants.Add(name, name);
    }
}