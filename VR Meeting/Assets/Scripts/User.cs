using System.Collections.Generic;

public class User
{
    private List<Meeting> meetings;
    private string password;
    private string playerId;
    private string username;

    // Constructor
    public User(string username, string password, string playerId)
    {
        this.username = username;
        this.password = password;
        this.playerId = playerId;
        this.meetings = new List<Meeting>();
    }

    // Meetings Property
    public List<Meeting> Meetings
    {
        get { return meetings; }
        set { meetings = value ?? new List<Meeting>(); }
    }

    // Password Property
    public string Password
    {
        get { return password; }
        set { password = value; }
    }

    // PlayerId Property
    public string PlayerId
    {
        get { return playerId; }
        set { playerId = value; }
    }

    // Username Property
    public string Username
    {
        get { return username; }
        set { username = value; }
    }
}
