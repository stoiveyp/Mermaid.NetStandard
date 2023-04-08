using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Mermaid.NetStandard.SequenceDiagrams;

public class SequenceContext
{
    public SequenceContext(MermaidParser parser)
    {
        Parser = parser;
        Diagram = new SequenceDiagram();
    }

    public SequenceDiagram Diagram { get; set; }
    public MermaidParser Parser { get; set; }
    public Participant? CurrentActor { get; set; }
    public Stack<MessageContainer> Containers { get; set; } = new();
    public MessageContainer? CurrentContainer => Containers.Count > 0 ? Containers.Peek() : null;

    public void AddMessage(Message message)
    {
        if (CurrentContainer != null)
        {
            CurrentContainer.Messages.Add(message);

        }
        else
        {
            Diagram.Messages.Add(message);
        }
    }

    public bool EndContainer()
    {
        if (Containers.Any())
        {
            Diagram.Containers.Add(Containers.Pop());
            return true;
        }

        return false;
    }
}

public abstract class MessageContainer
{
    public List<Message> Messages { get; set; } = new();
}