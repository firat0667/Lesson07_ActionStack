using Game.Battlescape;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Manager
{
    public class UnitManager : MonoBehaviour
    {
        private static UnitManager m_instance;
        public static UnitManager Instance=> m_instance;
        private List<Unit> m_allUnits = new List<Unit>();

        public IReadOnlyList<Unit> AllUnits => m_allUnits;

        private void Awake()
        {
            m_instance = this;
        }

        public void Register(Unit unit)
        {
            if (!m_allUnits.Contains(unit))
                m_allUnits.Add(unit);
        }

        public void Unregister(Unit unit)
        {
            m_allUnits.Remove(unit);
        }

        public IEnumerable<Unit> GetEnemiesOf(Unit self)
        {
            if (self == null)
                yield break;

            foreach (var u in m_allUnits)
            {
                if (u == null) continue;

                //if (u.TeamType != self.TeamType)
                //    yield return u;
            }
        }


    }
}
