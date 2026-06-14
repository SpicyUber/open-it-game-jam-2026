using UnityEngine;

public class EffectPlayer : MonoBehaviour
{
    public ParticleSystem _dustCloud;
    public ParticleSystem _dustTrail;
    public void PlayDustCloud() => _dustCloud.Play();

    public void ToggleDustTrailOff() => _dustTrail.Stop();

    public void ToggleDustTrailOn() => _dustTrail.Play();

    //
}
