using UnityEngine;

namespace GondrLib.Effects
{
    public interface IPlayableVFX
    {
        public string VfxName { get; }
        public void PlayVFX(Vector3 position, Quaternion rotation);
        public void StopVFX();
    }
}