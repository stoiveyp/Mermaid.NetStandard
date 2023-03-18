using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
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

    private static Dictionary<string, Func<SequenceContext, bool>> commands = new(StringComparer.InvariantCultureIgnoreCase)
    {
        { "autonumber", sc =>sc.Diagram.AutoNumber = true },
        { "participant", ParseParticipant }
    };

private static async Task Interpret(SequenceContext context)
{
    if (!await context.Parser.NextInterpeterLine())
    {
        return;
    }

    var actor = context.Parser.NextWord();
    if (actor == null)
    {
        throw new InvalidDiagramException("No actor found", context.Parser.LineNumber);
    }

    if (commands.ContainsKey(actor))
    {
        if (commands[actor](context)) return;
    }

    context.Diagram.Participants.Add(actor, actor);
}

private static bool ParseParticipant(SequenceContext context)
{
    if (!context.Parser.EndOfLine && context.Parser.Current != ' ')
    {
        return false;
    }

    var participantNext = context.Parser.NextWord();
    if (participantNext == null)
    {
        throw new InvalidDiagramException("Name expected after 'participant'", context.Parser.LineNumber);
    }

    if (context.Parser.EndOfLine)
    {
        context.Diagram.Participants.Add(participantNext, participantNext);
        return true;
    }

    var asWord = context.Parser.NextWord();
    if (asWord != "as")
    {
        throw new InvalidDiagramException("Expected 'as' after participant id", context.Parser.LineNumber);
    }

    var alias = context.Parser.NextWord();
    if (alias == null)
    {
        throw new InvalidDiagramException("Expected alias after 'as'", context.Parser.LineNumber);
    }

    context.Diagram.Participants.Add(participantNext, alias);
    return true;
}
}