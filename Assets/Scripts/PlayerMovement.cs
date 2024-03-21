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

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        tankRigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        GetAxis();
    }

    private void FixedUpdate()
    {
        MovePlayer();
        RotatePlayer();
    }

    void GetAxis()
    {
        if (!IsOwner)
            return;

        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
    }

    void MovePlayer ()
    {
        tankRigidbody.velocity = tankRigidbody.transform.forward * tankMoveSpeed * vertical;
    }

    void RotatePlayer ()
    {
        tankRigidbody.rotation = Quaternion.Euler(transform.eulerAngles + transform.up * horizontal * tankTurnSpeed);
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