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
        var openContainer = sequenceContext.IsOpen();
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
        { "end", sc => sc.EndContainer() },
        { "activate", sc => Activation.Parse(sc, ActivationType.Activate)},
        { "deactivate", sc => Activation.Parse(sc, ActivationType.Deactivate)}
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
        if (!char.IsLetterOrDigit(context.Parser.Current))
        {
            return;
        }

        var commandOrActor = context.Parser.NextWord() ?? throw new InvalidDiagramException("No actor found", context.Parser.LineNumber);
        if (Commands.TryGetValue(commandOrActor, out var command))
        {
            if (command(context)) return;
        }

        if (context.Parser.EndOfLine)
        {
            context.EnsureParticipant(commandOrActor, commandOrActor);
            return;
        }

        var restOfCommand = Participant.Next(context);
        if (!string.IsNullOrEmpty(restOfCommand))
        {
            commandOrActor += " ";
            commandOrActor += restOfCommand;
        }

        context.CurrentActor = context.EnsureParticipant(commandOrActor, commandOrActor);

        if (context.Parser.EndOfLine)
        {
            return;
        }

        var message = Message.Parse(context) ?? throw new InvalidDiagramException("Unknown message type", context.Parser.LineNumber);
        var activationType = Activation.ParseShortcut(context);

        var recipient = Participant.Next(context);

        if (recipient == null)
        {
            return;
        }

        message.Recipient =  context.EnsureParticipant(recipient, recipient);

        if (activationType == ActivationType.Activate)
        {
            context.AddElement(new Activation(activationType.Value,message.Recipient!));
        }

        context.AddElement(message);

        if (activationType == ActivationType.Deactivate)
        {
            context.AddElement(new Activation(activationType.Value, message.Originator!));
        }
    }
}