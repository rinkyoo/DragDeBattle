using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleHealObject : MonoBehaviour
{
    private float expandRate = 1.05f;

    void FixedUpdate()
    {
        transform.localScale = transform.localScale * expandRate;
        if (transform.localScale.x >= 7f)
        {
            Destroy(this.gameObject);
        }
    }
}
