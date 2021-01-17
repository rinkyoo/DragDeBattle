using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;
using Common;

public class KumamotoScript : CharaController
{
    private CharaManager charaManager;
    private Collider attackCollider;

    private float skillPower = 500f;
    private bool isInSkill = false;

    void Awake()
    {
        base.Awake();
        base.attackObj.GetComponent<PCAttackCollider>().charaController = this;
        attackCollider = attackObj.GetComponent<BoxCollider>();
    }

    void Start()
    {
        base.Start();
        charaManager = GameObject.Find("CharaScript").GetComponent<CharaManager>();
    }

    void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void Attack()
    {
        //攻撃対象のキャラが有効であれば
        if (base.lockObj.activeSelf)
        {
            base.animator.SetTrigger("Attack");
        }
        else
        {
            base.SetState(State.Idle);
        }

    }

    //animation内で実行
    public void SetAttack()
    {
        attackCollider.enabled = true;
        base.audioManager.Swing(); //攻撃SE鳴らす
    }
    public void ResetAttack()
    {
        attackCollider.enabled = false;
    }

    public override void ApplyHitAttack(bool isEnemyDied)
    {
        base.ApplyHitAttack(isEnemyDied);
        attackCollider.enabled = false;
        if (isInSkill && !isEnemyDied)
        {
            Rigidbody enemyRigid = base.lockObj.GetComponent<Rigidbody>();
            enemyRigid.isKinematic = false;
            enemyRigid.AddForce(transform.forward * skillPower, ForceMode.Impulse);
        }
    }

    public override void Skill()
    {
        //自身の攻撃にノックバック効果を付与
        isInSkill = true;
        base.SetBuffEffect();
        //効果時間後に解除（20秒後）
        StartCoroutine(DelayMethod(20f, () =>
        {
            isInSkill = false;
            base.RemoveBuffEffect();
        }));
        
        base.questController.ResumeBattle(); //時間を戻す
    }
    private IEnumerator DelayMethod(float waitTime, Action action)
    {
        yield return new WaitForSeconds(waitTime);
        action();
    }
}
