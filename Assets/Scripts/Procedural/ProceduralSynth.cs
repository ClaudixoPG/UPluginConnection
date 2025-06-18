/*[RequireComponent(typeof(AudioSource))]
public class ProceduralSynth : MonoBehaviour
{
    public float frequency = 440f;
    public float amplitude = 0.1f;
    private float phase;
    private int sampleRate;

    public bool isPulsing = true;

    private void Start()
    {
        sampleRate = AudioSettings.outputSampleRate;

        var audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.playOnAwake = true;
        audioSource.volume = 1f;
        audioSource.Play();
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        if (!isPulsing) return;

        float increment = 2 * Mathf.PI * frequency / sampleRate;

        for (int i = 0; i < data.Length; i += channels)
        {
            float sample = Mathf.Sin(phase) * amplitude;

            for (int ch = 0; ch < channels; ch++)
                data[i + ch] = sample;

            phase += increment;
            if (phase > Mathf.PI * 2)
                phase -= Mathf.PI * 2;
        }
    }
}
*/

using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ProceduralSynth : MonoBehaviour
{
    public float frequency = 440f;
    public float amplitude = 0.1f;
    private float phase;
    private int sampleRate;

    public bool isPulsing = true;

    private void Start()
    {
        sampleRate = AudioSettings.outputSampleRate;

        var audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.playOnAwake = true;
        audioSource.volume = 1f;
        audioSource.Play();
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        if (!isPulsing) return;

        float increment = 2 * Mathf.PI * frequency / sampleRate;

        for (int i = 0; i < data.Length; i += channels)
        {
            float sample = Mathf.Sin(phase) * amplitude;

            for (int ch = 0; ch < channels; ch++)
                data[i + ch] = sample;

            phase += increment;
            if (phase > Mathf.PI * 2)
                phase -= Mathf.PI * 2;
        }
    }
}
