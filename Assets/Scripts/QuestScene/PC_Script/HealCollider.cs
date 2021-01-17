using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealCollider : MonoBehaviour
{
    [HideInInspector] public CharaController charaController;

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("PC_Field"))
        {
            if(charaController != null) charaController.HitAttack(collider.gameObject);
        }
    }
}
