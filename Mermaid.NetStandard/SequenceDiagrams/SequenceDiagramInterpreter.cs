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

    private static int GetActorPos(SequenceContext context)
    {
        for (var p = 0; p <= context.Parser.CurrentLine.Length - 1; p++)
        {
            if (char.IsLetterOrDigit(context.Parser.CurrentLine[p])) continue;
            return p;
        }

        return context.Parser.CurrentLine.Length;
    }

    private static async Task Interpret(SequenceContext context)
    {
        if (!await context.Parser.NextInterpeterLine())
        {
            return;
        }

        var actorPos = GetActorPos(context);
        if (actorPos == 0)
        {
            throw new InvalidDiagramException("No actor found", context.Parser.LineNumber);
        }
        var actor = context.Parser.CurrentLine[..actorPos];

        if (actor.Equals("autonumber", StringComparison.InvariantCultureIgnoreCase))
        {
            context.Diagram.AutoNumber = true;
        }
    }
}