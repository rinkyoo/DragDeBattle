using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCAttackCollider : MonoBehaviour
{
    [HideInInspector] public CharaController charaController;
    [SerializeField] GameObject damageEffect;


    void OnTriggerEnter(Collider collider)
    {
        if (charaController == null) return;
        if (collider.gameObject.CompareTag("Enemy"))
        {
            charaController.HitAttack(collider.gameObject);
            GameObject effect = Instantiate(damageEffect) as GameObject;
            effect.transform.position = collider.ClosestPointOnBounds(this.transform.position);
        }
    }
}
