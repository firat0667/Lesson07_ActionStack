using Actions;
using Game.Manager;
using Game.Combat;
using Graphs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Battlescape
{
    public class EnemyAIAction : ActionStack.Action
    {
        private Unit m_unit;
        private bool m_isDone = false;

        public EnemyAIAction(Unit unit)
        {
            m_unit = unit;
        }

        public override void OnBegin(bool bFirstTime)
        {
            base.OnBegin(bFirstTime);

            Unit closestPlayer = null;
            float closestDistance = float.MaxValue;

            
            foreach (Unit possibleTarget in UnitManager.Instance.GetEnemiesOf(m_unit))
            {
                float dist = Vector3.Distance(m_unit.transform.position, possibleTarget.transform.position);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    closestPlayer = possibleTarget;
                }
            }

            if (closestPlayer == null)
            {
                Debug.LogWarning("EnemyAIAction: No closest player found.");
                m_isDone = true;
                return;
            }

            
            if (closestDistance < 1.5f)
            {
                Debug.Log($"{m_unit.name} attacks {closestPlayer.name}!");
                if (closestPlayer is IDamageable damageable)
                {
                    damageable.TakeDamage(1);
                }
                m_unit.RemainingActionPoints = 0;
                m_isDone = true;
                return;
            }

            
            Battlescape.Node targetNode = GraphAlgorithms.GetClosestNode<Battlescape.Node>(
                Battlescape.Instance,
                closestPlayer.transform.position
            );

            
            List<Battlescape.Node> path = GraphAlgorithms.FindShortestPath_AStar(
                Battlescape.Instance,
                m_unit.Node,
                targetNode
            );

            if (path != null && path.Count > 1)
            {
                Battlescape.Node moveTarget = path[1];

                
                if (UnitManager.Instance.IsNodeOccupied(moveTarget))
                {
                    Debug.Log($"{m_unit.name} hedef node dolu, hareket iptal!");
                    m_isDone = true;
                    return;
                }

               
                m_unit.StartCoroutine(DelayedMove(moveTarget));
            }
            else
            {
                Debug.LogWarning("EnemyAIAction: No valid path found.");
                m_isDone = true;
            }
        }


        IEnumerator DelayedMove(Battlescape.Node target)
        {
            yield return new WaitForSeconds(0.3f);

            UnitMoveAction move = null;

            if (target != null)
            {
                yield return new WaitUntil(() => ActionStack.Main.IsEmpty || ActionStack.Main.CurrentAction == this);

                move = m_unit.gameObject.AddComponent<UnitMoveAction>();
                move.Init(m_unit, target);
                ActionStack.Main.PushAction(move);
                m_unit.RemainingActionPoints = 0;

                yield return new WaitUntil(() => move.IsDone());
            }

            m_isDone = true;
        }



        public override bool IsDone() => m_isDone;
    }
}
