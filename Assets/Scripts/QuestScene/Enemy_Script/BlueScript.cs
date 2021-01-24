using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueScript : EnemyController
{
    new void Awake()
    {
        base.Awake();
    }

    new void Start()
    {
        base.Start();
    }

    new void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void Attack()
    {
        //攻撃対象のキャラが有効であれば
        if (base.lockObj.activeSelf)
        {
            base.animator.SetTrigger("Throw");
        }
        else
        {
            base.lockObj = null;
            base.SetState(EState.Idle);
        }
    }

    //animation内で実行
    void SetThrowAttack()
    {
        if (base.lockObj != null)
        {
            GameObject obj = Instantiate(base.attackObj, transform.position + new Vector3(0f, 3f, 0), Quaternion.identity);
            obj.GetComponent<EnemyAttackCollider>().enemyController = this;
            obj.GetComponent<EnemyThrowObject>().SetThrow(base.lockObj.transform.position);
        }
    }
}