using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mermaid.NetStandard.SequenceDiagrams;

public class SequenceDiagramInterpreter
{
    public static async Task<SequenceDiagram> Interpret(MermaidParser parser)
    {
        var sequenceContext = new SequenceContext(parser);
        await Interpret(sequenceContext);
        var openContainer = sequenceContext.CurrentContainer;
        if (openContainer != null)
        {
            throw new InvalidDiagramException($"{openContainer.GetType().Name} without end");
        }
        return sequenceContext.Diagram;
    }

    private static Dictionary<string, Func<SequenceContext, bool>> commands = new(StringComparer.InvariantCultureIgnoreCase)
    {
        { "autonumber", sc =>sc.Diagram.AutoNumber = true },
        { "participant", Participant.Parse },
        { "box", Box.Parse },
        { "end", sc => sc.EndContainer() }
    };

    private static async Task Interpret(SequenceContext context)
    {
        while (await context.Parser.NextInterpeterLine())
        {
            await InterpretLine(context);
        }
    }

    private static async Task InterpretLine(SequenceContext context)
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

        if (context.Parser.EndOfLine)
        {
            context.Diagram.Participants.Add(commandOrActor, commandOrActor);
            return;
        }

        var restOfCommand = Participant.Next(context);
        if (!string.IsNullOrEmpty(restOfCommand))
        {
            commandOrActor += " ";
            commandOrActor += restOfCommand;
        }

        if (!context.Diagram.Participants.ContainsKey(commandOrActor))
        {
            context.Diagram.Participants.Add(commandOrActor, commandOrActor);
        }

        context.CurrentActor = commandOrActor;

        if (context.Parser.EndOfLine)
        {
            return;
        }

        var message = Message.Parse(context);
        if (message == null)
        {
            throw new InvalidDiagramException("Unknown message type", context.Parser.LineNumber);
        }

        var recipient = Participant.Next(context);

        if (recipient == null)
        {
            return;
        }

        if (!context.Diagram.Participants.ContainsKey(recipient))
        {
            context.Diagram.Participants.Add(recipient, recipient);
        }

        message.Recipient = recipient;
    }
}