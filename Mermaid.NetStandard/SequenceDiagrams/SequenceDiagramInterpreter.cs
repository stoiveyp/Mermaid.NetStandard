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

    private static readonly Dictionary<string, Func<SequenceContext, bool>> Commands = new(StringComparer.InvariantCultureIgnoreCase)
    {
        { "autonumber", sc =>sc.Diagram.AutoNumber = true },
        { "participant", sc => Participant.Parse(sc,ParticipantType.Participant) },
        { "actor", sc => Participant.Parse(sc, ParticipantType.Actor) },
        { "box", Box.Parse },
        { "end", sc => sc.EndContainer() }
    };

    private static async Task Interpret(SequenceContext context)
    {
        while (await context.Parser.NextInterpeterLine())
        {
            InterpretLine(context);
        }
    }

    private static void InterpretLine(SequenceContext context)
    {
        var commandOrActor = context.Parser.NextWord() ?? throw new InvalidDiagramException("No actor found", context.Parser.LineNumber);
        if (Commands.ContainsKey(commandOrActor))
        {
            if (Commands[commandOrActor](context)) return;
        }

        if (context.Parser.EndOfLine)
        {
            context.Diagram.Participants.Add(commandOrActor, new Participant(commandOrActor));
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
            context.Diagram.Participants.Add(commandOrActor, new Participant(commandOrActor));
        }

        context.CurrentActor = context.Diagram.Participants[commandOrActor];

        if (context.Parser.EndOfLine)
        {
            return;
        }

        var message = Message.Parse(context) ?? throw new InvalidDiagramException("Unknown message type", context.Parser.LineNumber);
        var recipient = Participant.Next(context);

        if (recipient == null)
        {
            return;
        }

        if (!context.Diagram.Participants.ContainsKey(recipient))
        {
            context.Diagram.Participants.Add(recipient, new Participant(recipient));
        }

        message.Recipient = context.Diagram.Participants[recipient];
    }
}