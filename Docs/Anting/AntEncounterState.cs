using UnityEditor;
using UnityEngine;

namespace Anting
{
    public class AntEncounterState : IAntState
    {
        private AntContext _context;
        private IEncounterable _encounted;

        private float _timer;
        private float _timerMax;

        public AntEncounterState(AntContext context)
        {
            _context = context;
        }
        
        public void Enter()
        {
            _encounted = _context.CurEncounted;
            _timer = 0f;
            _timerMax = Random.value;

            if (_encounted != null)
            {
                _encounted.Exploration(_context.My);
            }
        }

        public EAntState? Execute()
        {
            if (_encounted is Object uo && uo == null)
                return EAntState.Search;

            Transform target = _encounted.Transform;

            float stopDistance = 1f;
            MoveTowardTarget(target, stopDistance);

            _timer += Time.deltaTime;

            if (_encounted is IAttackable enemy &&
                enemy.IsEnemy(_context.Team))
            {
                if (_timer < _timerMax)
                    return null;

                float confidence = CalcConfidence(enemy);
                 
                if (confidence > 0.7)
                {
                    _context.CurAttackTarget = enemy;
                    _encounted.ReceiveMessage(_context.My, EAntState.Attack);
                    return EAntState.Attack;
                }

                if (confidence < 0.4)
                {
                    _encounted.ReceiveMessage(_context.My, EAntState.FoodReturn);
                    _context.TargetSearch = (EAntWith)1;
                    return EAntState.FoodReturn;
                }

                return Random.value < 0.5 ? EAntState.Attack : EAntState.Search;
            }

            return EAntState.Search;
        }

        public void Exit()
        {
            _encounted = null;
        }

        private void MoveTowardTarget(Transform target, float stopDistance)
        {
            Vector3 myPos = _context.My.transform.position;
            Vector3 targetPos = target.position;

            Vector3 dir = targetPos - myPos;
            float dist = dir.magnitude;

            FaceTarget(target);

            if (dist > stopDistance)
            {
                dir /= dist;
                float speed = 10f;

                _context.My.transform.position +=
                    dir * speed * Time.deltaTime;
            }
        }


        private void FaceTarget(Transform target)
        {
            Vector3 dir = target.position - _context.My.transform.position;
            //_context.Renderer.Look(dir.x > 0);
            //dir.y = 0f;

            //if (dir.sqrMagnitude < 0.0001f)
            //    return;

            //Quaternion rot = Quaternion.LookRotation(dir);

            //_context.My.transform.rotation =
            //    Quaternion.Slerp(
            //        _context.My.transform.rotation,
            //        rot,
            //        Time.deltaTime * 1f);
        }

        private float CalcConfidence(IAttackable enemy)
        {
            int diff = _context.Stat.ATK - enemy.Armor;

            const int autoAttackThreshold = 30;
            const float k = 0.15f;              

            if (diff >= autoAttackThreshold)
                return 1f;

            float confidence = 1f / (1f + Mathf.Exp(-diff * k));
            return confidence;
        }
    }
}
