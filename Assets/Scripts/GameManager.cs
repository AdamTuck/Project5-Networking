using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class GameManager : NetworkBehaviour
{
    public static GameManager instance;

    void Singleton ()
    {
        if (instance && instance != this)
        {
            Destroy(instance);
        }

        instance = this;
    }

    [SerializeField] TMP_InputField playerNameField;

    private NetworkObject localPlayerObj;

    public NetworkVariable<short> state = new NetworkVariable<short>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    Dictionary<ulong, string> playerNames = new Dictionary<ulong, string>();
    Dictionary<ulong, int> playerScores = new Dictionary<ulong, int>();

    [SerializeField] private Transform[] startPositions;

    [Header("Game Properties")]
    [SerializeField] private int scoreToWin;

    [Header("UI Elements")]
    [SerializeField] private GameObject endGameScreen;
    [SerializeField] private TMP_Text endGameMessage;
    [SerializeField] private GameObject scoreUI;

    

    private void Awake()
    {
        Singleton();

        if (IsServer)
            state.Value = 0;
    }

    public void SetLocalPlayer (NetworkObject _localPlayer)
    {
        localPlayerObj = _localPlayer;

        if (playerNameField.text.Length > 0)
            localPlayerObj.GetComponent<PlayerInfo>().SetName(playerNameField.text);
        else
            localPlayerObj.GetComponent<PlayerInfo>().SetName($"Player-{localPlayerObj.OwnerClientId}");

        playerNameField.gameObject.SetActive(false);
    }

    public void SetPlayerName(NetworkObject playerObj, string name)
    {
        if (playerNames.ContainsKey(playerObj.OwnerClientId))
            playerNames[playerObj.OwnerClientId] = name;
        else
            playerNames.Add(playerObj.OwnerClientId, name);
    }

    public void OnPlayerJoined(NetworkObject playerObj)
    {
        // Assign player a position based on the ID.
        playerObj.transform.position = startPositions[(int)playerObj.OwnerClientId].position;

        // Set the player score to 0
        playerScores.Add(playerObj.OwnerClientId, 0);
    }

    public void StartGame ()
    {
        state.Value = 1;
    }

    public void AddScore(ulong playerId)
    {
        if (IsServer)
        {
            playerScores[playerId]++;
            // Display score here
            CheckWinner(playerId);
        }
    }

    void CheckWinner (ulong playerId)
    {
        if (playerScores[playerId] >= scoreToWin)
        {
            EndGame(playerId);
        }
    }

    public void EndGame (ulong winnerId)
    {
        if (IsServer)
        {
            endGameScreen.SetActive(true);
            
            if (winnerId == NetworkManager.LocalClientId)
                endGameMessage.text = "YOU WIN";
            else
                endGameMessage.text = $"YOU LOSE\nWinner: {playerNames[winnerId]}";

            ScoreInfo tempInfo = new ScoreInfo();
            tempInfo.score = playerScores[winnerId];
            tempInfo.id = winnerId;
            tempInfo.name = playerNames[winnerId];

            ShowGameEndUIClientRPC(JsonUtility.ToJson(tempInfo));
        }
    }

    [ClientRpc]
    public void ShowGameEndUIClientRPC(string winnerInfo)
    {
        endGameScreen.SetActive(true);

        ScoreInfo info = JsonUtility.FromJson<ScoreInfo>(winnerInfo);

        if (info.id == NetworkManager.LocalClientId)
            endGameMessage.text = "YOU WIN";
        else
            endGameMessage.text = $"YOU LOSE\nWinner: {info.name}";
    }
}

[System.Serializable]
public class ScoreInfo
{
    public ulong id;
    public string name;
    public int score;
}