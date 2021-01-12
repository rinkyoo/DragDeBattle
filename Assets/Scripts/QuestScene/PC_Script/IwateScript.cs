using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IwateScript : CharaController
{
    bool isInSkill = false;
    bool isRotate = false;
    int chageAngle = 24;
    int nowChangedAngle = 0;

    void Awake()
    {
        base.Awake();
    }
    void Start()
    {
        base.Start();
    }

    void FixedUpdate()
    {
        base.FixedUpdate();
        if(isRotate)
        {
            transform.Rotate(0, chageAngle, 0);
            nowChangedAngle += chageAngle;
            if (nowChangedAngle >= 360)
            {
                nowChangedAngle = 0;
                isRotate = false;
            }
            else
            {
                if(nowChangedAngle % 60 == 0)base.audioManager.Shoot();
                GameObject obj = Instantiate(base.attackObj, transform.position + new Vector3(0f, 3f, 0), Quaternion.identity);
                obj.GetComponent<ShootObject>().charaController = this;
                obj.GetComponent<ShootObject>().SetShoot(transform.position + transform.rotation * new Vector3(10f, 0, 0));
            }
        }
    }

    public override void Attack()
    {
        if (base.lockObj.activeSelf)
        {
            if(isRotate)
            {
                transform.LookAt(base.lockObj.transform);
                nowChangedAngle = 0;
                isRotate = false;
            }
            animator.SetTrigger("Shoot");
            if (isInSkill) isRotate = true;
        }
        else
        {
            base.SetState(State.Idle);
        }
    }
    //animation内で実行
    void SetShootAttack()
    {
        if (base.lockObj != null)
        {
            base.audioManager.Shoot();
            GameObject obj = Instantiate(base.attackObj, transform.position + new Vector3(0f, 3f, 0), Quaternion.identity);
            obj.GetComponent<ShootObject>().charaController = this;
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
