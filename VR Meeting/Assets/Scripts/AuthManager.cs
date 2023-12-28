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

    public async void Create()
    {
        string usernameText = usernameInput.text;
        string passwordText = passwordInput.text;
        await SignUpWithUsernameAndPassword(usernameText,passwordText);

    }

    public async void SignIn()
    {
        string usernameText = usernameInput.text;
        string passwordText = passwordInput.text;
        await SignInWithUsernameAndPassword(usernameText,passwordText);
    }

    /*public async void SignOut(){
        await SignOutOfGame();
    }

    async Task SignOutOfGame()
    {
        try
        {
            AuthenticationService.Instance.SignOut(true);
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

    }*/

    async Task SignUpWithUsernameAndPassword(string username,string password)
    {
        try
        {
            await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username,password);
            Debug.Log("Sign Up Successful");
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

    async Task SignInWithUsernameAndPassword(string username,string password)
    {
        try
        {
            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username,password);
            Debug.Log("Sign In Successful");
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
