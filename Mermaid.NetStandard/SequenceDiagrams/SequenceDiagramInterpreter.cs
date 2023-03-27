using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Xml;

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

    private static Dictionary<char, ArrowEnding> Endings = new()
    {
        { '>', SequenceDiagrams.ArrowEnding.None },
        { 'X', SequenceDiagrams.ArrowEnding.Cross },
        { ')', SequenceDiagrams.ArrowEnding.Open }
    };

    private static bool ArrowEnding(SequenceContext context, Message msg)
    {
        var peek = context.Parser.Peek();
        
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
        if (context.Parser.Current != '-')
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