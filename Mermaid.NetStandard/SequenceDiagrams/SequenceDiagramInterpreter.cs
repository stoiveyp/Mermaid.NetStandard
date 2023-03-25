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

        while (!context.Parser.EndOfLine)
        {
            var commandOrActor = context.Parser.NextWord();
            if (commandOrActor == null)
            {
                throw new InvalidDiagramException("No actor found", context.Parser.LineNumber);
            }

            if (commands.ContainsKey(commandOrActor))
            {
                if (commands[commandOrActor](context)) return;
            }

            context.Diagram.Participants.Add(commandOrActor, commandOrActor);
            context.CurrentActor = commandOrActor;

            return;
        }
    }

    private static bool ParseParticipant(SequenceContext context)
    {
        if (context.Parser.EndOfLine)
        {
            return false;
        }

        var participantNext = context.Parser.NextWord();
        if (participantNext == null)
        {
            return false;
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