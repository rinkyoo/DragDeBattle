using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShootObject : MonoBehaviour
{
    [HideInInspector] public EnemyController enemyController;

    //進む方向ベクトル
    Vector3 moveDirection;

    bool isShooted = false;

    public GameObject damageEffect;


    public void SetShoot(Vector3 posi)
    {
        posi.y = 3f;
        moveDirection = (posi - transform.position).normalized;
        this.transform.LookAt(posi);
        this.transform.Rotate(new Vector3(-90, 0, 0));

        isShooted = true;
    }

    void FixedUpdate()
    {
        if (isShooted)
        {
            transform.position += moveDirection * Time.deltaTime * 80f;
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (enemyController == null) return;
        if (collider.gameObject.CompareTag("PC_Field")) //ロック対象以外のPCにも攻撃を適用
        {
            enemyController.HitAttack(collider.gameObject);
            GameObject effect = Instantiate(damageEffect) as GameObject;
            effect.transform.position = collider.ClosestPointOnBounds(this.transform.position);
        }

        if (collider.gameObject.CompareTag("Wall"))
        {
            Destroy(this.gameObject);
        }
    }
}
