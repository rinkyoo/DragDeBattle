using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectDelete : MonoBehaviour
{
    void Start()
    {
        Invoke("Delete",0.5f);
    }
    void Delete()
    {
        Destroy(this.gameObject);
    }
}
