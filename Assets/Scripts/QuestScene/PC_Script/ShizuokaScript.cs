using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using QuestCommon;
using System.Diagnostics;
using UnityEngine.SceneManagement;

public class ShizuokaScript : CharaController
{
    bool isInSkill = false;

    new void Awake()
    {
        base.Awake();
        base.attackObj.GetComponent<PCAttackCollider>().charaController = this;
    }

    new void Start()
    {
        base.Start();
    }

    new void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void Attack()
    {
        if (base.lockObj.activeSelf)
        {
            animator.SetTrigger("Shoot");
            //スキル効果中は2回攻撃
            if (isInSkill)
            {
                StartCoroutine(DelayMethod(0.2f, () =>
                {
                    animator.SetTrigger("Shoot");
                }));
            }
        }
        else
        {
            base.SetState(State.Idle);
        }
    }

    public override void HitAttack(GameObject enemyObj)
    {
        base.questController.SetPCAttackEvent(this, enemyObj.GetComponent<EnemyController>());
    }

    //animation内で実行
    void SetShootAttack()
    {
        if (base.lockObj != null)
        {
            base.audioManager.Shoot();
            GameObject obj = Instantiate(base.attackObj, transform.position + new Vector3(0f, 3f, 0), Quaternion.identity);
            obj.GetComponent<ShootObject>().SetShoot(base.lockObj.transform.position);
        }
    }

    public override void ApplyHitAttack(bool isEnemyDied)
    {
        base.audioManager.Bomb();
        base.ApplyHitAttack(isEnemyDied);
    }

    public override void Skill()
    {
        isInSkill = true;
        //効果時間後にスキル効果解除（20秒後）
        StartCoroutine(DelayMethod(20f, () =>
        {
            isInSkill = false;
        }));

        base.questController.ResumeBattle(); //時間を戻す
    }

    private IEnumerator DelayMethod(float waitTime, Action action)
    {
        yield return new WaitForSeconds(waitTime);
        action();
    }
}
