using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using EmeraldAI;
using Random = UnityEngine.Random;

public class TwitchChatController : MonoBehaviour
{
    #region Variables

    [Header("Stage Settings")]
    [SerializeField] private bool waitStage;
    [SerializeField] private GameObject waitPanel;
    [SerializeField] private bool fightStage;
    [SerializeField] private GameObject fightPanel;
    [SerializeField] private bool rewardStage;
    [SerializeField] private GameObject rewardPanel;
    [SerializeField] private Text rewardPanelRanking;
    [SerializeField] private Text firstPlace;
    [SerializeField] private Text secondPlace;
    [SerializeField] private Text thirdPlace;

    [Header("Combatant Info Settings")]
    [SerializeField] private GameObject genericPlayerPrefab;
    [SerializeField] private List<SpawnPoint> unUsedSpawnPoints;
    [SerializeField] private List<SpawnPoint> usedSpawnPoints;
    [SerializeField] private Transform spawnPointParent;
    [SerializeField] private List<PlayerController> playerQueue;
    [SerializeField] private List<GameObject> instantiatedPlayerObjects;
    [SerializeField] private List<string> fighterList;
    [SerializeField] public List<PlayerController> playerRankings = new List<PlayerController>();

    [Header("Medieval/Fantasy Names")]
    private static readonly List<string> FantasyFirstNames = new List<string>
    {
        "Aragon", "Elowen", "Thorne", "Isolde", "Gandalf", "Faelan", "Eowyn", "Aurelia", "Lorelei", "Dorian",
        "Fenris", "Seraphina", "Dragomir", "Vesper", "Morgana", "Cassius", "Aurelius", "Sylas", "Elysia", "Caelum"
    };
    private static readonly List<string> FantasyLastNames = new List<string>
    {
        "Shadowblade", "Stormrider", "Fireheart", "Ironhelm", "Dragonrider", "Starfall", "Moonshadow", "Bloodthorn",
        "Blackthorn", "Dragonbane", "Frostbeard", "Stormbringer", "Bloodmoon", "Darkflame", "Shadowcaster", "Silverhand",
        "Stormcloak", "Frostborn", "Ravenwing", "Dreadwalker"
    };
    private bool listPopulated = false;

    [Header("Time Objects")]
    [SerializeField] private float waitTimeLeft = 30.0f;
    [SerializeField] private float rewardTimeLeft = 30.0f;
    [SerializeField] private float rewardReset = 0.1f;
    [SerializeField] private Text startText;
    [SerializeField] private Text rewardText;
    [SerializeField] private GameObject activateObject;
    [SerializeField] private GameObject mainCamera;
    [SerializeField] private float activateAt;

    [Header("Twitch Settings")]
    [SerializeField] private string username;
    [SerializeField] private string password;
    [SerializeField] private string channelName;
    [SerializeField] private Text chatBox;
    [SerializeField] private Text queueTextBox;
    [SerializeField] private Text fighterTextBox;

    private TcpClient twitchClient;
    private StreamReader reader;
    private StreamWriter writer;
    private HashSet<string> queueList = new HashSet<string>();

    public static TwitchChatController instance;

    #endregion

    #region Unity Methods

    private void Start()
    {
        instance = this;
        ConnectToTwitchChat();
        Database.singleton.Connect();
        InitializeSpawnPoints();
        InitializePlayerQueue();
    }

    private void Update()
    {
        if (!twitchClient.Connected)
        {
            ConnectToTwitchChat();
        }

        ReadTwitchChat();
        UpdateStages();
    }

    #endregion

    #region Twitch Chat

    private void ConnectToTwitchChat()
    {
        try
        {
            twitchClient = new TcpClient("irc.chat.twitch.tv", 6667);
            reader = new StreamReader(twitchClient.GetStream());
            writer = new StreamWriter(twitchClient.GetStream());
            writer.WriteLine("PASS " + password);
            writer.WriteLine("NICK " + username);
            writer.WriteLine("USER " + username + " 8 * :" + username);
            writer.WriteLine("JOIN #" + channelName);
            writer.Flush();
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to connect to Twitch chat: " + ex.Message);
        }
    }

    private void ReadTwitchChat()
    {
        if (twitchClient.Available > 0)
        {
            var message = reader.ReadLine();
            if (message.Contains("PRIVMSG"))
            {
                var splitPoint = message.IndexOf("!", 1);
                var chatName = message.Substring(0, splitPoint).Substring(1);
                splitPoint = message.IndexOf(":", 1);
                message = message.Substring(splitPoint + 1);
                Debug.Log($"{chatName}: {message}");
                ProcessChatInput(chatName, message);
            }
        }
    }

    private void ProcessChatInput(string username, string message)
    {
        if (message.Equals("play", StringComparison.OrdinalIgnoreCase))
        {
            if (!queueList.Contains(username) && !fighterList.Contains(username))
            {
                queueTextBox.text += $"\n{username}";
                queueList.Add(username);
            }
        }
        else if (message.Equals("betabomb", StringComparison.OrdinalIgnoreCase))
        {
            for (int i = 0; i < 20; i++)
            {
                string randomName = GenerateRandomFantasyName();
                queueList.Add(randomName);
            }
        }
    }

    #endregion

    #region Game Logic

    private void UpdateStages()
    {
        if (waitStage)
        {
            UpdateWaitStage();
        }
        else if (fightStage)
        {
            UpdateFightStage();
        }
        else if (rewardStage)
        {
            UpdateRewardStage();
        }
    }

    private void UpdateWaitStage()
    {
        SetPanelActive(waitPanel, true);
        SetPanelActive(fightPanel, false);
        SetPanelActive(rewardPanel, false);

        if (queueList.Count > 0)
        {
            int randomIndex = Random.Range(0, unUsedSpawnPoints.Count);
            SpawnPlayer(randomIndex);
        }

        StartCoroutine(EmptyQueueList());

        waitTimeLeft -= Time.deltaTime;
        startText.text = waitTimeLeft.ToString("0");

        if (waitTimeLeft < activateAt)
        {
            activateObject.SetActive(true);
        }

        if (waitTimeLeft <= 0)
        {
            if (fighterList.Count > 1)
            {
                SetStage(false, true, false);
            }
            else
            {
                waitTimeLeft = 60;
            }
        }
    }

    private void UpdateFightStage()
    {
        SetPanelActive(waitPanel, false);
        SetPanelActive(fightPanel, true);
        SetPanelActive(rewardPanel, false);

        int alivePlayers = 0;

        foreach (var playerObject in instantiatedPlayerObjects)
        {
            var playerController = playerObject.GetComponent<PlayerController>();
            var emeraldAI = playerObject.GetComponent<EmeraldAISystem>();

            if (!playerController.componentsEnabled)
            {
                EnablePlayerComponents(playerObject);
            }

            if (emeraldAI.CurrentHealth > 0)
            {
                alivePlayers++;
            }
        }

        listPopulated = false;

        if (alivePlayers == 1)
        {
            SetStage(false, false, true);
            waitTimeLeft = 60f;
            rewardTimeLeft = 60f;
            listPopulated = false;
        }
    }

    private void UpdateRewardStage()
    {
        SetPanelActive(rewardPanel, true);
        SetPanelActive(fightPanel, false);
        SetPanelActive(waitPanel, false);

        rewardTimeLeft -= Time.deltaTime;
        rewardText.text = rewardTimeLeft.ToString("0");

        DisplayRewardText();

        if (rewardTimeLeft < rewardReset)
        {
            ResetGame();
        }
    }

    private void SetStage(bool wait, bool fight, bool reward)
    {
        waitStage = wait;
        fightStage = fight;
        rewardStage = reward;
    }

    private void SetPanelActive(GameObject panel, bool active)
    {
        panel.SetActive(active);
    }

    private void SpawnPlayer(int spawnIndex)
    {
        var spawnPoint = unUsedSpawnPoints[spawnIndex];
        var playerObject = Instantiate(genericPlayerPrefab, spawnPoint.transform.position, Quaternion.identity);
        var playerController = playerObject.GetComponent<PlayerController>();
        var emeraldAI = playerObject.GetComponent<EmeraldAISystem>();

        playerController.playerName = queueList.First();
        instantiatedPlayerObjects.Add(playerObject);
        emeraldAI.AIName = playerController.playerName;

        if (Database.singleton.CharacterExists(playerController.playerName))
        {
            LoadPlayerData(playerController, emeraldAI);
        }

        DisablePlayerComponents(playerObject);

        string tempName = playerController.playerName;
        fighterList.Add(tempName);
        fighterTextBox.text += $"\n{tempName}";
        queueList.Remove(tempName);

        usedSpawnPoints.Add(spawnPoint);
        unUsedSpawnPoints.RemoveAt(spawnIndex);
    }

    private void EnablePlayerComponents(GameObject playerObject)
    {
        var playerController = playerObject.GetComponent<PlayerController>();
        var emeraldAI = playerObject.GetComponent<EmeraldAISystem>();
        var emeraldAIBehaviors = playerObject.GetComponent<EmeraldAIBehaviors>();
        var navMeshAgent = playerObject.GetComponent<NavMeshAgent>();

        emeraldAI.enabled = true;
        emeraldAIBehaviors.enabled = true;
        emeraldAI.AIAgentActive = true;
        navMeshAgent.enabled = true;
        playerController.componentsEnabled = true;

        foreach (var component in playerObject.GetComponents<MonoBehaviour>())
        {
            component.enabled = true;
        }
    }

    private void DisablePlayerComponents(GameObject playerObject)
    {
        var emeraldAI = playerObject.GetComponent<EmeraldAISystem>();
        var emeraldAIBehaviors = playerObject.GetComponent<EmeraldAIBehaviors>();
        var navMeshAgent = playerObject.GetComponent<NavMeshAgent>();

        emeraldAI.AIAnimator.Play("Locomotion");
        emeraldAIBehaviors.enabled = false;
        emeraldAI.AIAgentActive = false;
        emeraldAI.enabled = false;
        navMeshAgent.enabled = false;

        foreach (var component in playerObject.GetComponents<MonoBehaviour>())
        {
            component.enabled = false;
        }
    }

    private void LoadPlayerData(PlayerController playerController, EmeraldAISystem emeraldAI)
    {
        var playerData = Database.singleton.CharacterLoad(playerController.playerName);
        int damage = (int)playerData[1];
        int health = (int)playerData[0];

        emeraldAI.MeleeAttacks[0].MinDamage = damage;
        emeraldAI.MeleeAttacks[0].MaxDamage = damage;
        emeraldAI.MeleeAttacks[1].MinDamage = damage;
        emeraldAI.MeleeAttacks[1].MaxDamage = damage;
        emeraldAI.StartingHealth = health;
        emeraldAI.CurrentHealth = health;
        playerController.health = health;
    }

    private void DisplayRewardText()
    {
        rewardPanelRanking.text = "";
        firstPlace.text = "";
        secondPlace.text = "";
        thirdPlace.text = "";

        if (!listPopulated)
        {
            playerRankings.Reverse();
            listPopulated = true;
        }

        for (int i = 0; i < playerRankings.Count; i++)
        {
            var playerName = playerRankings[i].playerName;
            var playerKills = playerRankings[i].matchkills;


            if (i == 0)
            {
                firstPlace.text += $"{playerName}: {playerKills}\n";
            }
            else if (i == 1)
            {
                secondPlace.text += $"{playerName}: {playerKills}\n";
            }
            else if (i == 2)
            {
                thirdPlace.text += $"{playerName}: {playerKills}\n";
            }
            else
            {
                rewardPanelRanking.text += $"{playerName}: {playerKills}\n";
            }
        }
    }

    private void ResetGame()
    {
        foreach (var playerObject in instantiatedPlayerObjects)
        {
            Destroy(playerObject);
        }

        foreach (var playerRanking in playerRankings)
        {
            Database.singleton.CharacterSave(playerRanking.playerName, playerRanking.matchkills);
        }

        waitTimeLeft = 60.0f;
        instantiatedPlayerObjects.Clear();
        unUsedSpawnPoints.Clear();
        usedSpawnPoints.Clear();
        fighterList.Clear();
        fighterTextBox.text = "";

        InitializeSpawnPoints();

        SetStage(true, false, false);
    }

    private void InitializeSpawnPoints()
    {
        for (int i = 0; i < spawnPointParent.childCount; i++)
        {
            unUsedSpawnPoints.Add(spawnPointParent.GetChild(i).GetComponent<SpawnPoint>());
        }
    }

    private void InitializePlayerQueue()
    {
        playerQueue = new List<PlayerController>(unUsedSpawnPoints.Count);
        fighterList = new List<string>();
    }

    private string GenerateRandomFantasyName()
    {
        string randomFirstName = FantasyFirstNames[Random.Range(0, FantasyFirstNames.Count)];
        string randomLastName = FantasyLastNames[Random.Range(0, FantasyLastNames.Count)];
        return $"{randomFirstName} {randomLastName}";
    }

    #endregion

    #region Coroutines

    private IEnumerator EmptyQueueList()
    {
        string tempString = "";
        var tempStringList = queueList.ToList();

        foreach (var name in tempStringList)
        {
            tempString += $"\n{name}";
            yield return null;
        }

        queueTextBox.text = tempString;
    }

    #endregion
}