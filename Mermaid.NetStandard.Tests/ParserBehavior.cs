namespace Mermaid.NetStandard.Tests
{
    public class ParserBehavior
    {
        [Fact]
        public async Task ThrowOnEmptyStream()
        {
            var exc = await Assert.ThrowsAsync<InvalidOperationException>(() => MermaidParser.Parse(string.Empty));
            Assert.Equal(exc.Message, "No diagram found");
        }

        [Fact]
        public async Task ThrowOnUnsupportedDiagram()
        {
            var exc = await Assert.ThrowsAsync<InvalidOperationException>(() => MermaidParser.Parse("test"));
            Assert.Equal(exc.Message, "Diagram type test not supported");
        }

        [Fact]
        public async Task IgnoresBlankPrefixLines()
        {
            var src = @"


sequenceDiagram";
            var seq = await MermaidParser.Parse(src);
            Assert.IsType<SequenceDiagram>(seq);
        }
    }
}