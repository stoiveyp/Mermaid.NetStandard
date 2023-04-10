using System.Collections.Generic;

namespace Mermaid.NetStandard.SequenceDiagrams;

public abstract class SequenceContainer:SequenceElement
{
    public List<SequenceElement> Elements { get; set; } = new();
}