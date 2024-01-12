using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System.Threading.Tasks;
using TMPro;
using System;
using UnityEditor.Search;

public class AuthManager : MonoBehaviour
{
    [SerializeField] private GameObject signInDisplay = default;
    [SerializeField] private TMP_InputField usernameInput = default;
    [SerializeField] private TMP_InputField passwordInput = default;
    [SerializeField] private TextMeshProUGUI errorMessageText = default;
    public float displayErrorDuration = 5f;

    public static AuthManager Instance;

    public LobbyMenu lobbyMenu;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool IsUserSignedIn { get; private set; }

    // Start is called before the first frame update
    async void Start()
    {
        await UnityServices.InitializeAsync();
        bool isSignedIn = AuthenticationService.Instance.IsSignedIn;
        if(isSignedIn)
        {
            signInDisplay.SetActive(false);
        }
    }

    public async void Create() //register
    {
        string usernameText = usernameInput.text;
        string passwordText = passwordInput.text;
        await SignUpWithUsernameAndPassword(usernameText,passwordText);

        if(IsUserSignedIn)
        lobbyMenu.EnableMainMenu();

    }

    public async void SignIn() //login
    {
        string usernameText = usernameInput.text;
        string passwordText = passwordInput.text;
        await SignInWithUsernameAndPassword(usernameText,passwordText);

        if(IsUserSignedIn)
        lobbyMenu.EnableMainMenu();
    }

    public async void SignOut(){
        await SignOutOfGame();
    }

    async Task SignOutOfGame()
    {
        try
        {
            AuthenticationService.Instance.SignOut(true);
            
        IsUserSignedIn = false;
            Debug.Log("User is signed out");
        }
        catch(AuthenticationException ex)
        {
            ShowErrorMessage(ex.Message);
        }
        catch(RequestFailedException ex)
        {
            ShowErrorMessage(ex.Message);
        }

        lobbyMenu.EnableLoginPage();

    }

    async Task SignUpWithUsernameAndPassword(string username,string password) //register dgn username and password
    {
        try
        {
            await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username,password);
            Debug.Log("Sign Up Successful");
            IsUserSignedIn = true;
        }
        catch(AuthenticationException ex)
        {
            ShowErrorMessage(ex.Message);
        }
        catch(RequestFailedException ex)
        {
            ShowErrorMessage(ex.Message);
        }

    }

     async Task SignInWithUsernameAndPassword(string username, string password) //Login
    {
        try
        {
            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);
            Debug.Log("Sign In Successful");
            IsUserSignedIn = true;

            // Notify subscribers that sign-in is complete
            OnSignInComplete?.Invoke();
        }
        catch (AuthenticationException ex)
        {
            ShowErrorMessage(ex.Message);
        }
        catch (RequestFailedException ex)
        {
            ShowErrorMessage(ex.Message);
        }
    }

    public event System.Action OnSignInComplete;

    // Rest of your AuthManager code...


    public void ShowErrorMessage(string message)
    {
        errorMessageText.text = message;
        errorMessageText.gameObject.SetActive(true);
        Invoke("HideErrorMessage",displayErrorDuration);
    }

    private void HideErrorMessage()
    {
        errorMessageText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}