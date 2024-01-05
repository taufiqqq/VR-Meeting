using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MeetingsDisplay : MonoBehaviour
{
    public Text outputText;
    public RectTransform content;

    private CloudSave cloudSave;

    private void Start()
    {
        cloudSave = FindObjectOfType<CloudSave>();
        if (cloudSave != null)
        {
            DisplayMeetings();
        }
        else
        {
            Debug.LogError("CloudSave script not found in the scene!");
        }
    }

    private void DisplayMeetings()
    {
        List<CloudSave.Meeting> meetings = cloudSave.GetMeetings().Result;

        if (meetings != null && meetings.Count > 0)
        {
            string meetingsText = "Meetings:\n";

            foreach (var meeting in meetings)
            {
                meetingsText += $"{meeting.title} - {meeting.dateTime}\n";
            }

            outputText.text = meetingsText;

            // Optionally, you can instantiate UI elements for each meeting in a scroll view
            CreateUIElements(meetings);
        }
        else
        {
            outputText.text = "No meetings found.";
        }
    }

    private void CreateUIElements(List<CloudSave.Meeting> meetings)
    {
        // Clear existing content
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        // Instantiate UI elements for each meeting
        foreach (var meeting in meetings)
        {
            GameObject meetingTextObject = new GameObject("MeetingText", typeof(Text));
            Text meetingText = meetingTextObject.GetComponent<Text>();
            meetingText.text = $"{meeting.title} - {meeting.dateTime}";
            meetingText.transform.SetParent(content, false);
        }
    }
}
