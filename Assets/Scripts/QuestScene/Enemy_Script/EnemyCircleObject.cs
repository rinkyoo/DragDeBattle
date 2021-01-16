using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCircleObject : MonoBehaviour
{
    [HideInInspector] public EnemyController enemyController;

    public GameObject damageEffect;

    private float expandRate = 1.05f;

    void FixedUpdate()
    {
        transform.localScale = transform.localScale * expandRate;
        if (transform.localScale.x >= 7f)
        {
            Destroy(this.gameObject);
        }
    }

   

    void OnTriggerEnter(Collider collider)
    {
        print("hit circle trigger");

        if (enemyController == null) return;
        if (collider.gameObject.CompareTag("PC_Field")) //PC全てに攻撃を適用
        {
            print("hit circle atk");
            enemyController.HitAttack(collider.gameObject);
            GameObject effect = Instantiate(damageEffect) as GameObject;
            effect.transform.position = collider.ClosestPointOnBounds(this.transform.position);
        }
    }
}
