using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteScript : EnemyController
{
    new void Awake()
    {
        base.attackObj.GetComponent<EnemyAttackCollider>().enemyController = this;
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

    //animation内で実行
    public void SetAttack()
    {
        base.attackCollider.enabled = true;

    }
    public void ResetAttack()
    {
        base.attackCollider.enabled = false;
    }
}