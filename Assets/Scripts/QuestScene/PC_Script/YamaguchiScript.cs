using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YamaguchiScript : CharaController
{
    [SerializeField]
    private GameObject skillObj; //スキル発動時につかうオブジェクト
    
    void Awake()
    {
        base.Awake();
        base.attackObj.GetComponent<PCAttackCollider>().charaController = this;
    }
    void Start()
    {
        base.Start();
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
            base.animator.SetTrigger("Throw");
        }
        else
        {
            base.SetState(State.Idle);
        }
    }

    //animation内で実行
    void SetThrowAttack()
    {
        if (base.lockObj != null)
        {
            GameObject obj = Instantiate(base.attackObj, transform.position + new Vector3(0f, 3f, 0), Quaternion.identity);
            obj.GetComponent<ThrowObject>().SetThrow(base.lockObj.transform.position);
        }
    }
    
    public override void Skill()
    {
        GameObject obj = Instantiate(skillObj, transform.position+new Vector3(0,3f,0), Quaternion.identity);
        //自身の攻撃力を与える
        obj.GetComponent<YamaguchiSkill>().SetSkill(base.cs.str);
    }
}
