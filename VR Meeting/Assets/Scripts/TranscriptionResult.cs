using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using Whisper;

public class TranscriptionResult
{
    public string TranscribedText { get; set; }
    public string Language { get; set; }
    // You can add more properties as needed

    public TranscriptionResult(string transcribedText, string language)
    {
        TranscribedText = transcribedText;
        Language = language;
    }
}

