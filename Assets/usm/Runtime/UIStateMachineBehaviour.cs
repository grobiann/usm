using UnityEngine;

namespace Usm
{
    public class UIStateMachineBehaviour : MonoBehaviour
    {
        public UsmState CurrentState { get; protected set; }
        public string CurrentStateName => CurrentState != null ? CurrentState.StateName : string.Empty;
        public UIStateMachine Usm => _usm;

        [SerializeField] private UIStateMachine _usm;

        public void SetState(string stateName)
        {
            for (int i = 0; i < _usm.States.Count; i++)
            {
                if (_usm.States[i].StateName == stateName)
                {
                    SetState(_usm.States[i]);
                    return;
                }
            }

            Debug.LogWarning($"There does not exist a state with name: '{stateName}'");
        }

        public void SetState(UsmState state)
        {
            CurrentState = state;

            foreach (GameObject go in _usm.ActiveTargets)
            {
                bool active = state.IsActive(go);
                go.SetActive(active);
            }
        }
    }
}