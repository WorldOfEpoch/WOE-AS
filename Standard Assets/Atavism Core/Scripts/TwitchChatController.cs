using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Atavism;
using System.Linq;
using UnityEditor.PackageManager;

/* //commented out until later

public class TwitchChatController : MonoBehaviour
{
    public string twitchUsername;
    public string twitchOAuthToken;
    public string twitchChannelName;

    public GameObject panel1;
    public GameObject panel2;
    public float panelShowDuration = 5f;
    public float gameStartDelay = 3f;

    private Client client;
    private Queue<string> playerQueue = new Queue<string>();
    private string currentPlayer;
    private bool isGameStarted = false;

    private void Start()
    {
        Application.runInBackground = true;
        Connect();
    }

    private void Connect()
    {
        client = new Client();
        client.Initialize(new ConnectionCredentials(twitchUsername, twitchOAuthToken), twitchChannelName);

        client.OnConnected += OnConnected;
        client.OnMessageReceived += OnMessageReceived;

        client.Connect();
    }

    private void OnConnected(object sender, TwitchLib.Client.Events.OnConnectedArgs e)
    {
        Debug.Log("Connected to Twitch chat!");
    }

    private void OnMessageReceived(object sender, TwitchLib.Client.Events.OnMessageReceivedArgs e)
    {
        if (e.ChatMessage.Message.StartsWith("!play"))
        {
            string playerName = e.ChatMessage.Username;

            // Check if the player has a game character in the Atavism database
            if (AtavismClient.Instance.WorldManager.CharacterExistsForAccount(playerName))
            {
                // Add the player to the queue
                playerQueue.Enqueue(playerName);
                client.SendMessage(twitchChannelName, $"{playerName} has been added to the queue!");
            }
            else
            {
                // Send a message with the registration link
                client.SendMessage(twitchChannelName, $"{playerName}, you need to create a game character first. Please visit [registration link] to register and create a character.");
            }
        }
        else if (e.ChatMessage.Message.StartsWith("!queue"))
        {
            string playerName = e.ChatMessage.Username;

            if (playerQueue.Contains(playerName))
            {
                int position = playerQueue.ToArray().ToList().IndexOf(playerName) + 1;
                client.SendMessage(twitchChannelName, $"{playerName}, you are currently at position {position} in the queue.");
            }
            else
            {
                client.SendMessage(twitchChannelName, $"{playerName}, you are not currently in the queue. Type !play to join the queue.");
            }
        }
    }

    private void Update()
    {
        if (!isGameStarted && playerQueue.Count > 0)
        {
            currentPlayer = playerQueue.Dequeue();
            StartCoroutine(StartGame());
        }
    }

    private IEnumerator StartGame()
    {
        isGameStarted = true;

        // Show panel 1
        panel1.SetActive(true);
        yield return new WaitForSeconds(panelShowDuration);
        panel1.SetActive(false);

        // Show panel 2
        panel2.SetActive(true);
        yield return new WaitForSeconds(panelShowDuration);
        panel2.SetActive(false);

        // Wait for game start delay
        yield return new WaitForSeconds(gameStartDelay);

        // Start the game for the current player
        client.SendMessage(twitchChannelName, $"Game started for {currentPlayer}!");

        // Wait for the game to finish (replace this with your actual game logic)
        yield return new WaitForSeconds(10f); // Simulating game duration

        // Determine the winner (replace this with your actual game logic)
        bool isPlayerWinner = Random.value < 0.5f; // Simulating a random winner

        if (isPlayerWinner)
        {
            client.SendMessage(twitchChannelName, $"{currentPlayer} wins the game!");
            // Add any additional logic for handling the winner
        }
        else
        {
            client.SendMessage(twitchChannelName, $"{currentPlayer} loses the game!");
            // Add any additional logic for handling the loser
        }

        // Reset the game state
        isGameStarted = false;
        currentPlayer = null;
    }

    private void OnDestroy()
    {
        client.Disconnect();
    }
}
*/ //commented out until later