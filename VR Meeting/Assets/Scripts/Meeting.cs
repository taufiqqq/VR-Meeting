using System;
using System.Collections.Generic;

public class Meeting
{
    private List<User> attendees;
    private DateTime dateTime;
    private string hostUsername;
    private string title;

    // Constructor
    public Meeting(List<User> attendees, DateTime dateTime, string hostUsername, string title)
    {
        this.attendees = attendees ?? new List<User>();
        this.dateTime = dateTime;
        this.hostUsername = hostUsername;
        this.title = title;
    }

    // Attendees Property
    public List<User> Attendees
    {
        get { return attendees; }
        set { attendees = value ?? new List<User>(); }
    }

    // DateTime Property
    public DateTime DateTime
    {
        get { return dateTime; }
        set { dateTime = value; }
    }

    // HostUsername Property
    public string HostUsername
    {
        get { return hostUsername; }
        set { hostUsername = value; }
    }

    // Title Property
    public string Title
    {
        get { return title; }
        set { title = value; }
    }
}
