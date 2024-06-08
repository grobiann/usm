using UnityEngine;

namespace USM
{
    [System.Serializable]
    public class GameObjectActivation
    {
        public GameObject GameObject;
        public bool IsActive;

        public GameObjectActivation() { }
        public GameObjectActivation(GameObject go, bool isActive)
        {
            GameObject = go;
            IsActive = isActive;
        }
    }
}