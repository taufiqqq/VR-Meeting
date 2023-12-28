using System;
using System.Collections.Generic;
using UnityEngine;

public class MeetingManager : MonoBehaviour
{
    // Call this method to format meetings
    public string FormatMeetings(List<CloudSave.Meeting> meetings)
    {
        string formattedMeetings = "";

        // Get today's date
        DateTime today = DateTime.Now;

        // Iterate through meetings and filter based on date
        foreach (var meeting in meetings)
        {
            // Check if the meeting date is after today
            if (meeting.dateTime > today)
            {
                // Format the date and time
                string formattedDateTime = meeting.dateTime.ToString("dd/MM/yyyy hh:mmtt");

                // Add meeting information to the formatted string
                formattedMeetings += $"{meeting.title}\n{formattedDateTime}\n\n";
            }
        }

        return formattedMeetings;
    }
}
