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
}