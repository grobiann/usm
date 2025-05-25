using UnityEditor;
using UnityEngine;

namespace Usm.Editor
{
    [CustomEditor(typeof(UIStateMachineBehaviour))]
    public class UIStateMachineBehaviourEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            UIStateMachineBehaviour usm = (UIStateMachineBehaviour)target;

            if (GUILayout.Button("Open USM Window"))
            {
                var window = UsmWindow.ShowWindow();
                if (window.CurrentUsmBehaviour != usm)
                {
                    window.SelectUsmBehaviour(usm);
                }
            }
        }
    }
}