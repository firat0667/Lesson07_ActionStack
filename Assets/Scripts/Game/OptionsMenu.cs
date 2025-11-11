using Actions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class OptionsMenu : ActionStack.ActionBehavior
    {
        private bool m_bIsDone;

        #region Properties

        #endregion

        public override bool IsDone()
        {
            return m_bIsDone;
        }

        public void OnCancel()
        {
            Debug.Log("Discard User Changes");
            m_bIsDone = true;
        }

        public void OnOkay()
        {
            Debug.Log("Save User Changes");
            m_bIsDone = true;
        }

        public override void OnEnd()
        {
            base.OnEnd();
            Destroy(gameObject);      // commit greater sepuku!
        }

        public static OptionsMenu Create(Transform parent)
        {
            GameObject prefab = Resources.Load<GameObject>("Prefabs/OptionsMenu");
            GameObject go = Instantiate(prefab, parent);
            return go.GetComponent<OptionsMenu>();
        }
    }
}