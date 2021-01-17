using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using UnityEngine;
using Debug = UnityEngine.Debug;


public class ShootObject : MonoBehaviour
{
    //進む方向ベクトル
    Vector3 moveDirection;

    bool isShooted = false;

    public void SetShoot(Vector3 posi)
    {
        posi.y = 3f;
        moveDirection = (posi - transform.position).normalized;
        this.transform.LookAt(posi);
        this.transform.Rotate(new Vector3(-90, 0, 0));

        isShooted = true;
    }

    void FixedUpdate()
    {
        if (isShooted)
        {
            transform.position += moveDirection * Time.deltaTime * 80f;
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.CompareTag("Wall"))
        {
            Destroy(this.gameObject);
        }
    }
}