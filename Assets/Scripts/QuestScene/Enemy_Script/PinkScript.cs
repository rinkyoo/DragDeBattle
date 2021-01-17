using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinkScript : EnemyController
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
        animator.SetTrigger("Jump");

    }

    //animation内で実行
    void SetCircleAttack()
    {
        GameObject obj = Instantiate(base.attackObj, transform.position + new Vector3(0f, 1f, 0), Quaternion.Euler(0,0,0));
        obj.GetComponent<EnemyAttackCollider>().enemyController = this;
    }
}