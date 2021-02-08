using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;
using QuestCommon;

public class KumamotoScript : CharaController
{
    private CharaManager charaManager;
    private Collider attackCollider;

    private float skillPower = 500f;
    private bool isInSkill = false;

    new void Awake()
    {
        base.Awake();
        base.attackObj.GetComponent<PCAttackCollider>().charaController = this;
        attackCollider = attackObj.GetComponent<BoxCollider>();
    }

    new void Start()
    {
        base.Start();
        charaManager = GameObject.Find("CharaScript").GetComponent<CharaManager>();
    }

    new void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void Attack()
    {
        //UŒ‚‘ÎÛ‚ÌƒLƒƒƒ‰‚ª—LŒø‚Å‚ ‚ê‚Î
        if (base.lockObj.activeSelf)
        {
            base.animator.SetTrigger("Attack");
        }
        else
        {
            base.SetState(State.Idle);
        }

    }

    //animation“à‚ÅÀs
    public void SetAttack()
    {
        attackCollider.enabled = true;
        base.audioManager.Swing(); //UŒ‚SE–Â‚ç‚·
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
        //©g‚ÌUŒ‚—Í‚ğ‚Q”{
        int temp = base.cs.str;
        base.cs.str += temp;
        base.SetBuffEffect();
        //Œø‰ÊŠÔŒã‚É‰ğœi20•bŒãj
        StartCoroutine(DelayMethod(20f, () =>
        {
            base.cs.str -= temp;
            base.RemoveBuffEffect();
        }));
        
        base.questController.ResumeBattle(); //ŠÔ‚ğ–ß‚·
    }
    private IEnumerator DelayMethod(float waitTime, Action action)
    {
        yield return new WaitForSeconds(waitTime);
        action();
    }
}
