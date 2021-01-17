using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackScript : EnemyController
{
    void Awake()
    {
        base.Awake();
        base.attackObj.GetComponent<EnemyAttackCollider>().enemyController = this;
    }

    void Start()
    {
        base.Start();
    }

    void FixedUpdate()
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
