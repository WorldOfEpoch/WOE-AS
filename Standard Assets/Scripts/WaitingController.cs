// File path: Standard Assets/Scripts/WaitingController.cs

using UnityEngine;
using UnityEngine.UI;

public class WaitingController : MonoBehaviour
{
    public Text waitingText;
    public Button playButton;
    public GameController gameController;
    public PlayerManager playerManager;

    private void Start()
    {
        // Initialize waiting stage game state
        waitingText.text = "Waiting for players...";
    }

    public void OnPlayButtonClick()
    {
        // Check if player is in the queue
        if (playerManager.IsPlayerInQueue())
        {
            // Start the game
            gameController.StartGame();
        }
        else
        {
            // Display error message
            waitingText.text = "You are not in the queue!";
        }
    }
}