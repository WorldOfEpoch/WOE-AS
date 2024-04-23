using UnityEngine;
using Atavism.Api;
using Atavism;

public class ArenaSceneController : MonoBehaviour
{
    private AtavismApi atavismApi;
    private string currentGameMode = "FreeForAll";

    private void Start()
    {
        // Initialize Atavism API connection
        atavismApi = new AtavismApi("YOUR_API_KEY", "YOUR_API_SECRET");
    }

    public void InitializeArenaScene(string gameMode)
    {
        // Set the current game mode
        currentGameMode = gameMode;
    }

    public void SpawnPlayer(string username)
    {
        // Use Atavism's API to spawn the player
        atavismApi.SpawnPlayer(username, currentGameMode);
    }

    public void UpdateArenaScene()
    {
        // Update the arena scene logic
        atavismApi.UpdateArenaScene(currentGameMode);
    }
}