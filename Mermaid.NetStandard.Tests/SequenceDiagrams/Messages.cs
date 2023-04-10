using Mermaid.NetStandard.SequenceDiagrams;

namespace Mermaid.NetStandard.Tests.SequenceDiagrams;

public class Messages
{
    [Theory]
    [InlineData("A->B", "A", ArrowEnding.None, ArrowLine.Solid, "B")]
    [InlineData("Thing One->Thing Two", "Thing One", ArrowEnding.None, ArrowLine.Solid, "Thing Two")]
    [InlineData("AC->>BD", "AC", ArrowEnding.Arrowhead, ArrowLine.Solid, "BD")]
    [InlineData("AC-XBD", "AC", ArrowEnding.Cross, ArrowLine.Solid, "BD")]
    [InlineData("AC--XBD", "AC", ArrowEnding.Cross, ArrowLine.Dotted, "BD")]
    public async Task ParseMessage(string message, string originator, ArrowEnding ending, ArrowLine line,
        string recipient)
    {
        var src = @$"sequenceDiagram
{message}";
        var diagram = await src.IsDiagramType<SequenceDiagram>();
        Assert.Equal(2, diagram.Participants.Count);
        var msg = Assert.IsType<Message>(Assert.Single(diagram.Elements));
        msg.AssertMessage(originator, ending, line, recipient);
    }
}