using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerDamage : NetworkBehaviour
{
    public void GetDamage()
    {
        Debug.Log($"{OwnerClientId} destroyed");
    }
}
