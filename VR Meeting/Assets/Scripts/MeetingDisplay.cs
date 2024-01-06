using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class MeetingsDisplay : MonoBehaviour
{
    public Text outputText;
    public RectTransform content;

    [SerializeField] private CloudSave cloudSave;

    public async Task InitializeMeetingsDisplayAsync()
    {
        try
        {
            Debug.Log("Attempting to initialize MeetingsDisplay...");

            if (cloudSave == null)
            {
                Debug.LogError("CloudSave component not found!");
                return;
            }

            // Load meetings data from CloudSave
            List<CloudSave.Meeting> meetings = await cloudSave.GetMeetings();

            if (meetings != null)
            {
                Debug.Log($"Successfully loaded {meetings.Count} meetings.");
            }
            else
            {
                Debug.LogWarning("No meetings loaded or an error occurred during loading.");
            }

            // Update UI with meetings data
            UpdateMeetingsUI(meetings);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error during initialization: {e}");
        }
    }

    private void UpdateMeetingsUI(List<CloudSave.Meeting> meetings)
    {
        try
        {
            Debug.Log("Updating UI with meetings data...");

            if (content == null)
            {
                Debug.LogError("Content RectTransform not assigned!");
                return;
            }

            // Clear existing content
            foreach (Transform child in content.transform)
            {
                Destroy(child.gameObject);
            }

            if (meetings != null)
            {
                // Concatenate meeting information into a single string
                string meetingsInfo = "";
                foreach (var meeting in meetings)
                {
                    meetingsInfo += $"Date: {meeting.dateTime}, Title: {meeting.title}\n\n";
                }

                // Set the concatenated string to outputText
                outputText.text = meetingsInfo;
            }
            else
            {
                Debug.LogWarning("No meetings data to display.");
            }

            Debug.Log("UI update complete.");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error during UI update: {e}");
        }
    }
}
