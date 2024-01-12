using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Services.Core;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models;
using System.Threading.Tasks;

public class CloudSave : MonoBehaviour
{
    private const string CLOUD_SAVE_USER_KEY = "user_data";

    public TMP_InputField meetingDateInput;
    public TMP_InputField meetingTimeInput;
    public TMP_InputField meetingTitleInput;

    public NetworkConnect networkConnect;

    [System.Serializable]
    public class User
    {
        public string username;

        public List<Meeting> meetings;
    }

    [System.Serializable]
    public class Meeting
    {
        public DateTime dateTime;
        public string title;
        public bool isAttendee;
        public List<string> attendees;
    }

    private bool ValidateDateTimeFormat(string date, string time)
    {
        DateTime parsedDateTime;
        return DateTime.TryParseExact(date + " " + time, "dd/MM/yyyy hhmmtt", null, System.Globalization.DateTimeStyles.None, out parsedDateTime);
    }

    public async void SaveData(string attendeeUsername)
    {
        var username = networkConnect.getPlayerId();

        try
        {
            Debug.Log("Attempting to load existing data...");

            // Load existing user data
            User existingUser = await LoadUserData();

            Debug.Log("Loaded existing data.");

            var newMeeting = new Meeting
            {
                dateTime = DateTime.Parse(meetingDateInput.text + " " + meetingTimeInput.text),
                title = meetingTitleInput.text,
                isAttendee = false,
            };

            if (existingUser != null)
            {
                Debug.Log("Appending new meeting to existing data...");
                existingUser.meetings.Add(newMeeting);

                // Add attendee to the new meeting
                if (!string.IsNullOrEmpty(attendeeUsername))
                {
                    newMeeting.attendees = new List<string> { attendeeUsername };
                }
            }
            else
            {
                Debug.Log("Creating new user data with the meeting...");
                existingUser = new User
                {
                    username = username,

                    meetings = new List<Meeting> { newMeeting }
                };

                // Add attendee to the new meeting
                if (!string.IsNullOrEmpty(attendeeUsername))
                {
                    newMeeting.attendees = new List<string> { attendeeUsername };
                }
            }

            var data = new Dictionary<string, object>
            {
                { CLOUD_SAVE_USER_KEY, existingUser }
            };

            Debug.Log("Attempting to save data...");

            // Save updated data
            await CloudSaveService.Instance.Data.Player.SaveAsync(data);

            Debug.Log("Save data success!");
        }
        catch (ServicesInitializationException e)
        {
            Debug.LogError(e);
        }
        catch (CloudSaveValidationException e)
        {
            Debug.LogError(e);
        }
        catch (CloudSaveRateLimitedException e)
        {
            Debug.LogError(e);
        }
        catch (CloudSaveException e)
        {
            Debug.LogError(e);
        }
    }

    public async Task<User> LoadUserData()
    {

        try
        {
            Debug.Log("Attempting to load existing data...");

            // Load user data
            var cloudSaveData = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { CLOUD_SAVE_USER_KEY });

            // Check if the specified key exists in the loaded data
            if (cloudSaveData.TryGetValue(CLOUD_SAVE_USER_KEY, out var userData))
            {
                Debug.Log("Loaded existing data.");
                return userData.Value.GetAs<User>();
            }
            else
            {
                Debug.Log("No existing data found.");
                return null;
            }
        }
        catch (ServicesInitializationException e)
        {
            Debug.LogError(e);
            return null;
        }
        catch (CloudSaveValidationException e)
        {
            Debug.LogError(e);
            return null;
        }
        catch (CloudSaveRateLimitedException e)
        {
            Debug.LogError(e);
            return null;
        }
        catch (CloudSaveException e)
        {
            Debug.LogError(e);
            return null;
        }
    }

    public void OnSaveButtonClicked(string attendeeUsername)
    {
        SaveData(attendeeUsername);
    }

    public void OnLoadUserDataButtonClicked()
    {
        LoadUserData();
    }

    public async Task<List<Meeting>> GetMeetings()
    {
        var keysToLoad = new HashSet<string> { CLOUD_SAVE_USER_KEY };

        try
        {
            Debug.Log("Attempting to get meetings...");

            // Load user data
            var cloudSaveData = await CloudSaveService.Instance.Data.Player.LoadAsync(keysToLoad);

            // Check if the specified key exists in the loaded data
            if (cloudSaveData.TryGetValue(CLOUD_SAVE_USER_KEY, out var userData))
            {
                Debug.Log("Meetings loaded successfully.");
                return userData.Value.GetAs<User>().meetings;
            }
            else
            {
                Debug.Log("No meetings found.");
                return new List<Meeting>();
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return new List<Meeting>();
        }
    }


    // Update the method in CloudSave.cs
    // public async void AddAttendeeToMeeting(string attendeePlayerId, Meeting meetingToAdd)
    // {
    //     try
    //     {
    //         var hostPlayerId = networkConnect.getPlayerId();

    //         // Prepare CloudScript request
    //         var request = new ExecuteCloudScriptRequest
    //         {
    //             FunctionName = "addAttendeeToMeeting",
    //             FunctionParameter = new Dictionary<string, object>
    //             {
    //                 { "hostPlayerId", hostPlayerId },
    //                 { "attendeePlayerId", attendeePlayerId },
    //                 { "meetingToAdd", meetingToAdd.ToJson() }
    //             }
    //         };

    //         // Execute CloudScript function
    //         var response = await CloudCodeService.Instance.CallEndpointAsync<string>("AddAttendeeToMeeting", request.FunctionParameter);

    //         // Handle CloudScript response
    //         if (response != null)
    //         {
    //             Debug.Log("Attendee added successfully!");
    //             // Update UI to reflect successful addition
    //         }
    //         else
    //         {
    //             Debug.LogError("Failed to add attendee.");
    //             // Handle error gracefully
    //         }
    //     }
    //     catch (Exception ex)
    //     {
    //         Debug.LogError("Unexpected error: " + ex.Message);
    //         // Handle unexpected errors
    //     }
    // }

}
