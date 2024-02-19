using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicController : MonoBehaviour
{
    [SerializeField] private AudioClip menu;
    [SerializeField] private AudioClip intro;
    [SerializeField] private AudioClip loop;

    private AudioSource _source;

    private void Start()
    {
        _source = GetComponent<AudioSource>();
    }

    public void PlayMenuTheme()
    {
        _source.clip = menu;
        _source.loop = true;
        _source.Play();
    }

    public void PlayMainTheme()
    {
        var time = _source.time;
        _source.clip = intro;
        _source.loop = false;
        _source.time = time;
        _source.Play();
        StartCoroutine(TransitionToLoop());
    }

    private IEnumerator TransitionToLoop()
    {
        yield return new WaitWhile(() => _source.isPlaying);
        _source.clip = loop;
        _source.loop = true;
        _source.time = 0;
        _source.Play();
    }
}
