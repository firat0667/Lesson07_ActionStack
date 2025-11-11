using Actions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class MainMenu : ActionStack.ActionBehavior
    {
        private void Start()
        {
            ActionStack.Main.PushAction(this);
        }

        public void OnNewGame()
        {

        }

        public void OnOptions()
        {
            // create and push the options menu on the main stack!
            Canvas canvas = GetComponentInChildren<Canvas>();
            OptionsMenu om = OptionsMenu.Create(canvas.transform);
            ActionStack.Main.PushAction(om);
        }

        public void OnQuit()
        {

        }

        public override bool IsDone()
        {
            return false;
        }
    }
}