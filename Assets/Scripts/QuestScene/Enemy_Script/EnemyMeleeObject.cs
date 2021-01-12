using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeleeObject : MonoBehaviour
{
    [HideInInspector] public EnemyController enemyController;
    public GameObject damageEffect;

    void Awake()
    {
        enemyController = this.transform.root.gameObject.GetComponent<EnemyController>();
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("PC_Field"))
        {
            if (collider.gameObject == enemyController.lockObj) //ÉçÉbÉNëŒè€ÇÃPCÇ…ÇµÇ©çUåÇÇÕîΩâfÇ≥ÇÍÇ»Ç¢
            {
                enemyController.HitAttack(collider.gameObject);
                GameObject effect = Instantiate(damageEffect) as GameObject;
                effect.transform.position = collider.ClosestPointOnBounds(this.transform.position);
            }
        }
    }
}
