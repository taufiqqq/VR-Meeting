using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMenu : MonoBehaviour
{
    // UI Pages
    public GameObject loginPage;
    public GameObject mainMenu;
    public GameObject createPage;
    public GameObject joinPage;
    public GameObject viewMeetingPage;
    public GameObject scheduleMeetingPage;

    // Buttons in Login Page
    public Button loginSubmitButton;

    // Buttons in Register Page
    public Button registerSubmitButton;

    // Buttons in Main Menu
    public Button createMeetingButton;
    public Button joinMeetingButton;
    public Button viewMeetingButton;
    public Button scheduleMeetingButton;

    // Buttons in Create Page
    public Button createMeetingSubmitButton;

    // Buttons in Join Page
    public Button joinMeetingSubmitButton;

    // Buttons in Schedule Page
    public Button scheduleMeetingSubmitButton;

    // Other Components
    public NetworkConnect networkConnect;
    public CloudSave cloudSave;
    public MeetingsDisplay meetingDisplay;

    public List<Button> returnButtons; // Use a list of return buttons


    // Start is called before the first frame update
    void Start()
    {
        EnableLoginPage();

        loginSubmitButton.onClick.AddListener(LoginSubmit);
        registerSubmitButton.onClick.AddListener(RegisterSubmit);

        scheduleMeetingButton.onClick.AddListener(EnableScheduleMeetingPage);
        createMeetingButton.onClick.AddListener(EnableCreatePage);
        joinMeetingButton.onClick.AddListener(EnableJoinPage);
        viewMeetingButton.onClick.AddListener(EnableViewMeetingPage);

        createMeetingSubmitButton.onClick.AddListener(CreateMeetingSubmit);

        joinMeetingSubmitButton.onClick.AddListener(JoinMeetingSubmit);

        scheduleMeetingSubmitButton.onClick.AddListener(ScheduleMeetingSubmit);

        foreach (var returnButton in returnButtons)
        {
            returnButton.onClick.AddListener(EnablePreviousPage);
        }
    }

    void EnablePreviousPage()
{
    if (mainMenu.activeSelf)
        EnableLoginPage();
    else if (joinPage.activeSelf)
        EnableMainMenu();
    else if (viewMeetingPage.activeSelf)
        EnableMainMenu();
    else if (scheduleMeetingPage.activeSelf)
        EnableMainMenu();
    else if (createPage.activeSelf)
        EnableMainMenu();
}
 


   public void EnableLoginPage()
    {
        loginPage.SetActive(true);
        mainMenu.SetActive(false);
        createPage.SetActive(false);
        joinPage.SetActive(false);
        viewMeetingPage.SetActive(false);
        scheduleMeetingPage.SetActive(false);
    }

    void LoginSubmit()
    {
        // Implement your login logic using nc
        // Example: nc.Login(username, password);

        // Assuming successful login, enable the main menu
       
    }

    void RegisterSubmit()
    {
        // Implement your registration logic using nc
        // Example: nc.Register(username, password);

        // Assuming successful registration, enable the main menu
        
    }

    public void EnableMainMenu()
    {
        loginPage.SetActive(false);
        mainMenu.SetActive(true);
        createPage.SetActive(false);
        joinPage.SetActive(false);
        viewMeetingPage.SetActive(false);
        scheduleMeetingPage.SetActive(false);
    }

    void EnableScheduleMeetingPage()
    {
        mainMenu.SetActive(false);
        createPage.SetActive(false);
        joinPage.SetActive(false);
        viewMeetingPage.SetActive(false);
        scheduleMeetingPage.SetActive(true);
    }
    void EnableCreatePage()
    {
        mainMenu.SetActive(false);
        createPage.SetActive(true);
        joinPage.SetActive(false);
        viewMeetingPage.SetActive(false);
        scheduleMeetingPage.SetActive(false);
        createMeetingSubmitButton.interactable = true;
    }

    void EnableJoinPage()
    {
        mainMenu.SetActive(false);
        createPage.SetActive(false);
        joinPage.SetActive(true);
        viewMeetingPage.SetActive(false);
        scheduleMeetingPage.SetActive(false);
    }

    async void EnableViewMeetingPage()
    {
        await meetingDisplay.InitializeMeetingsDisplayAsync(); // Use await here

        mainMenu.SetActive(false);
        createPage.SetActive(false);
        joinPage.SetActive(false);
        viewMeetingPage.SetActive(true);
        scheduleMeetingPage.SetActive(false);
    }

    void CreateMeetingSubmit()
    {
        // Implement your create meeting logic using nc
        // Example: nc.CreateMeeting(meetingDetails);

        // Assuming successful meeting creation, enable the main menu
        EnableMainMenu();
    }

    void JoinMeetingSubmit()
    {
        // Implement your join meeting logic using nc
        // Example: nc.JoinMeeting(meetingCode);

        // Assuming successful meeting join, enable the main menu
        EnableMainMenu();
    }

    void ScheduleMeetingSubmit()
    {
        cloudSave.SaveData("", "");
        EnableMainMenu();   
    }
}
