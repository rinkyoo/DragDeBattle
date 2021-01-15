using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokyoHealScript : MonoBehaviour
{
    public GameObject HealEffect;

    void OnTriggerEnter(Collider collider)
    {
        CharaController charaController = gameObject.GetComponent<ThrowObject>().charaController;
        if (charaController == null) return;

        if (collider.gameObject.CompareTag("PC_Field"))
        {
            if(collider.gameObject == charaController.lockObj)
            {
                charaController.HitAttack(collider.gameObject);
                Destroy(this.gameObject);
            }
        }
    }
}
