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
using System.Diagnostics;

public class TwitchChatController : MonoBehaviour
{
    // ... (rest of the script remains the same)

    private PlayerManager playerManager;

    void Start()
    {
        // Initialize PlayerManager
        playerManager = GetComponent<PlayerManager>();

        // ... (rest of the script remains the same)
    }

    void ProcessChatMessage(string message)
    {
        // ... (rest of the script remains the same)

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
}