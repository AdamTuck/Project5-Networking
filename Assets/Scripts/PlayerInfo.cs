using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using TMPro;

public class PlayerInfo : NetworkBehaviour
{
    [SerializeField] private TMP_Text txtPlayerName;

    public NetworkVariable<FixedString64Bytes> playerName = new NetworkVariable<FixedString64Bytes>(
        new FixedString64Bytes("Player Name"), 
        NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Owner);

    public override void OnNetworkSpawn()
    {
        playerName.OnValueChanged += OnNameChanged;

        //txtPlayerName.SetText(playerName.Value.ToString());
        //txtPlayerName.text = playerName.Value.ToString();
        txtPlayerName.text = playerName.Value.ToString();
        Debug.Log($"Player name set to(OnNetworkSpawn): {txtPlayerName.text}");
        gameObject.name = "Player_" + playerName.Value.ToString();
        Debug.Log("Gameobject Name: " + gameObject.name);

        if (IsLocalPlayer)
            GameManager.instance.SetLocalPlayer(NetworkObject);

        GameManager.instance.OnPlayerJoined(NetworkObject);
    }

    public override void OnNetworkDespawn()
    {
        playerName.OnValueChanged -= OnNameChanged;
    }

    void OnNameChanged (FixedString64Bytes prevVal, FixedString64Bytes newVal)
    {
        //Debug.Log($"OnNameChanged Fired (prev, new): {prevVal}, {newVal}");
        if (newVal != prevVal)
        {
//            Debug.Log($"Setting player name: {newVal.Value}");
            //txtPlayerName.SetText(newVal.Value);
            txtPlayerName.text = newVal.Value.ToString();
            //txtPlayerName.SetText("TEST");
            GameManager.instance.SetPlayerName(NetworkObject, newVal.Value.ToString());
            Debug.Log($"Player name set to: {txtPlayerName.text}, {newVal.Value.ToString()}");
        }
    }

    public void SetName(string name)
    {
        playerName.Value = new FixedString64Bytes(name);
    }
}