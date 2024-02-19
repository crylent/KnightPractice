using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class UISoundController : MonoBehaviour
{
    [SerializeField] private AudioClip click;

    private AudioSource _source;

    private void Start()
    {
        _source = GetComponent<AudioSource>();
    }

    public void OnClick()
    {
        PlaySound(click);
    }

    public void PlaySound(AudioClip sound)
    {
        _source.PlayOneShot(sound);
    }
}
