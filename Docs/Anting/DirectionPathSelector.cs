using System.Collections.Generic;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;

namespace Anting
{
    public class DirectionPathSelector : INodeSelector
    {
        private readonly float _alpha;   // �� �߿䵵
        private readonly float _beta;   // �� �߿䵵
        private readonly Map map;
        private readonly float[] _probs;
        private readonly Vector2Int[] _dirs;
        private readonly AntTrailMemory _trailMemory;
        private const float TAU_MIN = 0.1f;
        private const float ETA_MIN = 0.05f;

        private int _pherIndex = 0;
        private PheromoneMap _pherMap;
        private Vector2Int _dir = Vector2Int.zero;

        public DirectionPathSelector(float alpha, float beta, Vector2Int[] dirs, AntContext context)
        {
            this._alpha = alpha;
            this._beta = beta;
            this.map = context.Map;
            _dirs = dirs;
            this._trailMemory = context.Trail;
            _probs = new float[_dirs.Length];
        }

        public void Reset(Vector2Int target, AntContext context, int pherIndex)
        {
            _pherMap = context.TargetPherMap;
            _pherIndex = pherIndex;
        }

        /// <summary>
        /// pij^k = (��ij^a * ��ij^b) / ��(��ih^a * ��ih^b)
        /// ��ij = 1 / lij
        /// </summary>
        public Vector2Int Go(Vector2Int from)
        {
            float total = 0f;

            for (int i = 0; i < _dirs.Length; i++)
            {
                Vector2Int next = from + _dirs[i];

                if (!map.IsMoveable(next.x, next.y))
                {
                    _probs[i] = 0f;
                    continue;
                }

                float dot = (_dir == Vector2Int.zero) ? 1f : Dot(_dirs[i]);

                float dirWeight;
                if (dot > 0.85f)          
                    dirWeight = 1.0f;
                else if (dot > 0.4f)    
                    dirWeight = 0.6f;
                else if (dot > -0.2f)   
                    dirWeight = 0.35f;
                else if (dot > -0.7f)   
                    dirWeight = 0.15f;
                else                  
                    dirWeight = 0.05f;

                float pher = _pherMap[_pherIndex, next.x, next.y];
                float tau = Mathf.Max(TAU_MIN, pher);
                float eta = Mathf.Max(ETA_MIN, dirWeight);

                float value =
                    Mathf.Pow(tau, _alpha) *
                    Mathf.Pow(eta, _beta);

                _probs[i] = value;
                total += value;

                // ���� (���� ���� ���ʽ�)
                // dot: -1~1 �� 0~1
                // float inertia = Mathf.Clamp01((dot + 1f) * 0.5f);

                // �޸���ƽ (���� ��� + ���� �ݿ�)
                //float eta = 1f;// (1f / l); //* Mathf.Lerp(0.6f, 1.4f, inertia);
            }


            if (total <= 0f)
                return from;

            float r = Random.Range(0, total);
            for (int i = 0; i < _probs.Length; i++)
            {
                r -= _probs[i];
                if (r <= 0f)
                {
                    float dot = (_dir == Vector2Int.zero) ? 0f : Dot(_dirs[i]);

                        _trailMemory.Record(from);

                    _dir = _dirs[i];
                    return from + _dirs[i];
                }
            }

            return from;
        }     

        private float Dot(Vector2Int nextDir)
        {
            return Vector2.Dot(_dir, ((Vector2)nextDir).normalized);
        }
    }
}