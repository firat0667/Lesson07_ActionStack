using Actions;
using System.Collections;
using UnityEngine;
using static Game.Battlescape.Battlescape;

namespace Game.Battlescape
{
    public class UnitMoveAction : ActionStack.ActionBehavior
    {
        private bool m_bIsDone = false;
        private Node m_targetTile;
        private float m_moveSpeed = 1.0f;
        private Unit m_unit; 
        public void Init(Unit unit, Node target)
        {
            this.m_unit = unit;
            this.m_targetTile = target;
        }
        public override bool IsDone()
        {
            return m_bIsDone;
        }

        public override void OnBegin(bool bFirstTime)
        {
            base.OnBegin(bFirstTime);
            if (m_unit != null && m_targetTile != null)
                m_unit.StartCoroutine(MoveRoutine());
            else
                m_bIsDone = true;
        }
      
       public override void OnUpdate()
       {
            base.OnUpdate();
       }
       public override void OnEnd() { }

        IEnumerator MoveRoutine()
        {
            Vector3 start = transform.position;
            Vector3 end = m_targetTile.WorldPosition;
            float t = 0;
            while (t < 1f)
            {
                transform.position = Vector3.Lerp(start, end, t);
                t += Time.deltaTime * m_moveSpeed;
                yield return null;
            }
            transform.position = end;
            m_unit.Node = m_targetTile;
            m_bIsDone = true;
        }

    }
}


