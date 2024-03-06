using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(GameEvent))]
public class GameEventEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var l_script = (GameEvent)target;

        if(GUILayout.Button("Raise", GUILayout.Height(40)))
        {
            l_script.Raise();
        }
        
    }
}
