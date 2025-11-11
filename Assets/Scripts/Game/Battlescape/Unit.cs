
using Actions;
using Game.Manager;
using Game.Combat;
using Graphs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Battlescape
{
    public enum TeamType { Player, Enemy }
    public class Unit : ActionStack.ActionBehavior,IDamageable
    {
        private Battlescape.Node    m_node;
        private int                 m_iRemainingActionPoints;

        private TeamType m_teamType;

        #region Properties

        private int m_maxHealth = 2;
        private int m_currentHealth;

        public int RemainingActionPoints
        {
            get => m_iRemainingActionPoints;
            set => m_iRemainingActionPoints = Mathf.Max(value, 0);
        }
        public TeamType TeamType
        {
            get=> m_teamType;
            set=> m_teamType = value;
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

        public int MaxHealth => m_maxHealth;
        public int CurrentHealth => m_currentHealth;

        #endregion

        public void InitializeUnit()
        {
            // get closest node to unit
            m_node = GraphAlgorithms.GetClosestNode<Battlescape.Node>(Battlescape.Instance, transform.position);
            m_currentHealth = m_maxHealth;
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
                if (m_teamType == TeamType.Enemy)
                {
                    ActionStack.Main.PushAction(new EnemyAIAction(this));
                }
                else
                {
                    ActionStack.Main.PushAction(new PlayerInputAction(this));
                }
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

        private void OnEnable()
        {
            UnitManager.Instance?.Register(this);
        }
        private void OnDisable()
        {
            UnitManager.Instance?.Unregister(this);
        }

        public void TakeDamage(int amount)
        {
            m_currentHealth -= amount;
            Debug.Log($"{name} took {amount} damage! ({m_currentHealth}/{m_maxHealth})");

            if (m_currentHealth <= 0)
            {
                OnDeath();
            }
        }

        public void OnDeath()
        {
            Debug.Log($"{name} has died!");
            UnitManager.Instance?.Unregister(this);
            Destroy(gameObject);
        }
    }
}