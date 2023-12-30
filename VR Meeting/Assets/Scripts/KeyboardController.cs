using UnityEngine;
using TMPro;

public class KeyboardController : MonoBehaviour
{
    // Reference to the associated TextMeshProUGUI input field
    public TMP_InputField associatedInputField;

    // Function to update the input field with the pressed key
    public void UpdateInputField(string key)
    {
        // Check if there's an associated input field
        if (associatedInputField != null)
        {
            // Update only the associated input field
            associatedInputField.text += key;
        }
    }
}
