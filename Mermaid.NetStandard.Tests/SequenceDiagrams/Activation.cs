using Mermaid.NetStandard.SequenceDiagrams;

namespace Mermaid.NetStandard.Tests.SequenceDiagrams;

public class Activation
{
    [Fact]
    public async Task ActivationShortcut()
    {
        var src = @"sequenceDiagram
A-->>+B";
        var drg = await src.IsDiagramType<SequenceDiagram>();
        Assert.Equal(2, drg.Participants.Count);
        var act = Assert.IsType<NetStandard.SequenceDiagrams.Activation>(drg.Elements.First());
        Assert.Equal(ActivationType.Activate, act.Type);
        Assert.Equal(drg.Participants["B"], act.Participant);
    }

    [Fact]
    public async Task DeactivationShortcut()
    {
        var src = @"sequenceDiagram
A-->>-B";
        var drg = await src.IsDiagramType<SequenceDiagram>();
        Assert.Equal(2, drg.Participants.Count);
        var act = Assert.IsType<NetStandard.SequenceDiagrams.Activation>(drg.Elements.Skip(1).First());
        Assert.Equal(ActivationType.Deactivate, act.Type);
        Assert.Equal(drg.Participants["A"], act.Participant);
    }

    [Fact]
    public async Task ActivationCommandExplicitParticipant()
    {
        var src = @"sequenceDiagram
participant A
activate A";
        var drg = await src.IsDiagramType<SequenceDiagram>();
        Assert.Single(drg.Participants);
        var act = Assert.IsType<NetStandard.SequenceDiagrams.Activation>(Assert.Single(drg.Elements));
        Assert.Equal(ActivationType.Activate, act.Type);
        Assert.Equal(drg.Participants["A"], act.Participant);
    }

    [Fact]
    public async Task DeaactivateCommandExplicitParticipant()
    {
        var src = @"sequenceDiagram
participant A
deactivate A";
        var drg = await src.IsDiagramType<SequenceDiagram>();
        Assert.Single(drg.Participants);
        var act = Assert.IsType<NetStandard.SequenceDiagrams.Activation>(Assert.Single(drg.Elements));
        Assert.Equal(ActivationType.Deactivate, act.Type);
        Assert.Equal(drg.Participants["A"], act.Participant);
    }


    [Fact]
    public async Task ActivationCommandImplicitParticipant()
    {
        var src = @"sequenceDiagram
activate A";
        var drg = await src.IsDiagramType<SequenceDiagram>();
        Assert.Single(drg.Participants);
        var act = Assert.IsType<NetStandard.SequenceDiagrams.Activation>(Assert.Single(drg.Elements));
        Assert.Equal(ActivationType.Activate, act.Type);
        Assert.Equal(drg.Participants["A"], act.Participant);
    }

    [Fact]
    public async Task DeaactivateCommandImplicitParticipant()
    {
        var src = @"sequenceDiagram
deactivate A";
        var drg = await src.IsDiagramType<SequenceDiagram>();
        Assert.Single(drg.Participants);
        var act = Assert.IsType<NetStandard.SequenceDiagrams.Activation>(Assert.Single(drg.Elements));
        Assert.Equal(ActivationType.Deactivate, act.Type);
        Assert.Equal(drg.Participants["A"], act.Participant);
    }
}