using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class OutputAudioRecorder : MonoBehaviour
{
    private int outputRate;
    private int headerSize = 44; // default for uncompressed wav
    private string fileName = "Output.mp3"; // Always overwrite Recording.mp3
    private bool recOutput = false;
    private FileStream fileStream;
    float[] tempDataSource;
    public Button StartBTN;
    

    void Awake()
    {
        outputRate = AudioSettings.outputSampleRate;
    }

    public void StartRecording()
    {
        if (!recOutput)
        {
            try
            {
                StartWriting();
                recOutput = true;
                StartBTN.image.color = Color.red;
                Debug.Log("Start Recording");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error starting recording: {e.Message}");
            }
        }
        else
        {
            Debug.Log("Recording is in progress already");
        }
    }

    public void StopRecording()
{
    recOutput = false;
    try
    {
        WriteHeader();
        StartBTN.image.color = Color.white;
        Debug.Log("Stop Recording");

        string filePath = Path.Combine(Application.dataPath, fileName); // Save in the project folder
        Debug.Log("Generated MP3 File Path: " + filePath);

        // Refresh the Asset Database to make Unity aware of the changes
        UnityEditor.AssetDatabase.Refresh();
    }
    catch (Exception e)
    {
        Debug.LogError($"Error stopping recording: {e.Message}");
    }
    finally
    {
        CloseFileStream();
    }
}

    private void StartWriting()
    {
        string filePath = Path.Combine(Application.dataPath, fileName);
        try
        {
            fileStream = new FileStream(filePath, FileMode.Create);
            var emptyByte = new byte();
            for (int i = 0; i < headerSize; i++) // preparing the header
            {
                fileStream.WriteByte(emptyByte);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error starting writing: {e.Message}");
            throw; // Re-throw the exception after logging
        }
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        if (recOutput)
        {
            ConvertAndWrite(data); // audio data is interlaced
        }
    }

    private void ConvertAndWrite(float[] dataSource)
    {
        try
        {
            var intData = new Int16[dataSource.Length];
            var bytesData = new Byte[dataSource.Length * 2];
            var rescaleFactor = 32767;
            for (var i = 0; i < dataSource.Length; i++)
            {
                intData[i] = (Int16)(dataSource[i] * rescaleFactor);
                var byteArr = new Byte[2];
                byteArr = BitConverter.GetBytes(intData[i]);
                byteArr.CopyTo(bytesData, i * 2);
            }
            fileStream.Write(bytesData, 0, bytesData.Length);
            tempDataSource = new float[dataSource.Length];
            tempDataSource = dataSource;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error converting and writing: {e.Message}");
            throw; // Re-throw the exception after logging
        }
    }

    private void WriteHeader()
    {
        try
        {

        Debug.LogError($"1: ");
        fileStream.Seek(0, SeekOrigin.Begin);
        var riff = System.Text.Encoding.UTF8.GetBytes("RIFF");

        Debug.LogError($"2: ");
        fileStream.Write(riff, 0, 4);
        var chunkSize = BitConverter.GetBytes(fileStream.Length - 8);
        
        Debug.LogError($"3: ");
        fileStream.Write(chunkSize, 0, 4);
        var wave = System.Text.Encoding.UTF8.GetBytes("WAVE");
        
        Debug.LogError($"4: ");
        fileStream.Write(wave, 0, 4);
        var fmt = System.Text.Encoding.UTF8.GetBytes("fmt ");
        
        Debug.LogError($"5: ");
        fileStream.Write(fmt, 0, 4);
        var subChunk1 = BitConverter.GetBytes(16);
        
        Debug.LogError($"6: ");
        fileStream.Write(subChunk1, 0, 4);
        UInt16 two = 2;
        UInt16 one = 1;
        var audioFormat = BitConverter.GetBytes(one);
        
        Debug.LogError($"7: ");
        fileStream.Write(audioFormat, 0, 2);
        var numChannels = BitConverter.GetBytes(two);
        
        Debug.LogError($"8: ");
        fileStream.Write(numChannels, 0, 2);
        var sampleRate = BitConverter.GetBytes(outputRate);
        
        Debug.LogError($"9: ");
        fileStream.Write(sampleRate, 0, 4);
        var byteRate = BitConverter.GetBytes(outputRate * 4);
        
        Debug.LogError($"10: ");
        fileStream.Write(byteRate, 0, 4);
        UInt16 four = 4;
        var blockAlign = BitConverter.GetBytes(four);
        
        Debug.LogError($"11: ");
        fileStream.Write(blockAlign, 0, 2);
        UInt16 sixteen = 16;
        var bitsPerSample = BitConverter.GetBytes(sixteen);
        
        Debug.LogError($"12: ");
        fileStream.Write(bitsPerSample, 0, 2);
        var dataString = System.Text.Encoding.UTF8.GetBytes("data");
        
        Debug.LogError($"13: ");
        fileStream.Write(dataString, 0, 4);
        var subChunk2 = BitConverter.GetBytes(fileStream.Length - headerSize);
        
        Debug.LogError($"14: ");
        fileStream.Write(subChunk2, 0, 4);
        fileStream.Close();
        }
        catch (Exception e)
        {
            Debug.LogError($"Error writing header: {e.Message}");
            throw; // Re-throw the exception after logging
        }
    }

    private void CloseFileStream()
    {
        if (fileStream != null)
        {
            try
            {
                fileStream.Close();
                fileStream.Dispose();
            }
            catch (Exception e)
            {
                Debug.LogError($"Error closing file stream: {e.Message}");
            }
        }
    }
}
