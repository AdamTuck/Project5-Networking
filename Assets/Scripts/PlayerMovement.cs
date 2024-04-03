using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private float tankMoveSpeed = 10;
    [SerializeField] private float tankTurnSpeed = 10;

    private Rigidbody tankRigidbody;

    private float horizontal, vertical;

    bool hasAuthority => IsServer;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        tankRigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        GetAxis();
    }

    [ServerRpc]
    public void MovementServerRPC (float _horizontal, float _vertical)
    {
        if (GameManager.instance.state.Value == 1)
        {
            horizontal = _horizontal;
            vertical = _vertical;
        }
        else
        {
            horizontal = 0;
            vertical = 0;
        }
    }

    private void FixedUpdate()
    {
        tankRigidbody.velocity = tankRigidbody.transform.forward * tankMoveSpeed * vertical;
        tankRigidbody.rotation = Quaternion.Euler(transform.eulerAngles + transform.up * horizontal * tankTurnSpeed);
    }

    void GetAxis()
    {
        if (IsServer && IsLocalPlayer)
        {
            if (GameManager.instance.state.Value == 1)
            {
                horizontal = Input.GetAxis("Horizontal");
                vertical = Input.GetAxis("Vertical");
            }
            else
            {
                horizontal = 0;
                vertical = 0;
            }
        }
        else if (IsClient && IsLocalPlayer)
        {
            MovementServerRPC(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }
    }

    //void ShootTankBullet()
    //{
    //    if (!IsOwner)

    //        if (Input.GetKeyDown(KeyCode.Space))
    //        {
    //            GameObject bullet = Instantiate(bulletObj, shootPoint.position, shootPoint.rotation);

    //            bullet.GetComponent<Rigidbody>().AddForce(tankRigidbody.velocity + bullet.transform.forward * bulletSpeed, ForceMode.VelocityChange);

    //            Destroy(bullet, 10);
    //        }
    //}
}