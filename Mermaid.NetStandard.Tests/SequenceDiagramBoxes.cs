using System.Drawing;
using Mermaid.NetStandard.SequenceDiagrams;

namespace Mermaid.NetStandard.Tests;

public class SequenceDiagramBoxes
{
    [Fact]
    public async Task BoxParsingRequiresEnd()
    {
        var src = @"sequenceDiagram
box transparent test box";
        await src.IsInvalidDiagram<InvalidDiagramException>("Box without end");
    }

    [Fact]
    public async Task BoxMessagesAreAttachedToBox()
    {
        var src = @"sequenceDiagram
box transparent test box
A-->>B
end";
        var drg = await src.IsDiagramType<SequenceDiagram>();
        var box = Assert.IsType<Box>(drg.Elements.First());
        var msg = Assert.IsType<Message>(Assert.Single(box.Elements));
        msg.AssertMessage("A", ArrowEnding.Arrowhead, ArrowLine.Dotted, "B");
    }

    public static IEnumerable<object?[]> BoxData => new List<object?[]>
    {
        new object?[] { string.Empty, Color.Transparent, null },
        new object?[] { "transparent label", Color.Transparent, "label" },
        new object[] { "purple green", Color.Purple, "green" },
        new object[] { "AntiqueWHite stuffy thing", Color.AntiqueWhite, "stuffy thing" },
        new object[] { "totally not green", Color.Transparent, "totally not green" },
        new object[] { "rgb(50,60,70) not green", Color.FromArgb(50, 60, 70), "not green" },
    };

    [Theory]
    [MemberData(nameof(BoxData))]
    public async Task ValidBoxAppearsInDiagram(string boxText, Color color, string? label)
    {
        var src = $@"sequenceDiagram
box {boxText}
end";
        var drg = await src.IsDiagramType<SequenceDiagram>();
        var container = Assert.Single(drg.Elements);
        var box = Assert.IsType<Box>(container);
        Assert.Equal(color, box.Color);
        Assert.Equal(label, box.Label);
    }
}