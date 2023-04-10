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

    public Box? CurrentBox { get; set; }

    public Stack<SequenceContainer> Containers { get; set; } = new();
    public SequenceContainer? CurrentContainer => Containers.Count > 0 ? Containers.Peek() : null;
    public object? IsOpen() => (object?)CurrentContainer ?? CurrentBox;

    public void AddElement(SequenceElement element)
    {
        if (CurrentBox != null)
        {
            throw new InvalidDiagramException("Only participants can be added to boxes");
        }

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
        if (CurrentBox != null)
        {
            CurrentBox = null;
            return true;
        }

        if (Containers.Any())
        {
            Diagram.Elements.Add(Containers.Pop());
            return true;
        }

        return false;
    }

    public Participant EnsureParticipant(string name, string alias, ParticipantType type = ParticipantType.Participant)
    {
        if (!Diagram.Participants.TryGetValue(name, out Participant participant))
        {
            participant = new Participant(name, alias, type);
            CurrentBox?.Participants.Add(participant);
            Diagram.Participants.Add(name, participant);
        }

        return participant;
    }
}