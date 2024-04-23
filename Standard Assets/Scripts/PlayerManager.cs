// File path: Assets/Scripts/Managers/PlayerManager.cs

using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    // Player database
    private Dictionary<string, PlayerAccount> playerAccounts = new Dictionary<string, PlayerAccount>();

    // Player queue
    private Queue<PlayerAccount> playerQueue = new Queue<PlayerAccount>();

    public void CreateAccount(string username, string email)
    {
        // TO DO: implement account creation logic
        // For now, just create a new account
        PlayerAccount account = new PlayerAccount(username, email);
        playerAccounts.Add(username, account);
    }

    public void CreateCharacter(string username, string characterName, string characterType)
    {
        // TO DO: implement character creation logic
        // For now, just create a new character
        PlayerAccount account = playerAccounts[username];
        account.CreateCharacter(characterName, characterType);
    }

    public void AddToQueue(string username)
    {
        PlayerAccount account = playerAccounts[username];
        playerQueue.Enqueue(account);
    }

    public PlayerAccount GetNextPlayerInQueue()
    {
        return playerQueue.Dequeue();
    }
}

public class PlayerAccount
{
    public string Username { get; set; }
    public string Email { get; set; }
    public List<Character> Characters { get; set; }

    public PlayerAccount(string username, string email)
    {
        Username = username;
        Email = email;
        Characters = new List<Character>();
    }

    public void CreateCharacter(string characterName, string characterType)
    {
        Character character = new Character(characterName, characterType);
        Characters.Add(character);
    }
}

public class Character
{
    public string Name { get; set; }
    public string Type { get; set; }

    public Character(string name, string type)
    {
        Name = name;
        Type = type;
    }
}