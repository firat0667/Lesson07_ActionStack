using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Game.Battlescape
{
    [CustomEditor(typeof(Battlescape))]
    public class BattlescapeEditor : Editor
    {
        private void OnSceneGUI()
        {
            Battlescape bs = target as Battlescape;
            Graphs.EditorGraphUtils.DrawGraph(bs);
        }
    }
}