using Actions;
using Game.Manager;
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
                m_isDone = true; return;
            }
            // get closest node to player
            Battlescape.Node targetNode = GraphAlgorithms.GetClosestNode<Battlescape.Node>(Battlescape.Instance, closestPlayer.transform.position);

            // pick a closer node to move to
            Battlescape.Node moveTarget = null;
            float minDistance = float.MaxValue;
            // look at all linked nodes
            foreach (ILink link in m_unit.Node.Links)
            {
                Battlescape.Node linkedNode = link.Target as Battlescape.Node;
                float dist = Vector3.Distance(linkedNode.WorldPosition, targetNode.WorldPosition);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    moveTarget = linkedNode;
                }
            }
            // start moving actions
            m_unit.StartCoroutine(DelayedMove(moveTarget));
        }
        IEnumerator DelayedMove(Battlescape.Node target)
        {
            yield return new WaitForSeconds(0.3f);

            UnitMoveAction move = null; 

            if (target != null)
            {
                move = m_unit.gameObject.AddComponent<UnitMoveAction>();
                move.Init(m_unit, target);
                ActionStack.Main.PushAction(move);
                m_unit.RemainingActionPoints = 0;
            }
            if (move != null)
                yield return new WaitUntil(() => move.IsDone());

            m_isDone = true;
        }


        public override bool IsDone() => m_isDone;
    }
}
