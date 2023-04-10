using Mermaid.NetStandard.SequenceDiagrams;

namespace Mermaid.NetStandard.Tests;

public class SequenceDiagramParticipants
{
    [Fact]
    public async Task NoParticipantActorGeneratesParticipant()
    {
        var src = @"
sequenceDiagram
participant
-->";
        var drg = await src.IsDiagramType<SequenceDiagram>();
        Assert.Equal("participant", Assert.Single(drg.Participants).Value.Name);
    }

    [Fact]
    public async Task ExplicitParticipantRequiresSpace()
    {
        var src = @"sequenceDiagram
participant->BC";
        var diagram = await src.IsDiagramType<SequenceDiagram>();
        Assert.Equal("participant", diagram.Participants.First().Key);
    }

    [Theory]
    [InlineData("participant BC Thingy", "BC Thingy", "BC Thingy")]
    [InlineData("participant BC", "BC", "BC")]
    [InlineData("participant BC as", "BC as", "BC as")]
    [InlineData("participant A as B", "A", "B")]
    [InlineData("actor as as as", "as", "as", ParticipantType.Actor)]
    [InlineData("participant as as as", "as", "as", ParticipantType.Participant)]
    [InlineData("participant as as as as", "as", "as as")]
    [InlineData("participant thing one as thing two", "thing one", "thing two")]
    [InlineData("actor thing one as thing two", "thing one", "thing two", ParticipantType.Actor)]
    public async Task ParseParticipant(string line, string id, string alias,
        ParticipantType type = ParticipantType.Participant)
    {
        var src = @$"sequenceDiagram
{line}";
        var diagram = await src.IsDiagramType<SequenceDiagram>();
        var part = Assert.Single(diagram.Participants);
        Assert.Equal(id, part.Key);
        Assert.Equal(id, part.Value.Name);
        Assert.Equal(alias, part.Value.Alias);
        Assert.Equal(type, part.Value.Type);
    }
}