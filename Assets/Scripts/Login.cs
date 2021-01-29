using System;
using Events;
using Managers;
using TMPro;
using Network;
using UnityEngine;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField usernameInputField;

    [SerializeField]
    private TMP_InputField passwordInputField;

    [SerializeField]
    private Text statusText;

    private NetworkEventManager _networkEventManager;
    
    private void Awake()
    {
        _networkEventManager = Registry.Get<NetworkEventManager>();
    }
    
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (usernameInputField.isFocused)
                passwordInputField.Select();
            else
                usernameInputField.Select();
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            OnClick();
        }
    }

    public void OnClick()
    {
        Debug.Log($"Username: {usernameInputField.text}");
        Debug.Log($"Password: {passwordInputField.text}");
        Authenticate();
    }

    private void Authenticate()
    {
        statusText.text = "Authenticating...";
        var result = DoLogin(usernameInputField.text, passwordInputField.text);
        if (!result)
        {
            statusText.text = "Login failed.";
            Debug.Log("Failed to authenticate");
        }
    }

    private bool DoLogin(string username, string password)
    {
        var configuration = Registry.Configuration;
        var authenticationClient = new AuthenticationClient(configuration.AuthenticationServer.Address, configuration.AuthenticationServer.Port);
        var result = authenticationClient.Authenticate(username, password);
        if (result.Status != AuthenticationStatus.Success)
        {
            Debug.Log("Failed to authenticate");
            return false;
        }

        _networkEventManager.FireEvent_OnLoginAuth(this, new NetworkLoginAuthEventArgs
        {
            Username = username,
            Token = result.Token,
            WorldHost = result.Ip,
            WorldPort = result.Port
        });
        
        return true;
    }
}
