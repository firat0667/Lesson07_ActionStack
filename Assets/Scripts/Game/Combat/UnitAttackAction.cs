using Actions;
using Game.Combat;
using System.Collections;
using UnityEngine;
using static Game.Battlescape.Battlescape;

namespace Game.Battlescape
{
    public class UnitAttackAction : ActionStack.ActionBehavior
    {
        private Unit m_attacker;
        private Unit m_target;
        private bool m_isDone;

        public void Init(Unit attacker, Unit target)
        {
            m_attacker = attacker;
            m_target = target;
        }

        public override void OnBegin(bool bFirstTime)
        {
            base.OnBegin(bFirstTime);

            if (m_attacker == null || m_target == null)
            {
                Debug.LogWarning("UnitAttackAction: attacker or target null!");
                m_isDone = true;
                return;
            }

            
            float dist = Vector3.Distance(m_attacker.transform.position, m_target.transform.position);
            if (dist > 1.5f)
            {
                Debug.Log($"{m_attacker.name} cannot reach {m_target.name}, too far!");
                m_isDone = true;
                return;
            }

            if (m_target is IDamageable damageable)
            {
                Debug.Log($"{m_attacker.name} attacks {m_target.name}!");
                damageable.TakeDamage(1);
            }

            
            m_attacker.RemainingActionPoints = 0;
            m_isDone = true;
        }
        public override void OnEnd()
        {
            base.OnEnd();
            Destroy(this); 
        }

        public override bool IsDone() => m_isDone;
    }
}
