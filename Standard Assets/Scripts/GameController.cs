// File path: Standard Assets/Scripts/GameController.cs

using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public PlayerManager playerManager;
    public TwitchChatController twitchChatController;

    private void Start()
    {
        // Initialize game state
        isGameInProgress = false;
    }

    public void StartGame()
    {
        // Check if there are enough players in the queue
        if (playerManager.GetPlayerQueueCount() < 2)
        {
            twitchChatController.SendMessage("Not enough players in the queue!");
            return;
        }

        // Start the game
        isGameInProgress = true;
        SceneManager.LoadScene("ArenaScene");
    }

    public void EndGame()
    {
        // End the game
        isGameInProgress = false;
        SceneManager.LoadScene("WaitingScene");
    }
}