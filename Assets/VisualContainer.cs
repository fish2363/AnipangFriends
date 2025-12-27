using UnityEngine;

public class VisualContainer : MonoBehaviour, IChangableInfo
{
    [SerializeField] private GameObject currentVisual;

    public void Change(CharacterSO info)
    {
        Destroy(currentVisual);
        currentVisual = Instantiate(info.visual,transform);
    }
}
