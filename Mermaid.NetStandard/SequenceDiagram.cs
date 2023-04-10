using System;
using System.Collections.Generic;
using Mermaid.NetStandard.SequenceDiagrams;

namespace Mermaid.NetStandard
{
    public class SequenceDiagram:MermaidDiagram
    {
        public const string MermaidType = "sequenceDiagram";
        public override string Type => MermaidType;
        public bool AutoNumber { get; set; }
        public Dictionary<string, Participant> Participants { get; set; } = new(StringComparer.InvariantCultureIgnoreCase);
        public List<SequenceElement> Elements { get; set; } = new();
        public List<Box> Boxes { get; set; } = new();
    }
}
