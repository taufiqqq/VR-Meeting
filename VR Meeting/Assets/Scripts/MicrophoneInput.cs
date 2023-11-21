using UnityEngine;

public class MicrophoneExample : MonoBehaviour
{
    // Audio source component for playback
    private AudioSource audioSource;

    // Name of the microphone device to use
    public string selectedMicrophone;

    // Duration of recording in seconds
    public float recordingDuration;

    // Sampling rate for recording (Hz)
    public int samplingRate;

    // Start is called before the first frame update
    void Start()
    {
        // Loop through each available microphone device and log its name
        foreach (var device in Microphone.devices)
        {
            Debug.Log("Microphone Device: " + device);
        }

        // Initialize audio source component
        audioSource = GetComponent<AudioSource>();

        // Start recording with the specified microphone and settings
        StartRecording(selectedMicrophone, recordingDuration, samplingRate);
    }

    
        void Update()
    {
        // Assuming 'example' is an instance of the script
        int currentPosition = GetPosition(example.selectedMicrophone);

        //////////
    }


    // Start recording with the specified microphone and settings
    private void StartRecording(string deviceName, float duration, int rate)
    {
        audioSource.clip = Microphone.Start("Built-in Microphone", true, 10, 44100);
        audioSource.Play();
    }

    // Check if a specific microphone device is currently recording
    public static bool IsRecording(string deviceName)
    {
        return Microphone.IsRecording(deviceName);
    }

    // Get the position in samples of the recording
    public static int GetPosition(string deviceName)
    {
        return Microphone.GetPosition(deviceName);
    }

    // Stop recording for a specific microphone device
    public static void End(string deviceName)
    {
        Microphone.End(deviceName);
    }
}
