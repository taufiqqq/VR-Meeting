using UnityEngine;

public class AudioListenerExample : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Example usage of AudioListener.volume
        SetAudioListenerVolume(0.5);

        // Example usage of AudioListener.pause
        SetAudioListenerPause(false);

        // Example usage of AudioListener.velocityUpdateMode
        SetAudioListenerVelocityUpdateMode(AudioVelocityUpdateMode.Fixed);

        // Example usage of AudioListener.GetOutputData
        float[] audioData = GetAudioListenerOutputData(1024, 0);
        // Process the audioData as needed

        // Example usage of AudioListener.GetSpectrumData
        float[] spectrumData = GetAudioListenerSpectrumData(1024, 0, FFTWindow.Hamming);
        // Process the spectrumData as needed
    }

    // Example function using AudioListener.volume
    void SetAudioListenerVolume(float volume)
    {
        AudioListener.volume = volume;
    }

    // Example function using AudioListener.pause
    void SetAudioListenerPause(bool pause)
    {
        AudioListener.pause = pause;
    }

    // Example function using AudioListener.velocityUpdateMode
    void SetAudioListenerVelocityUpdateMode(AudioVelocityUpdateMode mode)
    {
        AudioListener.velocityUpdateMode = mode;
    }

    // Example function using AudioListener.GetOutputData
    float[] GetAudioListenerOutputData(int numSamples, int channel)
    {
        float[] audioData = new float[numSamples];
        AudioListener.GetOutputData(audioData, channel);
        return audioData;
    }

    // Example function using AudioListener.GetSpectrumData
    float[] GetAudioListenerSpectrumData(int numSamples, int channel, FFTWindow window)
    {
        float[] spectrumData = new float[numSamples];
        AudioListener.GetSpectrumData(spectrumData, channel, window);
        return spectrumData;
    }
}
  