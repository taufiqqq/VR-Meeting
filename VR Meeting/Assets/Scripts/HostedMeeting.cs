using UnityEngine;
using UnityEngine.UI;

public class HostedMeeting : MonoBehaviour
{
    public CloudSave cloudSave;
    public MeetingManager mm;
    public Text meetingsText;

    void Start()
    {
        meetingsText.text = "HOSTED MEETINGS : \n";
        // Retrieve the meetings as a formatted string from MeetingManager
        string formattedMeetings = mm.FormatMeetings(cloudSave.GetMeetings());

        // Set the formatted meetings text to the Text component
        if (meetingsText != null)
        {
            meetingsText.text += formattedMeetings;
        }
        else
        {
            Debug.LogError("MeetingsText component is not assigned in the inspector.");
        }
    }
}
