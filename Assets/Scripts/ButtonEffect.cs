using UnityEngine;
using System.Collections;

public class ButtonEffects : MonoBehaviour
{
    public AudioClip clickSound;
    public float scaleAmount = 1.1f;
    public float scaleDuration = 0.1f;

    private AudioSource audioSource;
    private Vector3 originalScale;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        originalScale = transform.localScale;
    }

    public void OnButtonClick()
    {
        // Zvuk
        if (clickSound != null)
            audioSource.PlayOneShot(clickSound);

        // Animacija
        StartCoroutine(ScaleEffect());
    }

    private IEnumerator ScaleEffect()
    {
        // Uvecaj
        transform.localScale = originalScale * scaleAmount;
        yield return new WaitForSeconds(scaleDuration);

        // Vrati na originalnu velicinu
        transform.localScale = originalScale;
    }
}