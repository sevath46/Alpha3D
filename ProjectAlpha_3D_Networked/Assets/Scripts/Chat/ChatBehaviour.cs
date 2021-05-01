using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class ChatBehaviour : NetworkBehaviour
{
    [SerializeField] private Text chatText;
    [SerializeField] private InputField inputField;
    [SerializeField] private GameObject canvas;

    //? What is event? 
    private static event Action<string> OnMessage;


    //Called when the client connects to the server.
    public override void OnStartAuthority() 
    {
        //Chatbox turns true
        canvas.SetActive(true);

        //? What does this do?
        OnMessage += HandleNewMessage;
    }

    //Called when a client exits the server.
    //? What is ClientCallback?
    [ClientCallback]
    private void OnDestroy() 
    {
        //If the player does not have authority do nothing.
        if (!hasAuthority) 
        {
            return;
        }

        //? Otherwise ?
        OnMessage -= HandleNewMessage;
    }

    //When a new message is added, update the scrolll view text to include the message.
    private void HandleNewMessage(string message) 
    {
        //Text equals addition of message.
        chatText.text += message;
    }

    //When a client hits enter, send the message.
    [Client]
    public void Send() 
    {
        //If any key but enter is pressed
        if (!Input.GetKeyDown(KeyCode.Return)) 
        {
            //Do nothing.
            return;
        }
        //If the InputField is blank or not selected.
        if (string.IsNullOrWhiteSpace(inputField.text)) 
        {
            //Do nothing.
            return;
        }
        //Run CmdSendMessage with the value of the text in inputfield.
        CmdSendMessage(inputField.text);
        inputField.text = string.Empty;
    }

    //Run the function on the server when called
    [Command]
    private void CmdSendMessage(string message) 
    {
        //Validate the message
        //! Replace connectioonToClient.connectionId with local player name.
        RpcHandleMessage($"[{connectionToClient.connectionId}]: {message}");
    }

    //Run the function on all clients when called
    [ClientRpc]
    private void RpcHandleMessage(string message) 
    {
        OnMessage?.Invoke($"\n{message}");
    }

    //If user presses send button

    public void ButtonClick() 
    {
        //If the InputField is blank or not selected.
        if (string.IsNullOrWhiteSpace(inputField.text))
        {
            //Do nothing.
            return;
        }
        //Run CmdSendMessage with the value of the text in inputfield.
        CmdSendMessage(inputField.text);
        inputField.text = string.Empty;
    }
}
