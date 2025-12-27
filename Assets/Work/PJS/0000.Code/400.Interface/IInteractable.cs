using UnityEngine;

namespace Code.Interface
{
    public interface ICollectable
    {
        public void OnInteract();
        public void EnterInteractionRange();
        public void ExitInteractionRange();
    }
}