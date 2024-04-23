using System.Collections.Generic;
using UnityEngine;
using TwitchLib.Api;
using TwitchLib.Api.V5.Models.Channels;

public class TwitchChatController : MonoBehaviour
{
    private Dictionary<string, string> userAccounts = new Dictionary<string, string>();
    private Dictionary<string, string> userCharacters = new Dictionary<string, string>();
    private List<string> playerQueue = new List<string>();

    private void Start()
    {
        // Initialize Twitch API connection
        // ...
    }

    private void HandleChatMessage(string message)
    {
        string username = GetUsernameFromMessage(message);
        string command = GetCommandFromMessage(message);

        if (command.StartsWith("!play"))
        {
            if (HasAccountAndCharacter(username))
            {
                AddToPlayerQueue(username);
            }
            else
            {
                SendChatMessage("Please create an account or download the app to play!");
            }
        }
        else if (command.StartsWith("!createaccount"))
        {
            CreateAccount(username, message.Split(' ')[1], message.Split(' ')[2]);
        }
        else if (command.StartsWith("!createplayer"))
        {
            CreatePlayer(username, message.Split(' ')[1], message.Split(' ')[2]);
        }
    }

    private string GetUsernameFromMessage(string message)
    {
        // TO DO: Implement username extraction logic
        return "username"; // placeholder
    }

    private string GetCommandFromMessage(string message)
    {
        // TO DO: Implement command extraction logic
        return "!play"; // placeholder
    }

    private bool HasAccountAndCharacter(string username)
    {
        return userAccounts.ContainsKey(username) && userCharacters.ContainsKey(username);
    }

    private void AddToPlayerQueue(string username)
    {
        playerQueue.Add(username);
        SendChatMessage("You have been added to the player queue!");
    }

    private void CreateAccount(string username, string email)
    {
        if (!userAccounts.ContainsKey(username))
        {
            userAccounts.Add(username, email);
            SendChatMessage("Account created successfully!");
        }
        else
        {
            SendChatMessage("Account already exists!");
        }
    }

    private void CreatePlayer(string username, string race, string gender)
    {
        if (userAccounts.ContainsKey(username) && !userCharacters.ContainsKey(username))
        {
            userCharacters.Add(username, $"{race} {gender}");
            SendChatMessage("Player created successfully!");
        }
        else
        {
            SendChatMessage("You need to create an account first or you already have a player!");
        }
    }

    private void SendChatMessage(string message)
    {
        // TO DO: Implement chat message sending logic
    }
}