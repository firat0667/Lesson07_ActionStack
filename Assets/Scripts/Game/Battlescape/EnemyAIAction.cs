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
                UnitAttackAction attack = m_unit.gameObject.AddComponent<UnitAttackAction>();
                attack.Init(m_unit, closestPlayer);
                ActionStack.Main.PushAction(attack);
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
              
                Battlescape.Node moveTarget = null;
                for (int i = 1; i < path.Count; i++)
                {
                    if (!UnitManager.Instance.IsNodeOccupied(path[i]))
                    {
                        moveTarget = path[i];
                        break;
                    }
                }

                if (moveTarget != null)
                {
                    m_unit.StartCoroutine(DelayedMove(moveTarget));
                }
                else
                {
                    Debug.Log($"{m_unit.name} path üzerindeki tüm node'lar dolu, hareket iptal!");
                    m_isDone = true;
                }
            }
            else
            {
                Debug.LogWarning("EnemyAIAction: No valid path found.");
                m_isDone = true;
            }
        }

        private IEnumerator DelayedMove(Battlescape.Node target)
        {
            yield return new WaitForSeconds(0.3f);

            if (target != null)
            {
                yield return new WaitUntil(() => ActionStack.Main.IsEmpty || ActionStack.Main.CurrentAction == this);

                UnitMoveAction move = m_unit.gameObject.AddComponent<UnitMoveAction>();
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
