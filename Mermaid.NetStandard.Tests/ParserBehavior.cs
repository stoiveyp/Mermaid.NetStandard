namespace Mermaid.NetStandard.Tests
{
    public class ParserBehavior
    {
        [Fact]
        public async Task ThrowOnEmptyStream()
        {
            var exc = await Assert.ThrowsAsync<InvalidOperationException>(() => MermaidParser.Parse(string.Empty));
            Assert.Equal("No diagram found", exc.Message);
        }

        [Fact]
        public async Task ThrowOnUnsupportedDiagram()
        {
            var exc = await Assert.ThrowsAsync<InvalidOperationException>(() => MermaidParser.Parse("test"));
            Assert.Equal("Diagram type test not supported", exc.Message);
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