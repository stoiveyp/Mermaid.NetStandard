using System.Collections.Generic;

namespace Mermaid.NetStandard.SequenceDiagrams;

public abstract class GroupedSequenceContainer<TPrimary, TAlternative> where TPrimary:SequenceContainer where TAlternative:SequenceContainer
{
    public TPrimary Primary { get; set; }

    public List<TAlternative> Alternatives { get; set; } = new();
}