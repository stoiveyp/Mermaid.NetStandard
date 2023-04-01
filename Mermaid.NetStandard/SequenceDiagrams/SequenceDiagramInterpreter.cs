using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mermaid.NetStandard.SequenceDiagrams;

public class SequenceDiagramInterpreter
{
    private static Dictionary<char, ArrowEnding> Endings = new()
    {
        { '>', SequenceDiagrams.ArrowEnding.None },
        { 'X', SequenceDiagrams.ArrowEnding.Cross },
        { ')', SequenceDiagrams.ArrowEnding.Open }
    };

    private const char MessageChar = '-';

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

        var commandOrActor = context.Parser.NextWord();
        if (commandOrActor == null)
        {
            throw new InvalidDiagramException("No actor found", context.Parser.LineNumber);
        }

        if (commands.ContainsKey(commandOrActor))
        {
            if (commands[commandOrActor](context)) return;
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

        var message = ParseMessage(context);
        if (message == null)
        {
            throw new InvalidDiagramException("Unknown message type", context.Parser.LineNumber);
        }

        var recipient = context.Parser.NextWord();
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

    private static bool ArrowEnding(SequenceContext context, Message msg)
    {
        var peek = context.Parser.Peek();

        if (!peek.HasValue)
        {
            return false;
        }

        if (peek.Value == MessageChar)
        {
            msg.Line = ArrowLine.Dotted;
            context.Parser.Next();
            peek = context.Parser.Peek();
        }

        if (peek.HasValue && Endings.ContainsKey(peek.Value))
        {
            context.Parser.Next();
            msg.Ending = Endings[context.Parser.Current];
        }
        else
        {
            return false;
        }

        var secondPeek = context.Parser.Peek();
        if (msg.Ending == SequenceDiagrams.ArrowEnding.None && secondPeek.HasValue &&
            secondPeek.Value == '>')
        {
            context.Parser.Next();
            msg.Ending = SequenceDiagrams.ArrowEnding.Arrowhead;
        }

        return true;
    }

    private static Message ParseMessage(SequenceContext context)
    {
        if (context.Parser.Current != MessageChar)
        {
            return null;
        }

        var msg = new Message
        {
            Originator = context.CurrentActor
        };

        if (!ArrowEnding(context, msg))
        {
            return null;
        }

        context.Parser.Next();

        context.Diagram.Messages.Add(msg);
        return msg;
    }

    private static bool ParseParticipant(SequenceContext context)
    {
        if (context.Parser.EndOfLine)
        {
            return false;
        }

        if (context.Parser.Current == MessageChar)
        {
            return false;
        }

        var identifier = context.Parser.NextWord();
        while (!context.Parser.EndOfLine)
        {
            var next = context.Parser.NextWord();
            if (next == "as")
            {
                if (!context.Parser.EndOfLine)
                {
                    break;
                }
            }

            if (string.IsNullOrEmpty(next))
            {
                context.Diagram.Participants.Add(identifier, identifier);
                return true;
            }

            identifier += " ";
            identifier += next;
        }

        if (context.Parser.EndOfLine)
        {
            context.Diagram.Participants.Add(identifier, identifier);
            return true;
        }

        context.Diagram.Participants.Add(identifier, context.Parser.CurrentLine[context.Parser.CurrentPosition..].Trim());
        return true;
    }
}