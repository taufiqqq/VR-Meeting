using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMenu : MonoBehaviour
{
    // UI Pages
    public GameObject accountValidation;
    public GameObject loginPage;
    public GameObject registerPage;
    public GameObject mainMenu;
    public GameObject createPage;
    public GameObject joinPage;
    public GameObject viewMeetingPage;

    // Buttons in Account Validation
    public Button loginButton;
    public Button registerButton;

    // Buttons in Login Page
    public Button loginSubmitButton;

    // Buttons in Register Page
    public Button registerSubmitButton;

    // Buttons in Main Menu
    public Button createMeetingButton;
    public Button joinMeetingButton;
    public Button viewMeetingButton;

    // Buttons in Create Page
    public Button createMeetingSubmitButton;
    public List<Button> returnButtons; // Use a list of return buttons

    // Buttons in Join Page
    public Button joinMeetingSubmitButton;

    public NetworkConnect nc;

    // Start is called before the first frame update
    void Start()
    {
        EnableAccountValidation();

        // Hook events
        loginButton.onClick.AddListener(EnableLoginPage);
        registerButton.onClick.AddListener(EnableRegisterPage);

        loginSubmitButton.onClick.AddListener(LoginSubmit);
        registerSubmitButton.onClick.AddListener(RegisterSubmit);

        createMeetingButton.onClick.AddListener(EnableCreatePage);
        joinMeetingButton.onClick.AddListener(EnableJoinPage);
        viewMeetingButton.onClick.AddListener(EnableViewMeetingPage);

        createMeetingSubmitButton.onClick.AddListener(CreateMeetingSubmit);

        joinMeetingSubmitButton.onClick.AddListener(JoinMeetingSubmit);

        foreach (var returnButton in returnButtons)
        {
            returnButton.onClick.AddListener(EnablePreviousPage);
        }
    }

    void EnablePreviousPage()
    {
        // Add logic to determine the previous page and enable it
        if (loginPage.activeSelf)
            EnableAccountValidation();
        else if (registerPage.activeSelf)
            EnableAccountValidation();
        else if (mainMenu.activeSelf)
            EnableAccountValidation();
        else if (createPage.activeSelf)
            EnableMainMenu();
        else if (joinPage.activeSelf)
            EnableMainMenu();
        else if (viewMeetingPage.activeSelf)
            EnableMainMenu();
    }

    void EnableAccountValidation()
    {
        accountValidation.SetActive(true);
        loginPage.SetActive(false);
        registerPage.SetActive(false);
        mainMenu.SetActive(false);
        createPage.SetActive(false);
        joinPage.SetActive(false);
        viewMeetingPage.SetActive(false);
    }

    void EnableLoginPage()
    {
        accountValidation.SetActive(false);
        loginPage.SetActive(true);
        registerPage.SetActive(false);
        mainMenu.SetActive(false);
        createPage.SetActive(false);
        joinPage.SetActive(false);
        viewMeetingPage.SetActive(false);
    }

    void EnableRegisterPage()
    {
        accountValidation.SetActive(false);
        loginPage.SetActive(false);
        registerPage.SetActive(true);
        mainMenu.SetActive(false);
        createPage.SetActive(false);
        joinPage.SetActive(false);
        viewMeetingPage.SetActive(false);
    }

    void LoginSubmit()
    {
        // Implement your login logic using nc
        // Example: nc.Login(username, password);

        // Assuming successful login, enable the main menu
        EnableMainMenu();
    }

    void RegisterSubmit()
    {
        // Implement your registration logic using nc
        // Example: nc.Register(username, password);

        // Assuming successful registration, enable the main menu
        EnableMainMenu();
    }

    void EnableMainMenu()
    {
        accountValidation.SetActive(false);
        loginPage.SetActive(false);
        registerPage.SetActive(false);
        mainMenu.SetActive(true);
        createPage.SetActive(false);
        joinPage.SetActive(false);
        viewMeetingPage.SetActive(false);
    }

    void EnableCreatePage()
    {
        mainMenu.SetActive(false);
        createPage.SetActive(true);
        joinPage.SetActive(false);
        viewMeetingPage.SetActive(false);
    }

    void EnableJoinPage()
    {
        mainMenu.SetActive(false);
        createPage.SetActive(false);
        joinPage.SetActive(true);
        viewMeetingPage.SetActive(false);
    }

    void EnableViewMeetingPage()
    {
        mainMenu.SetActive(false);
        createPage.SetActive(false);
        joinPage.SetActive(false);
        viewMeetingPage.SetActive(true);
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
}
