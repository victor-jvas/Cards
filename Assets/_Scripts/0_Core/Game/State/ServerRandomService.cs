using System;

public sealed class ServerRandomService
{
    private readonly Random m_rng;

    public int Seed { get; }

    public ServerRandomService(int seed)
    {
        Seed = seed;
        m_rng = new Random(seed);
    }

    public int NextInt(int minInclusive, int maxExclusive) => m_rng.Next(minInclusive, maxExclusive);
}
