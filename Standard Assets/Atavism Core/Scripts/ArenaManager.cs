using System.Collections.Generic;
using UnityEngine;
using Atavism.Api;
using Atavism;

public class ArenaManager : MonoBehaviour
{
    private AtavismApi atavismApi;
    private List<string> playerQueue = new List<string>();
    private string currentGameMode = "FreeForAll";
    private bool isGameInProgress = false;

    private void Start()
    {
        // Initialize Atavism API connection
        atavismApi = new AtavismApi("YOUR_API_KEY", "YOUR_API_SECRET");
    }

    public void AddToPlayerQueue(string username)
    {
        // Add user to player queue
        playerQueue.Add(username);
        atavismApi.AddToPlayerQueue(username);
    }

    public void StartGame()
    {
        // Check if there are enough players in the queue
        if (playerQueue.Count >= 2)
        {
            // Select a random game mode
            currentGameMode = GetRandomGameMode();

            // Start the game
            isGameInProgress = true;
            atavismApi.StartGame(currentGameMode, playerQueue);
        }
        else
        {
            Debug.Log("Not enough players in the queue.");
        }
    }

    private string GetRandomGameMode()
    {
        // TO DO: Implement game mode selection logic
        return "FreeForAll"; // placeholder
    }

    public void UpdateGameState()
    {
        // Update game state logic
        if (isGameInProgress)
        {
            // Check if the game is over
            if (atavismApi.IsGameFinished())
            {
                // Reward players and reset the game state
                RewardPlayers();
                isGameInProgress = false;
            }
        }
    }

    private void RewardPlayers()
    {
        // Reward players with gold, experience, and items
        foreach (string username in playerQueue)
        {
            atavismApi.RewardPlayer(username, 100, 100, "gold");
        }
    }
}