using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MieScript : CharaController
{
    private Collider attackCollider;
    [HideInInspector] public bool isThrowing = false;
    bool isSkill = false;

    void Awake()
    {
        base.Awake();
        base.attackObj.GetComponent<BoomerangObject>().charaController = this;
        base.attackObj.GetComponent<BoomerangObject>().mieScript = this;
    }
    void Start()
    {
        base.Start();
    }

    void FixedUpdate()
    {
        if (isThrowing) return;
        base.FixedUpdate();
    }

    public override void Attack()
    {
        if (base.lockObj.activeSelf && !isSkill)
        {
            base.animator.SetTrigger("Boomerang");
        }
        else
        {
            base.SetState(State.Idle);
        }
    }
    void SetBoomerangAttack()
    {
        if (base.lockObj != null)
            base.attackObj.GetComponent<BoomerangObject>().SetBoomerang(base.lockObj.transform.position);
    }

    /*
    10•bŠÔUŒ‚•s”\ && “G‘Sˆõ‚ÌƒƒbƒN‘ÎÛ‚É‚È‚é
    */
    public override void Skill()
    {
        isSkill = true;
        StartCoroutine("ForcedLockPCCoroutine");
        StartCoroutine(DelayMethod(10f, () =>
        {
            isSkill = false;
            StopCoroutine("ForcedLockPCCoroutine");
            base.questController.StartLockPCCoroutine();
            base.lockObj = null;
        }));
        
        base.questController.ResumeBattle(); //ŽžŠÔ‚ð–ß‚·
    }

    public override void SetMovePosi(Vector3 posi)
    {
        if (isThrowing) return;
        base.SetMovePosi(posi);
    }

    public override void LockEnemy(GameObject obj)
    {
        if (isThrowing) return;
        base.LockEnemy(obj);
    }
    
    private IEnumerator DelayMethod(float waitTime, Action action)
    {
        yield return new WaitForSeconds(waitTime);
        action();
    }
    
    IEnumerator ForcedLockPCCoroutine()
    {
        for (; ; )
        {
            base.questController.ForcedLockPC(this.gameObject);
            yield return new WaitForSeconds(0.5f);
        }
    }
}
