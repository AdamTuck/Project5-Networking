using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Bullet : NetworkBehaviour
{
    [HideInInspector] public ulong clientID;

    private void OnCollisionEnter(Collision collision)
    {
        if (IsServer)
        {
            PlayerDamage otherObj = collision.gameObject.GetComponent<PlayerDamage>();

            if (otherObj && clientID != otherObj.OwnerClientId)
            {
                otherObj.GetDamage();
                Debug.Log($"Client ID: {clientID}    Owner Client ID: {OwnerClientId}");

                GameManager.instance.AddScore(clientID);
            }

            Destroy(gameObject);
        }
    }
}