using System.Collections.Generic;
using UnityEngine;

namespace USM
{
    [System.Serializable]
    public class UiStateMachine
    {
        public List<UiState> states = new List<UiState>();
        public List<GameObject> activeTargets = new List<GameObject>();

        public void AddState(UiState state)
        {
            states.Add(state);
        }

        public void RemoveState(UiState state)
        {
            states.Remove(state);
        }

        public void AddTarget(GameObject go)
        {
            activeTargets.Add(go);
        }

        public void RemoveTarget(GameObject go)
        {
            activeTargets.Remove(go);
        }
    }
}