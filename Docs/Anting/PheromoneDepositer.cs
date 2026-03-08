using UnityEngine;
using UnityEngine.UIElements;

namespace Anting
{
    public class PheromoneDepositer
    {
        private PheromoneMap pherMap;

        private int pathLength;
        private int currentIndex;

        private readonly float minDeposit;

        private float _minDeposit;
        private float _maxDeposit;
        private int _pherIndex = 0;

        public PheromoneDepositer(
            float minDeposit = 0.01f)
        {
            this.minDeposit = minDeposit;
        }

        public void Reset(int pherIndex, int stepCount, AntContext context, float min, float max)
        {
            _pherIndex = pherIndex;
            pherMap = context.TargetPherMap;
            pathLength = stepCount;
            currentIndex = 0;
            _minDeposit = Mathf.Max(0.01f, min / pathLength);
            _maxDeposit = max / pathLength;
        }

        public void Deposit(Vector2Int pos)
        {
            if (pathLength <= 0 || currentIndex >= pathLength)
                return;

            float t = (float)currentIndex / (pathLength - 1);
            t = 1 - t;

            float strength = Mathf.Lerp(_minDeposit, _maxDeposit, t * t * t);

            pherMap.Add(_pherIndex, pos.x, pos.y, strength);

            currentIndex++;
        }
    }

}