using UnityEngine;

public class PlayParticleVFX : MonoBehaviour
{
    [field:SerializeField] public string VfxName { get; private set; }
    [SerializeField] private bool isOnPosition;

    [SerializeField] private ParticleSystem[] particles;
        
    public void PlayVFX(Vector3 position, Quaternion rotation)
    {
        if(isOnPosition == false)
            transform.SetPositionAndRotation(position, rotation);

        foreach (ParticleSystem particle in particles)
        {
            particle.Play();
        }
    }

    public void StopVFX()
    {
        foreach (ParticleSystem particle in particles)
        {
            particle.Stop();
        }
    }

    private void OnValidate()
    {
        if(string.IsNullOrEmpty(VfxName)  == false)
            gameObject.name = VfxName;
    }
}
