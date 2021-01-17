using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackCollider : MonoBehaviour
{
    [HideInInspector] public EnemyController enemyController;
    [SerializeField] GameObject damageEffect;


    void OnTriggerEnter(Collider collider)
    {
        if (enemyController == null) return;
        if (collider.gameObject.CompareTag("PC_Field"))
        {
            enemyController.HitAttack(collider.gameObject);
            if (damageEffect != null)
            {
                GameObject effect = Instantiate(damageEffect) as GameObject;
                effect.transform.position = collider.ClosestPointOnBounds(this.transform.position);
            }
        }
    }
}
