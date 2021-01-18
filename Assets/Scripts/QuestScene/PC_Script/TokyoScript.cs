using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuestCommon;

public class TokyoScript : CharaController
{
    private CharaManager charaManager;

    [SerializeField] GameObject HealEffect;

    void Awake()
    {
        base.Awake();
        base.attackObj.GetComponent<PCAttackCollider>().charaController = this;
        base.attackObj.GetComponent<HealCollider>().charaController = this;
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
        //対象キャラが有効であれば
        if(base.lockObj.activeSelf)
        {
            animator.SetTrigger("Throw");
        }
        else
        {
            base.SetState(State.Idle);
        }
        
    }

    //Animation内で実行
    void SetThrowAttack()
    {
        if (base.lockObj != null)
        {
            GameObject obj = Instantiate(base.attackObj, transform.position + new Vector3(0, 3f, 0), Quaternion.identity);
            obj.GetComponent<ThrowObject>().SetThrow(base.lockObj.transform.position);
        }
    }

    public override void LockEnemy(GameObject obj)
    {
        //自身の回復はできない
        if (obj == this.gameObject) return;

        if (lockObj == obj) return;
        this.transform.LookAt(obj.transform.position);
        SetState(State.Lock);
        lockObj = obj;
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
        if(obj == lockObj)
        {
            cs.GetSP(1);
            spSlider.value = cs.nowSP;
            
            if(obj.CompareTag("Enemy"))
            {
                base.HitAttack(obj);
            }
            else if(obj.CompareTag("PC_Field"))
            {
                obj.GetComponent<CharaController>().Healed(cs.str * 1);
                GameObject healEffect = Instantiate(HealEffect, new Vector3(0, 0, 0), Quaternion.Euler(-90f, 0f, 0f));
                healEffect.transform.SetParent(obj.transform, false);
            }
        }
        
        
    }
}
