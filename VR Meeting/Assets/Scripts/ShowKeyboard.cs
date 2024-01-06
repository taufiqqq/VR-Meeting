using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Experimental.UI;
using TMPro;
public class ShowKeyboard : MonoBehaviour
{
    private TMP_InputField inputfield;

    public float distance = 0.5f;
    public float verticalOffset = 0;
    
    public Transform positionSource;

    // Start is called before the first frame update
    void Start()
    {
        inputfield = GetComponent<TMP_InputField>();
        inputfield.onSelect.AddListener(x => OpenKeyboard());
    }

    // Update is called once per frame
    public void OpenKeyboard()
    {
        NonNativeKeyboard.Instance.InputField = inputfield;
        NonNativeKeyboard.Instance.PresentKeyboard(inputfield.text);
    
        Vector3 direction = positionSource.forward;
        direction.y = 0;
        direction.Normalize();

        Vector3 targetPosition = positionSource.position + direction*distance + Vector3.up * verticalOffset;

        NonNativeKeyboard.Instance.RepositionKeyboard(targetPosition);
    }
}
