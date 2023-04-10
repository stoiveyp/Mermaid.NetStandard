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
    public Stack<SequenceContainer> Containers { get; set; } = new();
    public SequenceContainer? CurrentContainer => Containers.Count > 0 ? Containers.Peek() : null;

    public void AddElement(SequenceElement element)
    {
        if (CurrentContainer != null)
        {
            CurrentContainer.Elements.Add(element);

        }
        else
        {
            Diagram.Elements.Add(element);
        }
    }
    
    public bool EndContainer()
    {
        if (Containers.Any())
        {
            Diagram.Elements.Add(Containers.Pop());
            return true;
        }

        return false;
    }

    public Participant EnsureParticipant(string name, string alias, ParticipantType type = ParticipantType.Participant)
    {
        if (!Diagram.Participants.ContainsKey(name))
        {
            Diagram.Participants.Add(name, new Participant(name, alias, type));
        }

        return Diagram.Participants[name];
    }
}