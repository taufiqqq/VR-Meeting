using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
// ReSharper disable RedundantCast

namespace Whisper.Utils
{
    /// <summary>
    /// Portion of recorded audio clip.
    /// </summary>
    public struct AudioChunk
    {
        public float[] Data;
        public int Frequency;
        public int Channels;
        public float Length;
        public bool IsVoiceDetected;
    }
    
    public delegate void OnChunkReadyDelegate(AudioChunk chunk);

    
    /// <summary>
    /// Controls microphone input settings and recording. 
    /// </summary>
    public class MicrophoneRecord : MonoBehaviour
    {
        [Tooltip("Max length of recorded audio from microphone in seconds")]
        public int maxLengthSec = 60;
        [Tooltip("After reaching max length microphone record will continue")]
        public bool loop;
        [Tooltip("Microphone sample rate")]
        public int frequency = 16000;
        [Tooltip("Length of audio chunks in seconds, useful for streaming")]
        public float chunksLengthSec = 0.5f;
        [Tooltip("Should microphone play echo when recording is complete?")]
        public bool echo = true;
        
        [Header("Voice Activity Detection (VAD)")]
        [Tooltip("Should microphone check if audio input has speech?")]
        public bool useVad = true;
        [Tooltip("How often VAD checks if current audio chunk has speech")]
        public float vadUpdateRateSec = 0.1f;
        [Tooltip("Seconds of audio record that VAD uses to check if chunk has speech")]
        public float vadContextSec = 30f;
        [Tooltip("Window size where VAD tries to detect speech")]
        public float vadLastSec = 1.25f;
        [Tooltip("Threshold of VAD energy activation")]
        public float vadThd = 1.0f;
        [Tooltip("Threshold of VAD filter frequency")]
        public float vadFreqThd = 100.0f;
        [Tooltip("Optional indicator that changes color when speech detected")]
        [CanBeNull] public Image vadIndicatorImage;
        
        [Header("VAD Stop")]
        [Tooltip("If true microphone will stop record when no speech detected")]
        public bool vadStop;
        [Tooltip("If true whisper transcription will drop last audio where silence was detected")]
        public bool dropVadPart = true;
        [Tooltip("After how many seconds of silence microphone will stop record")]
        public float vadStopTime = 3f;

        [Header("Microphone selection (optional)")] 
        [Tooltip("Optional UI dropdown with all available microphone inputs")]
        [CanBeNull] public Dropdown microphoneDropdown;
        [Tooltip("The label of default microphone input in dropdown")]
        public string microphoneDefaultLabel = "Default microphone";

        /// <summary>
        /// Raised when VAD status changed.
        /// </summary>

        /// <summary>
        /// Raised when new audio chunk from microphone is ready.
        /// </summary>
        public event OnChunkReadyDelegate OnChunkReady;
        /// <summary>
        /// Raised when microphone record stopped.
        /// Returns <see cref="maxLengthSec"/> or less of recorded audio.
        /// </summary>

       
        private AudioClip _clip;
        private float _length;
        private int _lastChunkPos;
        private int _chunksLength;
        private bool _madeLoopLap;

        /*!!!!!!!!!!!SELECTED MIC DEVICE!!!!!!!!!!!*/
        private string _selectedMicDevice;

        public string SelectedMicDevice
        {
            get => _selectedMicDevice;
            set
            {
                if (value != null && !AvailableMicDevices.Contains(value))
                    throw new ArgumentException("Microphone device not found");
                _selectedMicDevice = value;
            }
        }

        public int ClipSamples => _clip.samples * _clip.channels;

        public string RecordStartMicDevice { get; private set; }
        public bool IsRecording { get; private set; }
        public bool IsVoiceDetected { get; private set; }

        /*!!!!!!!!!!!!!!!!AVAILABLE MIC DEVICE, MICROPHONE!!!!!!!!!!!!!!!!*/
        public IEnumerable<string> AvailableMicDevices => Microphone.devices;

        private void Awake()
        {
            if(microphoneDropdown != null)
            {
                microphoneDropdown.options = AvailableMicDevices
                    .Prepend(microphoneDefaultLabel)
                    .Select(text => new Dropdown.OptionData(text))
                    .ToList();
                microphoneDropdown.value = microphoneDropdown.options
                    .FindIndex(op => op.text == microphoneDefaultLabel);
                microphoneDropdown.onValueChanged.AddListener(OnMicrophoneChanged);
            }
        }

        private void Update()
        {
           
        }
        
        private void UpdateChunks(int micPos)
        {
            // is anyone even subscribe to do this?
            if (OnChunkReady == null)
                return;

            // check if chunks length is valid
            if (_chunksLength <= 0)
                return;
            
            // get current chunk length
            var chunk = GetMicPosDist(_lastChunkPos, micPos);
            
            // send new chunks while there has valid size
            while (chunk > _chunksLength)
            {
                var origData = new float[_chunksLength];
                _clip.GetData(origData, _lastChunkPos);

                var chunkStruct = new AudioChunk()
                {
                    Data = origData,
                    Frequency = _clip.frequency,
                    Channels = _clip.channels,
                    Length = chunksLengthSec,
                    IsVoiceDetected = IsVoiceDetected
                };
                OnChunkReady(chunkStruct);

                _lastChunkPos = (_lastChunkPos + _chunksLength) % ClipSamples;
                chunk = GetMicPosDist(_lastChunkPos, micPos);
            }
        }
        
       
        
        
        private void OnMicrophoneChanged(int ind)
        {
            if (microphoneDropdown == null) return;
            var opt = microphoneDropdown.options[ind];
            SelectedMicDevice = opt.text == microphoneDefaultLabel ? null : opt.text;
        }

        /// <summary>
        /// Start microphone recording
        /// </summary>
        public void StartRecord()
        {
            
        }

        /// <summary>
        /// Stop microphone record.
        /// </summary>
        /// <param name="dropTimeSec">How many last recording seconds to drop.</param>
        

        /// <summary>
        /// Get all recorded mic buffer.
        /// </summary>
        private float[] GetMicBuffer(float dropTimeSec = 0f)
        {
            var micPos = Microphone.GetPosition(RecordStartMicDevice);
            var len = GetMicBufferLength(micPos);
            if (len == 0) return Array.Empty<float>();
            
            // drop last samples from length if necessary
            var dropTimeSamples = (int) (_clip.frequency * dropTimeSec);
            len = Math.Max(0, len - dropTimeSamples);
            
            // get last len samples from recorded audio
            // offset used to get audio from previous circular buffer lap
            var data = new float[len];
            var offset = _madeLoopLap ? micPos : 0;
            _clip.GetData(data, offset);
            
            return data;
        }

        /// <summary>
        /// Get last sec of recorded mic buffer.
        /// </summary>
        private float[] GetMicBufferLast(int micPos, float lastSec)
        {
            var len = GetMicBufferLength(micPos);
            if (len == 0) 
                return Array.Empty<float>();
            
            var lastSamples = (int) (_clip.frequency * lastSec);
            var dataLength = Math.Min(lastSamples, len);
            var offset = micPos - dataLength;
            if (offset < 0) offset = len + offset;

            var data = new float[dataLength];
            _clip.GetData(data, offset);
            return data;
        }

        /// <summary>
        /// Get mic buffer length that was actually recorded.
        /// </summary>
        private int GetMicBufferLength(int micPos)
        {
            // looks like we just started recording and stopped it immediately
            // nothing was actually recorded
            if (micPos == 0 && !_madeLoopLap) 
                return 0;
            
            // get length of the mic buffer that we want to return
            // this need to account circular loop buffer
            var len = _madeLoopLap ? ClipSamples : micPos;
            return len;
        }

        /// <summary>
        /// Calculate distance between two mic positions.
        /// It takes circular buffer into account.
        /// </summary>
        private int GetMicPosDist(int prevPos, int newPos)
        {
            if (newPos >= prevPos)
                return newPos - prevPos;

            // circular buffer case
            return ClipSamples - prevPos + newPos;
        }
    }
}