using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using TMPro;

public class PlayerInfo : NetworkBehaviour
{
    [SerializeField] private TMP_Text txtPlayerName;
    [SerializeField] private MeshRenderer tankRenderer;

    public NetworkVariable<FixedString64Bytes> playerName = new NetworkVariable<FixedString64Bytes>(
        new FixedString64Bytes("Player Name"), 
        NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Owner);

    public NetworkVariable<Color> playerSkin = new NetworkVariable<Color>(new Color(),
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);

    public override void OnNetworkSpawn()
    {
        playerName.OnValueChanged += OnNameChanged;
        playerSkin.OnValueChanged += OnColorChange;

        txtPlayerName.text = playerName.Value.ToString();
        gameObject.name = "Player_" + playerName.Value.ToString();

        tankRenderer.material.color = playerSkin.Value;

        if (IsLocalPlayer)
            GameManager.instance.SetLocalPlayer(NetworkObject);

        GameManager.instance.OnPlayerJoined(NetworkObject);
    }

    public override void OnNetworkDespawn()
    {
        playerName.OnValueChanged -= OnNameChanged;
        playerSkin.OnValueChanged -= OnColorChange;
    }

    void OnNameChanged (FixedString64Bytes prevVal, FixedString64Bytes newVal)
    {
        if (newVal != prevVal)
        {
            txtPlayerName.text = newVal.Value.ToString();
            GameManager.instance.SetPlayerName(NetworkObject, newVal.Value.ToString());
        }
    }

    void OnColorChange(Color prevVal, Color newVal)
    {
        if (newVal != prevVal)
        {
            tankRenderer.material.color = newVal;
        }
    }

    public void SetName(string name)
    {
        playerName.Value = new FixedString64Bytes(name);
    }

    public void SetSkin(Color color)
    {
        playerSkin.Value = new Color(color.r, color.g, color.b);
    }
}