using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Services.Core;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models;

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
        public int level;
        public List<Meeting> meetings;
    }

    [System.Serializable]
    public class Meeting
    {
        public DateTime dateTime;
        public string title;
    }

private bool ValidateDateTimeFormat(string date, string time)
{
    DateTime parsedDateTime;
    return DateTime.TryParseExact(date + " " + time, "dd/MM/yyyy hhmmtt", null, System.Globalization.DateTimeStyles.None, out parsedDateTime);
}

public async void SaveData()
{
    var user = new User
    {
        username = networkConnect.getPlayerId(),
        meetings = new List<Meeting>
        {
            new Meeting
            {
                dateTime = DateTime.Parse(meetingDateInput.text + " " + meetingTimeInput.text),
                title = meetingTitleInput.text
            }
        }
    };

    var data = new Dictionary<string, object>
    {
        { CLOUD_SAVE_USER_KEY, user }
    };

    try
    {
        Debug.Log("Attempting to save data...");

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


    public async void LoadUserData()
    {
        var keysToLoad = new HashSet<string>
        {
            CLOUD_SAVE_USER_KEY
        };

        try
        {
            var loadedData = await CloudSaveService.Instance.Data.Player.LoadAsync(keysToLoad);

            if (loadedData.TryGetValue(CLOUD_SAVE_USER_KEY, out var loadedUserObj) && loadedUserObj is Dictionary<string, Item>)
            {
                var loadedUserData = JsonUtility.FromJson<User>(JsonUtility.ToJson(loadedUserObj));

                Debug.Log("Loaded saved username: " + loadedUserData.username);

                foreach (var meeting in loadedUserData.meetings)
                {
                    Debug.Log($"{meeting.title}\n{meeting.dateTime}");
                }
            }
            else
            {
                Debug.Log("User data not found in cloud save");
            }
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

    public void OnSaveButtonClicked()
    {
        SaveData();
    }

    public void OnLoadUserDataButtonClicked()
    {
        LoadUserData();
    }

    public List<Meeting> GetMeetings()
    {
        var keysToLoad = new HashSet<string> { CLOUD_SAVE_USER_KEY };

        try
        {
            var loadedData = CloudSaveService.Instance.Data.LoadAsync(keysToLoad).Result;

            if (loadedData.TryGetValue(CLOUD_SAVE_USER_KEY, out var loadedUserObj) && loadedUserObj is Dictionary<string, object>)
            {
                var loadedUserData = JsonUtility.FromJson<User>(JsonUtility.ToJson(loadedUserObj));

                return loadedUserData.meetings;
            }
            else
            {
                return new List<Meeting>();
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return new List<Meeting>();
        }
    }
}
