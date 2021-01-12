using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;
using DG.Tweening;
using Common;

public class HyogoScript : CharaController
{
    private CharaManager charaManager;
    private Collider attackCollider;

    [SerializeField] GameObject damageEffect;

    void Awake()
    {
        base.Awake();
        attackCollider = gameObject.GetComponent<SphereCollider>();
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
            base.animator.SetTrigger("Fighting");
            SetAttack();
        }
        else
        {
            base.SetState(State.Idle);
        }
    }

    public void SetAttack()
    {
        Sequence Seq = DOTween.Sequence();
        Seq.Append(transform.DOLocalRotate(new Vector3(0f, 360f, 0f), 0.5f, RotateMode.LocalAxisAdd))
            .InsertCallback(0.1f, () =>
             {
                 base.audioManager.Swing(); //攻撃SE鳴らす
                 Instantiate(base.attackObj, transform.position, Quaternion.Euler(90f, 0f, 0f));
             })
            .InsertCallback(0.1f, ()=>
            {
                Collider[] colliders = Physics.OverlapSphere(transform.position, 7f);
                for(int i=0;i<colliders.Length;i++)
                {
                    if(colliders[i].gameObject.CompareTag("Enemy"))
                    {
                        GameObject effect = Instantiate(damageEffect) as GameObject;
                        effect.transform.position = colliders[i].gameObject.transform.position;
                        base.HitAttack(colliders[i].gameObject);
                    }
                }
            });
        Seq.Play();
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
