using UnityEngine;
using Code.Interface;
using Code.Events;
using Core.EventBus;
public class ItemCounter : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemInfo itemInfo;

    public void SetItem(ItemInfo item)
    {
        itemInfo = item;
    }
    public void EnterInteractionRange()
    {
        
    }

    public void ExitInteractionRange()
    {
        
    }
    [ContextMenu("GetItem")]

    public void OnInteract()
    {
        Bus<GoldDecreaseEvent>.Raise(
            new GoldDecreaseEvent
            { 
            amount = itemInfo.Cost,
            ResultCallback = BuyCallback
        });
    }

    private void BuyCallback(bool Succeed)
    {
        if(Succeed)
        {
            Debug.Log("Succeed");
        }
        else
        {
            Debug.Log("Failed");
        }
    }
}
