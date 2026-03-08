using UnityEngine;

public class PheromoneEvaporator
{
    private readonly float evaporation = 0.9f;   // ¥ñ

    /// <summary>
    /// ¥ó(t+1) = (1 - ¥ñ)¥ó(t) + ¥ÄT
    /// </summary>
    public float ApplyEvaporation(float pher)
    {
        return pher *= evaporation;
    }
}
