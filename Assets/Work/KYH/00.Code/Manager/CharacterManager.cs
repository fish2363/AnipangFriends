using System;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    [SerializeField] private PlayerInputSO InputSO;
    [SerializeField] private Entity _entity;
    [SerializeField] private CharacterSO[] testCharacter;
    private int testIdx;

    private void Awake()
    {
        InputSO.OnChangePressed += ChangeMainCharacter;
    }

    private void OnDestroy()
    {
        InputSO.OnChangePressed -= ChangeMainCharacter;
    }

    private void ChangeMainCharacter()
    {
        if (testCharacter == null || testCharacter.Length == 0)
        {
            Debug.LogWarning("[CharacterManager] No characters assigned.");
            return;
        }

        testIdx++;

        if (testIdx >= testCharacter.Length)
            testIdx = 0;

        _entity.ChangeInfo(testCharacter[testIdx]);

        Debug.Log($"[ChangeMainCharacter] Changed to index {testIdx}");
    }
}
