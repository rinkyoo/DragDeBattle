using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeObject : MonoBehaviour
{
    [HideInInspector] public CharaController charaController;
    public GameObject damageEffect;

    void OnTriggerEnter(Collider collider)
    {
        if (charaController == null) return;
        if (collider.gameObject.CompareTag("Enemy"))
        {
            if (collider.gameObject == charaController.lockObj) //ロック対象の敵しか攻撃しない
            {
                charaController.HitAttack(collider.gameObject);
                GameObject effect = Instantiate(damageEffect) as GameObject;
                effect.transform.position = collider.ClosestPointOnBounds(this.transform.position);
            }
        }
    }
}
