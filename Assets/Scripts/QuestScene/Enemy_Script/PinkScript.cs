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
        base.attackCollider = base.attackObj.GetComponent<BoxCollider>();
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