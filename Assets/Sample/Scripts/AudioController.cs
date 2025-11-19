using UnityEngine;

public class AudioController : MonoBehaviour
{
    public AudioSource[] AudioSources;

    public void EnableAudio(bool enableAudio)
    {
        foreach (AudioSource audio in AudioSources)
            audio.mute = !enableAudio;
    }
}
