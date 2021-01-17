using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using System;
using Common;

public class NiigataScript : CharaController
{
    private CharaManager charaManager;

    void Awake()
    {
        base.Awake();
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
            base.animator.SetTrigger("Jump");
        }
        else
        {
            base.SetState(State.Idle);
        }

    }

    //Animation内で実行
    void SetJumpAttack()
    {
        if (base.lockObj != null)
        {
            base.audioManager.Water();
            Vector3 targetPosi = base.lockObj.transform.position;
            targetPosi.y = 0f;
            GameObject obj = Instantiate(base.attackObj, targetPosi, Quaternion.Euler(90f, 0f, 0f));
            base.HitAttack(base.lockObj);
        }
    }

    public override void Skill()
    {
        //味方全員にバフを付与（攻撃力アップ）
        for (int i = 0; i < Define.charaNum; i++)
        {
            CharaController cc = charaManager.GetCharaController(i);
            if (!cc.IsInField()) continue; //フィールド上に存在していない場合は無視
            int temp = Convert.ToInt32(Math.Floor(cc.cs.str * 0.5)); //攻撃力＋50%（切り捨て）
            cc.cs.str += temp;
            cc.SetBuffEffect();
            //効果時間後にバフを解除（10秒後）
            StartCoroutine(DelayMethod(10f, () =>
            {
                cc.cs.str -= temp;
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
