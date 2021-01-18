using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCircleObject : MonoBehaviour
{
    private float expandRate = 1.08f;

    void FixedUpdate()
    {
        transform.localScale = transform.localScale * expandRate;
        if (transform.localScale.x >= 5f)
        {
            Destroy(this.gameObject);
        }
    }
}
