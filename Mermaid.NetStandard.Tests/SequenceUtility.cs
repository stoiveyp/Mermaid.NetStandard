﻿using Mermaid.NetStandard.SequenceDiagrams;

namespace Mermaid.NetStandard.Tests;

public static class SequenceUtility
{
    public static void AssertMessage(this Message msg, string originator, ArrowEnding ending, ArrowLine line, string recipient)
    {
        Assert.Equal(ending, msg.Ending);
        Assert.Equal(line, msg.Line);
        Assert.Equal(originator, msg.Originator!.Name);
        Assert.Equal(recipient, msg.Recipient!.Name);
    }
}