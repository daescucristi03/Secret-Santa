using System.Collections;
using UnityEngine;

public class RandomSoundPlayer : MonoBehaviour
{
    public AudioClip[] sounds; // Assign a range of sound clips here
    public float minTime = 2f; // Minimum time between sounds
    public float maxTime = 5f; // Maximum time between sounds

    private AudioSource audioSource;

    void Start()
    {
        // Get or add an AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Start the coroutine to play sounds
        StartCoroutine(PlaySoundRandomly());
    }

    IEnumerator PlaySoundRandomly()
    {
        while (true)
        {
            // Wait for a random time between minTime and maxTime
            float waitTime = Random.Range(minTime, maxTime);
            yield return new WaitForSeconds(waitTime);

            // Play a random sound from the range
            if (sounds != null && sounds.Length > 0 && audioSource != null)
            {
                AudioClip selectedSound = sounds[Random.Range(0, sounds.Length)];
                audioSource.clip = selectedSound;
                audioSource.Play();
            }
        }
    }
}
