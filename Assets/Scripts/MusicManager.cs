using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource sourceA;
    [SerializeField] private AudioSource sourceB;

    [Header("Settings")]
    [SerializeField] private float fadeDuration = 2f;

    [SerializeField] private float maxVolume = 0.25f;

    [SerializeField] private bool crossFade;

    private AudioSource currentSource;
    private AudioSource nextSource;

    private AudioClip currentClip;
    private AudioClip nextClip;

    private Coroutine crossfadeRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        currentSource = sourceA;
        nextSource = sourceB;
    }

    public void EnterLevel(AudioClip clip)
    {
        if (clip == null) return;

        if (currentClip == clip) return;

        if (currentClip == null)
        {
            PlayNow(clip);
            return;
        }

        nextClip = clip;

        Debug.Log("Próxima música: " + clip.name);

        
        if (crossfadeRoutine != null) { 
            StopCoroutine(crossfadeRoutine); 
        }

        crossfadeRoutine = StartCoroutine(WaitForLoopThenCrossfade());
    }

    void PlayNow(AudioClip clip)
    {
        currentClip = clip;

        currentSource.clip = clip;
        currentSource.volume = maxVolume;
        currentSource.loop = true;
        currentSource.Play();

        Debug.Log("Tocando agora: " + clip.name);
    }

    IEnumerator WaitForLoopThenCrossfade()
    {
        if (currentSource.clip == null)
            yield break;

        float remainingTime = currentSource.clip.length - currentSource.time;

        // Espera terminar o loop atual
        yield return new WaitForSeconds(remainingTime);

        // Se ainda existe música na fila
        if (nextClip != null)
        {
            if (crossFade)
            {
                // crossfade
                yield return StartCoroutine(Crossfade());
            }
            else
            {
                // troca direta sem fade
                currentSource.Stop();
                PlayNow(nextClip);
                nextClip = null;
            }
        }

        crossfadeRoutine = null;
    }

    private IEnumerator Crossfade()
    {
        // se não tiver música na fila
        if (nextClip == null)
        {
            crossfadeRoutine = null;
            yield break;
        }

        nextSource.clip = nextClip;
        nextSource.volume = 0f;
        nextSource.loop = true;
        nextSource.Play();

        float time = 0f;

        while (time < fadeDuration)
        {
            // Se a música foi cancelada durante o fade
            if (nextClip == null)
            {
                nextSource.Stop();
                nextSource.clip = null;

                crossfadeRoutine = null;
                yield break;
            }

            time += Time.deltaTime;
            float t = time / fadeDuration;

            currentSource.volume = Mathf.Lerp(maxVolume, 0f, t);
            nextSource.volume = Mathf.Lerp(0f, maxVolume, t);

            yield return null;
        }

        // Finaliza troca
        currentSource.Stop();

        // Swap das fontes
        var temp = currentSource;
        currentSource = nextSource;
        nextSource = temp;

        currentClip = currentSource.clip;
        nextClip = null;

        crossfadeRoutine = null;

        Debug.Log("Nova música: " + currentClip.name);
    }
}
