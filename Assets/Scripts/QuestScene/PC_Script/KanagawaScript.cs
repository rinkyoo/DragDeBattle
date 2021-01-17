using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Common;
using System.Diagnostics;

public class KanagawaScript : CharaController
{
    private CharaManager charaManager;

    private Collider attackCollider;

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
        audioManager.Swing(); //攻撃SE鳴らす
    }
    public void ResetAttack()
    {
        attackCollider.enabled = false;
    }

    public override void ApplyHitAttack(bool isEnemyDied)
    {
        base.ApplyHitAttack(isEnemyDied);
        attackCollider.enabled = false;
    }

    public override void Skill()
    {
        /*
        味方全員にバフを付与
        10秒間攻撃間隔が-50%
        */
        for (int i = 0; i < Define.charaNum; i++)
        {
            CharaController cc = charaManager.GetCharaController(i);
            if (!cc.IsInField()) continue; //フィールド上に存在していない場合は無視
            int temp = Convert.ToInt32(Math.Floor(cc.cs.speedAtk * 0.5));
            cc.cs.speedAtk -= temp;
            cc.SetBuffEffect();
            
            StartCoroutine(DelayMethod(10f, () =>
            {
                cc.cs.speedAtk += temp;
                cc.RemoveBuffEffect();
            }));

            base.questController.ResumeBattle(); //時間を戻す

        }
    }

    private IEnumerator DelayMethod(float waitTime, Action action)
    {
        yield return new WaitForSeconds(waitTime);
        action();
    }
}
