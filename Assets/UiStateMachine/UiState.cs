using System.Collections.Generic;
using UnityEngine;

namespace USM
{
    [System.Serializable]
    public class UiState
    {
        public string StateName
        {
            get { return _stateName; }
            set { _stateName = value; }
        }
        [SerializeField] private string _stateName;

        public List<GameObjectActivation> GoActivations = new List<GameObjectActivation>();

        public bool IsActive(GameObject go)
        {
            foreach (var activation in GoActivations)
            {
                if (activation.GameObject == go)
                    return activation.IsActive;
            }

            return false;
        }

        public void SetActive(GameObject go, bool active)
        {
            var obj = GoActivations.Find(x => x.GameObject == go);
            if (obj != null)
            {
                obj.IsActive = active;
                return;
            }

            GoActivations.Add(new GameObjectActivation(go, active));
        }
    }
}