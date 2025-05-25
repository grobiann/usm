using System.Collections.Generic;
using UnityEngine;

namespace USM
{
    [System.Serializable]
    public class UiStateMachine
    {
        public IReadOnlyList<UsmState> States => _states;
        public IReadOnlyList<GameObject> ActiveTargets => _activeTargets;

        [SerializeField] private List<UsmState> _states = new List<UsmState>();
        [SerializeField] private List<GameObject> _activeTargets = new List<GameObject>();

        public void AddState(UsmState state)
        {
            _states.Add(state);
        }

        public void RemoveState(UsmState state)
        {
            _states.Remove(state);
        }

        public void AddTarget(GameObject go)
        {
            _activeTargets.Add(go);
        }

        public void RemoveTarget(GameObject go)
        {
            _activeTargets.Remove(go);
        }

        public bool RemoveInvalidLinks()
        {
            bool removed = false;
            for (int i = _activeTargets.Count - 1; i >= 0; i--)
            {
                var go = _activeTargets[i];
                if (go == null)
                {
                    _activeTargets.RemoveAt(i);
                    removed = true;
                }
            }

            for (int i = _states.Count - 1; i >= 0; i--)
            {
                var state = _states[i];
                if (state == null)
                {
                    _states.RemoveAt(i);
                    removed = true;
                    continue;
                }

                for (int j = state.GoActivations.Count - 1; j >= 0; j--)
                {
                    var activation = state.GoActivations[j];
                    if (activation.GameObject == null)
                    {
                        state.GoActivations.RemoveAt(j);
                        removed = true;
                    }
                }
            }

            return removed;
        }
    }
}