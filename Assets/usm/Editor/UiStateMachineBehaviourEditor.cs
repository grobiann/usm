using UnityEditor;
using UnityEngine;

namespace USM
{
    [CustomEditor(typeof(UiStateMachineBehaviour))]
    public class UiStateMachineBehaviourEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            UiStateMachineBehaviour usm = (UiStateMachineBehaviour)target;

            if (GUILayout.Button("Open USM Window"))
            {
                var window = USMWindow.ShowWindow();
                if (window.CurrentUsmBehaviour != usm)
                {
                    window.SelectUsmBehaviour(usm);
                }
            }
        }
    }
}