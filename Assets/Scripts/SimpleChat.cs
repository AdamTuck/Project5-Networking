using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;

public class SimpleChat : NetworkBehaviour
{
    [SerializeField] private TMP_Text chatText;
    
    public void SendChat ()
    {
        if (IsServer)
        {
            ChatClientRPC();
        }
        else if (IsClient)
        {
            ChatServerRPC();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void ChatServerRPC ()
    {
        chatText.text = "Test message from the client";
    }

    [ClientRpc]
    public void ChatClientRPC ()
    {
        chatText.text = "Test message from the server";
    }
}