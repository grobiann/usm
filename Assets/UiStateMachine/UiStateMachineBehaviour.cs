using UnityEngine;

namespace USM
{
    public class UiStateMachineBehaviour : MonoBehaviour
    {
        public UiState CurrentState { get; protected set; }
        public string CurrentStateName => CurrentState == null ? string.Empty : CurrentState.StateName;

        public UiStateMachine usm;

        public void Play(string stateName)
        {
            var state = usm.states.Find(x => x.StateName == stateName);
            if (state == null)
            {
                Debug.LogWarning($"There does not exist a state with name: '{stateName}'");
                return;
            }

            Play(state);
        }

        public void Play(UiState state)
        {
            CurrentState = state;

            foreach (var go in usm.activeTargets)
            {
                bool active = state.IsActive(go);
                go.gameObject.SetActive(active);
            }
        }
    }
}