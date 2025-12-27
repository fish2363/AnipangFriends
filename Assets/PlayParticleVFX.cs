using UnityEngine;

    public class PlayParticleVFX : MonoBehaviour, IPlayableVFX
    {
        [field:SerializeField] public string VfxName { get; private set; }
        [SerializeField] private bool isOnPosition;

        [SerializeField] private ParticleSystem[] particles;
        
        private void OnValidate()
        {
            if(string.IsNullOrEmpty(VfxName)  == false)
                gameObject.name = VfxName;
        }

    public void PlayVfx(Vector3 position, Quaternion rotation)
    {
        if (isOnPosition == false)
            transform.SetPositionAndRotation(position, rotation);

        foreach (ParticleSystem particle in particles)
        {
            particle.Play();
        }
    }

    public void StopVfx()
    {
        foreach (ParticleSystem particle in particles)
        {
            particle.Stop();
        }
    }
}
