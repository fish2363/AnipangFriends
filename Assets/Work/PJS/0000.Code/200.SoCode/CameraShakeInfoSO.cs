using Unity.Cinemachine;
using Unity.Cinemachine.Editor;
using UnityEngine;

[CreateAssetMenu(fileName = "CameraShakeInfoSO", menuName = "SO/Shake Info")]
public class CameraShakeInfoSO : ScriptableObject
{
    public CinemachineImpulseChannelPropertyAttribute channels;
    public CinemachineImpulseDefinition.ImpulseTypes impulseTypes;
    public float progationSpeed = 300f;
    public float dissipationDistance = 100f;
    [Range(0f, 1f)]
    public float dissipationRate = 0.5f;
    public CinemachineImpulseDefinition.ImpulseShapes impulseShapes;
    public Vector3 velocity;
}
