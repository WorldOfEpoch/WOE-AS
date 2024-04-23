// File path: Standard Assets/Scripts/TwitchChatController.cs

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TwitchLib.Api;
using TwitchLib.Api.V5.Models.Channels;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using UnityEngine.UI;
using System;

public class TwitchChatController : MonoBehaviour
{
    public string twitchChannelName = "epocharena";
    public string twitchOAuthToken = "YOUR_OAUTH_TOKEN";
    public string twitchClientId = "YOUR_CLIENT_ID";
    public string twitchClientSecret = "YOUR_CLIENT_SECRET";

    private TwitchClient twitchClient;
    private PlayerManager playerManager;
    private TwitchIRCConnector twitchIRCConnector;

    private Text chatLogText;
    private InputField userInputField;

    private bool isPlayerInQueue = false;
    private bool isGameInProgress = false;

    void Start()
    {
        // Initialize Twitch API connection
        twitchClient = new TwitchClient(twitchClientId, twitchClientId, twitchOAuthToken);
        twitchClient.OnMessageReceived += OnMessageReceived;
        twitchClient.Connect();

        // Initialize chat log text
        chatLogText.text = "";
    }

    void Update()
    {
        // Process chat messages
        while (twitchClient.MessageQueue.Count > 0)
        {
            string message = twitchClient.MessageQueue.Dequeue();
            ProcessChatMessage(message);
        }
    }

    void OnMessageReceived(object sender, OnMessageReceivedArgs e)
    {
        // Add message to queue
        twitchClient.MessageQueue.Enqueue(e.Message.ChatMessage.Message);
    }

    void ProcessChatMessage(string message)
    {
        // Split message into command and arguments
        string[] parts = message.Split(' ');
        string command = parts[0].ToLower();
        string[] args = new string[parts.Length - 1];
        Array.Copy(parts, 1, args, 0, parts.Length - 1);

        // Handle commands
        switch (command)
        {
            case "!play":
                HandlePlayCommand(args);
                break;
            case "!createaccount":
                HandleCreateAccountCommand(args);
                break;
            case "!createplayer":
                HandleCreatePlayerCommand(args);
                break;
            default:
                // Unknown command
                Debug.Log("Unknown command: " + command);
                break;
        }
    }

    void HandlePlayCommand(string[] args)
    {
        // Check if player is already in queue
        if (isPlayerInQueue)
        {
            chatLogText.text += "You are already in the queue!\n";
            return;
        }

        // Check if player has an account and character
        if (!playerManager.HasAccount(args[0]))
        {
            chatLogText.text += "You don't have an account! Type !createaccount to create one.\n";
            return;
        }

        if (!playerManager.HasCharacter(args[0]))
        {
            chatLogText.text += "You don't have a character! Type !createplayer to create one.\n";
            return;
        }

        // Add player to queue
        playerManager.AddToQueue(args[0]);
        isPlayerInQueue = true;
        chatLogText.text += "You have been added to the queue!\n";
    }

    void HandleCreateAccountCommand(string[] args)
    {
        playerManager.CreateAccount(args[0], args[1]);
        chatLogText.text += "Account created! Please type !createplayer to create a character.\n";
    }

    void HandleCreatePlayerCommand(string[] args)
    {
        playerManager.CreateCharacter(args[0], args[1], args[2]);
        chatLogText.text += "Character created! You can now type !play to join the queue.\n";
    }

    public void OnUserInput(string message)
    {
        twitchIRCConnector.SendIRCMessage("PRIVMSG #" + twitchChannelName + " :" + message);
    }
}