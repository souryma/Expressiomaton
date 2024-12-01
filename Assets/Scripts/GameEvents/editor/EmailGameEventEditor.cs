using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameEvents.editor
{
    [CustomEditor(typeof(EmailGameEvent))]
    public class EmailGameEventEditor : Editor
    {
        [SerializeField]
        private string m_email = "";

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var l_script = (EmailGameEvent)target;

            GUILayout.Label("test email");
            m_email = EditorGUILayout.TextField("email", m_email);
            if(GUILayout.Button("Raise", GUILayout.Height(40)) && m_email.Length != 0)
            {
                l_script.RaiseEvent(m_email);
            }
        
        }
    }
}