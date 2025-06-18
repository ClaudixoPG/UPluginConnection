using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ProceduralSynth : MonoBehaviour
{
    public float frequency = 120f;    // Grave
    public float amplitude = 0.5f;    // Fuerte
    public float pingDuration = 0.2f; // En segundos
    public bool triggerPing = false;

    private float phase;
    private int sampleRate;
    private float pingTimer = 0f;
    private bool isPinging = false;

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
        if (triggerPing)
        {
            isPinging = true;
            pingTimer = pingDuration;
            triggerPing = false;
        }

        float increment = 2 * Mathf.PI * frequency / sampleRate;

        for (int i = 0; i < data.Length; i += channels)
        {
            float sample = 0f;

            if (isPinging)
            {
                // Envolvente: decae a lo largo del ping
                float t = 1f - (pingDuration - pingTimer) / pingDuration;
                float envelope = Mathf.Clamp01(t);
                sample = Mathf.Sin(phase) * amplitude * envelope;

                pingTimer -= 1f / sampleRate;
                if (pingTimer <= 0f)
                    isPinging = false;
            }

            for (int ch = 0; ch < channels; ch++)
                data[i + ch] = sample;

            phase += increment;
            if (phase > Mathf.PI * 2)
                phase -= Mathf.PI * 2;
        }
    }

    public void PlayPing(float newFreq = 120f, float newAmp = 0.5f)
    {
        frequency = newFreq;
        amplitude = newAmp;
        triggerPing = true;
    }
}
