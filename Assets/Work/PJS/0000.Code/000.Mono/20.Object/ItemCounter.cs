using UnityEngine;
using Code.Interface;
using Code.Events;
using Core.EventBus;
public class ItemCounter : MonoBehaviour, Code.Interface.ICollectable
{
    [SerializeField] private ItemInfoSO itemInfo;

    public void SetItem(ItemInfoSO item)
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
