using Code.Data;
using System.Collections.Generic;
using UnityEngine;

public class Store : MonoBehaviour
{
    [SerializeField] private List<ItemCounter> counters;
    [SerializeField] private List<ItemWeightData> datas;

    public void OnRoomInited()
    {
        List<ItemInfo> itemInfos = ShuffleItem(counters.Count);
        for(int i = 0; i < itemInfos.Count; i++)
        {
            counters[i].SetItem(itemInfos[i]);
        }
    }

    private List<ItemInfo> ShuffleItem(int count)
    {
        List<ItemInfo> infos = new List<ItemInfo>(count);
        List<ItemWeightData> availableItems = new List<ItemWeightData>(datas);

        float totalWeight = 0;
        foreach (ItemWeightData data in availableItems)
        {
            totalWeight += data.weight;
        }

        for (int i = 0; i < count; ++i)
        {
            if (availableItems.Count == 0 || totalWeight <= 0)
            {
                break;
            }

            float val = Random.Range(0, totalWeight);
            float currentWeight = 0;
            ItemWeightData removeTarget = default;

            foreach (ItemWeightData weightData in availableItems)
            {
                currentWeight += weightData.weight;
                if (currentWeight >= val)
                {
                    removeTarget = weightData;
                    break;
                }
            }

            if (removeTarget.itemInfo != null)
            {
                infos.Add(removeTarget.itemInfo);

                totalWeight -= removeTarget.weight;

                availableItems.Remove(removeTarget);
            }
        }

        return infos;
    }
}
