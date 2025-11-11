using Actions;
using Game.Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Battlescape
{
    public class Team : ActionStack.ActionBehavior
    {
        [SerializeField] private TeamType m_teamType = TeamType.Player;

        private List<Unit> m_units = new List<Unit>();

        public override void OnBegin(bool bFirstTime)
        {
            base.OnBegin(bFirstTime);

            m_units = new List<Unit>(GetComponentsInChildren<Unit>());

            if (bFirstTime)
            {
                // let units know a new turn has started!
                foreach (Unit unit in m_units)
                {
                    unit.OnNewTurn();
                }
            }
            foreach (Unit u in m_units)
                u.TeamType= m_teamType;

            // remove all units that have no remaining action points
            m_units.RemoveAll(u => u == null || u.RemainingActionPoints == 0);

            // pick next unit
            Unit nextUnit = m_units.Find(u => u.RemainingActionPoints > 0);
            if (nextUnit != null)
            {
                ActionStack.Main.PushAction(nextUnit);
            }
        }

        public override bool IsDone()
        {
            return m_units.TrueForAll(u => u == null || u.RemainingActionPoints == 0);
        }


        public override string ToString()
        {
            return "Team: " + name;
        }

    }
}