using System;
using TMPro;
using Tyranny.Client.Events;
using Tyranny.Client.Network;
using Tyranny.Client.System;
using Tyranny.Networking;
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

        Debug.Log($"Status: {result.Status}");
        if (result.Status != AuthenticationStatus.Success)
        {
            Debug.Log("Failed to authenticate");
            return false;
        }

        Debug.Log($"Server: {result.Ip}:{result.Port}");
        Debug.Log($"Token:{result.Token.Length} => {BitConverter.ToString(result.Token).Replace("-", string.Empty)}");
        var worldClient = Registry.Get<WorldClient>();
        worldClient.Host = result.Ip;
        worldClient.Port = result.Port;
        
        Registry.Get<EventManager>().OnWorldAuth += OnWorldAuth;
        worldClient.Connect(username, result.Token);
        return true;
    }

    private void OnWorldAuth(object source, WorldAuthEventArgs args)
    {
        SceneManager.LoadScene("WorldScene");
        var readyPacket = new PacketWriter(TyrannyOpcode.GameReady);
        var worldClient = Registry.Get<WorldClient>();
        worldClient.Send(readyPacket);
    }
}
