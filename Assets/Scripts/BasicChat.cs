using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;

public class BasicChat : NetworkBehaviour
{
    [SerializeField] private TMP_Text chatText;
    [SerializeField] private TMP_InputField chatInput;

    public void SendChat()
    {
        if (IsServer)
        {
            ChatServerRPC(NetworkManager.Singleton.LocalClientId + ": " + chatInput.text);
        }
        else if (IsClient)
        {
            ChatServerRPC(NetworkManager.Singleton.LocalClientId + ": " + chatInput.text);
        }

        chatInput.text = "";
    }

    [ServerRpc(RequireOwnership = false)]
    public void ChatServerRPC(string message)
    {
        if (!IsHost)
            chatText.text += "\n" + message;

        ChatClientRPC(message);
    }

    [ClientRpc]
    public void ChatClientRPC(string message)
    {
        chatText.text += "\n" + message;
    }
}