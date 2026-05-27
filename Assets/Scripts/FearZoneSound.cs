using UnityEngine;

public class FearZoneSound : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip[] scaryClips;

    [Header("Configuración")]
    public bool playOnlyOnce = true;
    public float cooldown = 5f;
    public float minPitch = 0.9f;
    public float maxPitch = 1.1f;

    private bool hasPlayed = false;
    private float lastPlayTime = -999f;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (playOnlyOnce && hasPlayed)
            return;

        if (Time.time < lastPlayTime + cooldown)
            return;

        PlayRandomScarySound();
    }

    private void PlayRandomScarySound()
    {
        if (audioSource == null || scaryClips.Length == 0)
        {
            Debug.LogWarning("Falta AudioSource o clips de miedo en " + gameObject.name);
            return;
        }

        int randomIndex = Random.Range(0, scaryClips.Length);

        audioSource.clip = scaryClips[randomIndex];
        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.Play();

        hasPlayed = true;
        lastPlayTime = Time.time;
    }
}