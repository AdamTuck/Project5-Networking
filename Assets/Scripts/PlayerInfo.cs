using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using TMPro;

public class PlayerInfo : NetworkBehaviour
{
    [SerializeField] private TMP_Text txtPlayerName;

    public NetworkVariable<FixedString32Bytes> playerName = new NetworkVariable<FixedString32Bytes>(new FixedString32Bytes("Player Name"), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public override void OnNetworkSpawn()
    {
        playerName.OnValueChanged += OnNameChanged;

        txtPlayerName.SetText(playerName.Value.ToString());
        gameObject.name = "Player_" + playerName.Value.ToString();

        if (IsLocalPlayer)
            GameManager.instance.SetLocalPlayer(NetworkObject);

        GameManager.instance.OnPlayerJoined(NetworkObject);
    }

    public override void OnNetworkDespawn()
    {
        playerName.OnValueChanged -= OnNameChanged;
    }

    void OnNameChanged (FixedString32Bytes prevVal, FixedString32Bytes newVal)
    {
        if (newVal != prevVal)
        {
            txtPlayerName.SetText(newVal.Value);
            GameManager.instance.SetPlayerName(NetworkObject, newVal.ToString());
        }
    }

    public void SetName(string name)
    {
        playerName.Value = new FixedString32Bytes(name);
    }
}