using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuestCommon;

public class TottoriScript : CharaController
{
    private CharaManager charaManager;

    [SerializeField] GameObject healCircle;
    [SerializeField] GameObject healEffect;

    new void Awake()
    {
        base.Awake();
        healCircle.GetComponent<HealCollider>().charaController = this;
    }

    new void Start()
    {
        base.Start();
        charaManager = GameObject.Find("CharaScript").GetComponent<CharaManager>();
        base.isHealer = true;
    }

    new void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void Attack()
    {
        //対象キャラが有効であれば
        if (base.lockObj.activeSelf)
        {
            animator.SetTrigger("Jump");
        }
        else
        {
            base.SetState(State.Idle);
        }

    }

    //Animation内で実行
    void SetJumpAttack()
    {
        if (base.lockObj == null) return;
        base.audioManager.Water();
        if (base.lockObj.CompareTag("PC_Field"))
        {
            Instantiate(healCircle, base.lockObj.transform.position + new Vector3(0, 1f, 0), Quaternion.identity);
        }
        else
        {
            HitAttack(base.lockObj); //TottoriScript内のHitAttackを実行
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
            GameObject effect = Instantiate(healEffect, new Vector3(0, 0, 0), Quaternion.Euler(-90f, 0f, 0f));
            effect.transform.SetParent(cc.gameObject.transform, false);

            base.questController.ResumeBattle(); //時間を戻す

        }
    }

    public override void HitAttack(GameObject obj)
    {
        if (obj.CompareTag("PC_Field"))
        {
            obj.GetComponent<CharaController>().Healed(cs.str * 1);
            GameObject effect = Instantiate(healEffect, new Vector3(0, 0, 0), Quaternion.Euler(-90f, 0f, 0f));
            effect.transform.SetParent(obj.transform, false);
        }
        else if (obj.CompareTag("Enemy"))
        {
            if (obj == lockObj)
            {
                //敵位置に攻撃エフェクトを表示
                Vector3 targetPosi = base.lockObj.transform.position;
                targetPosi.y = 0f;
                Instantiate(base.attackObj, targetPosi, Quaternion.Euler(-90f, 0f, 0f));

                base.HitAttack(obj);
            }
        }
    }
}
