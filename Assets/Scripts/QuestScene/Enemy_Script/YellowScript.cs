using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YellowScript : EnemyController
{
    void Awake()
    {
        base.Awake();
    }

    void Start()
    {
        base.Start();
    }

    void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void Attack()
    {
        if (base.lockObj.GetComponent<CharaController>().IsInField())
        {
            animator.SetTrigger("Shoot");
        }
        else
        {
            base.lockObj = null;
            base.SetState(EState.Idle);
        }
    }
    //animation内で実行
    void SetShootAttack()
    {
        if (base.lockObj != null)
        {
            //base.audioManager.Shoot();
            GameObject obj = Instantiate(base.attackObj, transform.position + new Vector3(0f, 3f, 0), Quaternion.identity);
            obj.GetComponent<EnemyAttackCollider>().enemyController = this;
            obj.GetComponent<EnemyShootObject>().SetShoot(base.lockObj.transform.position);
        }
    }
}