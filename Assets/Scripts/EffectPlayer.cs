using UnityEngine;

public class EffectPlayer : MonoBehaviour
{
    public ParticleSystem _dustCloud;
    public void PlayDustCloud() => _dustCloud.Play();
}
