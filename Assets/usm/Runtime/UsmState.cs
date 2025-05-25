using System.Collections.Generic;
using UnityEngine;

namespace USM
{
    [System.Serializable]
    public struct GameObjectActivation
    {
        public GameObject GameObject;
        public bool IsActive;

        public GameObjectActivation(GameObject go, bool isActive)
        {
            GameObject = go;
            IsActive = isActive;
        }
    }

    [System.Serializable]
    public class UsmState
    {
        public string StateName
        {
            get { return _stateName; }
            set { _stateName = value; }
        }
        public List<GameObjectActivation> GoActivations
        {
            get { return _goActivations; }
            set { _goActivations = value; }
        }

        [SerializeField] private string _stateName;
        [SerializeField] private List<GameObjectActivation> _goActivations = new List<GameObjectActivation>();

        public bool IsActive(GameObject go)
        {
            foreach (var activation in _goActivations)
            {
                if (activation.GameObject == go)
                {
                    return activation.IsActive;
                }
            }

            return false;
        }

        public void SetActive(GameObject go, bool active)
        {
            for (int i = 0; i < _goActivations.Count; i++)
            {
                if (_goActivations[i].GameObject == go)
                {
                    _goActivations[i] = new GameObjectActivation(go, active);
                    return;
                }
            }

            _goActivations.Add(new GameObjectActivation(go, active));
        }
    }
}