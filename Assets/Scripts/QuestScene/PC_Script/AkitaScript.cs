using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using System;
using Common;

public class AkitaScript : CharaController
{
    private CharaManager charaManager;

    public GameObject HealEffect;


    void Awake()
    {
        base.Awake();
    }
    void Start()
    {
        base.Start();
        charaManager = GameObject.Find("CharaScript").GetComponent<CharaManager>();
        base.isHealer = true;
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
    void SetWaterAttack()
    {
        if (base.lockObj != null)
        {
            base.audioManager.Water();
            HitAttack(base.lockObj); //AkitaScript内のHitAttackを実行
        }
    }

    public override void Skill()
    {
        //味方全員に回復効果を付与/自身も（攻撃力＊３分）
        for (int i = 0; i < Define.charaNum; i++)
        {
            CharaController cc = charaManager.GetCharaController(i);
            if (!cc.IsInField()) continue; //フィールド上に存在していない場合は無視
            cc.Healed(cs.str * 3);
            GameObject healEffect = Instantiate(HealEffect, new Vector3(0, 0, 0), Quaternion.Euler(-90f, 0f, 0f));
            healEffect.transform.SetParent(cc.gameObject.transform, false);

            base.questController.ResumeBattle(); //時間を戻す

        }
    }

    public override void HitAttack(GameObject obj)
    {
        if (obj == lockObj)
        {
            cs.GetSP(1);
            spSlider.value = cs.nowSP;

            if (obj.CompareTag("Enemy"))
            {
                //敵位置に攻撃エフェクトを表示
                Vector3 targetPosi = base.lockObj.transform.position;
                targetPosi.y = 0f;
                Instantiate(base.attackObj, targetPosi, Quaternion.Euler(-90f, 0f, 0f));

                base.HitAttack(obj);
            }
            else if (obj.CompareTag("PC_Field"))
            {
                obj.GetComponent<CharaController>().Healed(cs.str * 1);
                GameObject healEffect = Instantiate(HealEffect, new Vector3(0, 0, 0), Quaternion.Euler(-90f, 0f, 0f));
                healEffect.transform.SetParent(obj.transform, false);
            }
        }
    }
}
