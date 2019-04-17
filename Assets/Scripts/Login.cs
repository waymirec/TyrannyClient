using System;
using TMPro;
using Tyranny.Client.Events;
using Tyranny.Client.Network;
using Tyranny.Client.System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField usernameInputField;

    [SerializeField]
    private TMP_InputField passwordInputField;

    [SerializeField]
    private Text statusText;

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
        bool result = DoLogin(usernameInputField.text, passwordInputField.text);
        if (!result)
        {
            statusText.text = "Login failed.";
            Debug.Log("Failed to authenticate");
            return;
        }
    }

    private bool DoLogin(string username, string password)
    {
        Configuration configuration = Registry.Configuration;
        AuthenticationClient authenticationClient = new AuthenticationClient(configuration.AuthenticationServer.Address, configuration.AuthenticationServer.Port);
        AuthenticationResult result = authenticationClient.authenticate(username, password);

        Debug.Log($"Status: {result.Status}");
        if (result.Status != AuthenticationStatus.Success)
        {
            Debug.Log("Failed to authenticate");
            return false;
        }

        Debug.Log($"Server: {result.Ip}:{result.Port}");
        Debug.Log($"Token:{result.Token.Length} => {BitConverter.ToString(result.Token).Replace("-", string.Empty)}");
        WorldClient worldClient = Registry.Get<WorldClient>();
        worldClient.Host = "192.168.0.127";
        worldClient.Port = 13579;
        Registry.Get<EventManager>().OnLoggedIn += OnLoggedIn;
        worldClient.Connect(username, result.Token);
        return true;
    }

    public void OnLoggedIn(object source, LoginEventArgs args)
    {
        SceneManager.LoadScene("WorldScene");
    }
}
