using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TwitchLib.Api;
using TwitchLib.Api.V5.Models.Channels;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;
using System.Diagnostics;
using System;

public class TwitchChatController : MonoBehaviour
{
    // Twitch API credentials
    public string twitchChannelName = "epocharena";
    public string twitchOAuthToken = "YOUR_OAUTH_TOKEN";
    public string twitchClientId = "YOUR_CLIENT_ID";
    public string twitchClientSecret = "YOUR_CLIENT_SECRET";

    // Twitch API connection
    private TwitchClient twitchClient;

    // Chat message queue
    private Queue<string> chatMessageQueue = new Queue<string>();

    // UI elements
    public Text chatLogText;
    public InputField userInputField;

    // Game state
    private bool isPlayerInQueue = false;
    private bool isGameInProgress = false;

    void Start()
    {
        // Initialize Twitch API connection
        twitchClient = new TwitchClient(twitchClientId, twitchClientId, twitchOAuthToken);
        twitchClient.OnMessageReceived += OnMessageReceived;
        twitchClient.OnConnected += OnConnected;
        twitchClient.Connect();

        // Initialize chat log text
        chatLogText.text = "";
    }

    void Update()
    {
        // Process chat messages
        while (chatMessageQueue.Count > 0)
        {
            string message = chatMessageQueue.Dequeue();
            ProcessChatMessage(message);
        }
    }

    void OnConnected(object sender, OnConnectedArgs e)
    {
        Debug.Log("Connected to Twitch chat!");
    }

    void OnMessageReceived(object sender, OnMessageReceivedArgs e)
    {
        // Add message to queue
        chatMessageQueue.Enqueue(e.Message.ChatMessage.Message);
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
        // TO DO: implement account and character checks

        // Add player to queue
        isPlayerInQueue = true;
        chatLogText.text += "You have been added to the queue!\n";
    }

    void HandleCreateAccountCommand(string[] args)
    {
        // Create new account
        // TO DO: implement account creation

        chatLogText.text += "Account created! Please type !createplayer to create a character.\n";
    }

    void HandleCreatePlayerCommand(string[] args)
    {
        // Create new character
        // TO DO: implement character creation

        chatLogText.text += "Character created! You can now type !play to join the queue.\n";
    }

    public void SendMessage(string message)
    {
        twitchClient.SendMessage(twitchChannelName, message);
    }

    public void OnUserInput(string message)
    {
        SendMessage(message);
    }
}
