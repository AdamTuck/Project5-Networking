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
        ShootTankBullet();
    }

    void ShootTankBullet ()
    {
        if (!IsOwner)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject bullet = Instantiate(bulletObj, shootPoint.position, shootPoint.rotation);

            bullet.GetComponent<Rigidbody>().AddForce(tankRigidbody.velocity + bullet.transform.forward * bulletSpeed, ForceMode.VelocityChange);

            Destroy(bullet, 10);
        }
    }
}