
using Actions;
using Graphs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Battlescape
{
    public class Unit : ActionStack.ActionBehavior
    {
        private Battlescape.Node    m_node;
        private int                 m_iRemainingActionPoints;

        #region Properties

        public int RemainingActionPoints
        {
            get => m_iRemainingActionPoints;
            set => m_iRemainingActionPoints = Mathf.Max(value, 0);
        }

        public Battlescape.Node Node
        {
            get => m_node;
            set
            {
                if (m_node != value)
                {
                    m_node = value;
                    transform.position = m_node.WorldPosition;
                }
            }
        }

        #endregion

        public void InitializeUnit()
        {
            // get closest node to unit
            m_node = GraphAlgorithms.GetClosestNode<Battlescape.Node>(Battlescape.Instance, transform.position);
        }

        public void OnNewTurn()
        {
            m_iRemainingActionPoints = 2;
        }

        public override void OnBegin(bool bFirstTime)
        {
            base.OnBegin(bFirstTime);

            // do an input action
            if (bFirstTime)
            {
                ActionStack.Main.PushAction(new PlayerInputAction(this));
            }

            /*
            // reduce action points
            m_iRemainingActionPoints--;

            // do random move
            Link link = Node.m_links[Random.Range(0, Node.m_links.Count)];
            Node = link.Target as Battlescape.Node;
            transform.position = Node.WorldPosition;
            */
        }

        public override bool IsDone()
        {
            return true;
        }

        public override string ToString()
        {
            return "Unit: " + name;
        }
    }
}