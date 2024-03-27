using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerShoot : NetworkBehaviour
{
    Rigidbody tankRigidbody;
    [SerializeField] GameObject bulletObj;
    [SerializeField] Transform shootPoint;

    [SerializeField] private float bulletSpeed;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        tankRigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (IsServer && IsLocalPlayer)
            {
                ShootTankBullet(OwnerClientId);
            }
            else if (IsClient && IsLocalPlayer) 
            {
                RequestShootServerRPC();
            }
        }  
    }

    [ServerRpc]
    public void RequestShootServerRPC (ServerRpcParams serverParams = default)
    {
        ShootTankBullet(serverParams.Receive.SenderClientId);
    }

    void ShootTankBullet (ulong ownerID)
    {
        GameObject bullet = Instantiate(bulletObj, shootPoint.position, shootPoint.rotation);
        bullet.GetComponent<NetworkObject>().Spawn();
        bullet.GetComponent<Bullet>().clientID = ownerID;

        bullet.GetComponent<Rigidbody>().AddForce(tankRigidbody.velocity + bullet.transform.forward * bulletSpeed, ForceMode.VelocityChange);

        Destroy(bullet, 10);
    }
}