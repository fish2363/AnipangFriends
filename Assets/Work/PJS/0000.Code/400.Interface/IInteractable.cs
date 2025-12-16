using UnityEngine;

namespace Code.Interface
{
    public interface IInteractable
    {
        public void OnInteract();
        public void EnterInteractionRange();
        public void ExitInteractionRange();
    }
}