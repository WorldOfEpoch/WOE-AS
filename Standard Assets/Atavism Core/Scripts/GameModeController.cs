using System.Collections.Generic;
using UnityEngine;
using Atavism.Api;
using Atavism;

public class GameModeController : MonoBehaviour
{
    private AtavismApi atavismApi;
    private string currentGameMode = "FreeForAll";
    private bool isGameInProgress = false;

    private void Start()
    {
        // Initialize Atavism API connection
        atavismApi = new AtavismApi("YOUR_API_KEY", "YOUR_API_SECRET");
    }

    public void SelectGameMode(string gameMode)
    {
        // Set the current game mode
        currentGameMode = gameMode;
    }

    public void StartGame()
    {
        // Start the game
        isGameInProgress = true;
        atavismApi.StartGame(currentGameMode);
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
        foreach (string username in atavismApi.GetPlayersInGame())
        {
            atavismApi.RewardPlayer(username, 100, 100, "gold");
        }
    }
}