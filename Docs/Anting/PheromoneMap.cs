using System.Drawing;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Anting
{
    public class PheromoneMap
    {
        private const float Decay = 0.9f;
        private const float Cutoff = 0.001f; 
        private const float MaxPheromone = 5.0f;

        public readonly int PheromoneTypesCount;
        public readonly int Width;
        public readonly int Height;
        public readonly int Size;
        private readonly float[] _pherMap;

        public float this[int pher, int x, int y]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _pherMap[pher * Size + y * Width + x];
        }

        public PheromoneMap(int pheromoneTypesCount, int width, int height)
        {
            PheromoneTypesCount = pheromoneTypesCount;
            Width = width;
            Height = height;
            Size = Width * Height;
            _pherMap = new float[pheromoneTypesCount * Width * Height];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(int pher, int x, int y, float amount)
        {
            int idx = pher * Size + y * Width + x;
            _pherMap[idx] = Mathf.Min(_pherMap[idx] + amount, MaxPheromone * (pher + 1));
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EvaporateAll()
        {
            for (int pher = 0; pher < PheromoneTypesCount; pher++)
            {
                int baseIdx = pher * Size;

                for (int i = 0; i < Size; i++)
                {
                    int idx = baseIdx + i;

                    float v = _pherMap[idx] * Decay;
                    _pherMap[idx] = (v > Cutoff) ? v : 0f;
                }
            }
        }
    }
}