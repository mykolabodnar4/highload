using System;

namespace ReplicatedLog.Services;

public class GlobalSequencer
{
    private int _sequenceNumber = 0;

    public int NextSequenceNumber
        => Interlocked.Increment(ref _sequenceNumber);
}
