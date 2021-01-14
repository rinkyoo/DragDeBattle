using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    private QuestController questController;
    [HideInInspector] public EnemyStatus es = new EnemyStatus();
    protected Animator animator;
    
    [SerializeField] protected GameObject attackObj;
    protected Collider attackCollider;
    
    Slider hpSlider;
    
    private float elapsedTime = 0;
    [HideInInspector] public GameObject lockObj;

    #region ドロップ関連
    private int dropExp = 0;
    private int dropCoin = 0;
    private ExpItem_Info dropExpItem;
    private int expItemKakuritsu = 0;
    #endregion

    private bool dieAnimationFlag = true;

    public enum EState{
        Idle,
        Move,
        Lock,
        Wait,
        Freeze,
        Die,
    }
    protected EState state;
    protected EState preState;
    bool isInAttackRange = false;
    //***************************

    public void Awake()
    {
        questController = GameObject.Find("QuestController").GetComponent<QuestController>();
        hpSlider = transform.Find("Canvas/Slider").GetComponent<Slider>();
        animator = this.gameObject.GetComponent<Animator>();
        attackCollider = attackObj.GetComponent<BoxCollider>();
    }
    public void Start()
    {
        state = EState.Idle;
        preState = EState.Idle;
        WaitEnemyMove();
    }
    IEnumerator WaitEnemyMove()
    {
        yield return new WaitForSeconds(0.5f);
    }

    public void FixedUpdate()
    {
        elapsedTime += Time.deltaTime; //ここで計算していいのかは一考の余地あり！
        switch (state)
        {
            case EState.Lock:
                if (lockObj == null)
                { 
                    SetState(EState.Idle);
                    return;
                }
                //攻撃範囲内にロックしたPCがいる場合
                if(Vector3.Distance(transform.position,lockObj.transform.position)<=
                                    es.rangeAtk)
                {
                    isInAttackRange = true;
                    animator.SetBool("Walk",false); //移動モーションの停止
                    GetComponent<Rigidbody>().isKinematic = true; //キャラが他キャラに押されないようにするため
                }
                //攻撃範囲外だったら
                else
                {
                    isInAttackRange = false;
                    GetComponent<Rigidbody>().isKinematic = false;
                }

                //攻撃範囲外の場合は移動
                if (!isInAttackRange)
                {
                    //lockObjに向かって進む
                    transform.position += transform.forward * Time.deltaTime * 10f * es.agi;
                }
                //攻撃範囲内の場合は攻撃
                else
                {
                    if (elapsedTime < es.speedAtk)
                    {
                        SetState(EState.Freeze);
                        return;
                    }
                    if (!es.isDied())
                    {
                        Attack();
                        elapsedTime = 0;
                        SetState(EState.Freeze);
                    }
                }
                break;
                
            case EState.Freeze:
                if(elapsedTime > es.speedAtk)
                {
                    state = preState;
                }
                break;
                
            case EState.Die:
                break;
        }

        if(lockObj != null)
        {
            //lockObjの方に少しずつ向きが変わる
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lockObj.transform.position - transform.position), 0.2f);
        }
    }
    
    protected void SetState(EState setState)
    {
        switch(setState)
        {
            case EState.Wait:
                state = setState;
                break;
            case EState.Idle:
                state = setState;
                lockObj = null;
                animator.SetBool("Walk",false);
                animator.SetTrigger("Idle");
                break;
            case EState.Lock:
                state = setState;
                animator.SetBool("Walk", true);
                break;
            case EState.Freeze:
                preState = state;
                state = setState;
                break;
            case EState.Die:
                if (state == setState) return;
                state = setState;
                gameObject.GetComponent<CapsuleCollider>().enabled = false;
                animator.SetTrigger("Die");
                #region ドロップ関連
                questController.PlusExp(dropExp);
                if (dropCoin != 0)
                {
                    questController.SetDropItem(transform.position + new Vector3(0,10f,0), "coin");
                    questController.PlusCoin(dropCoin);
                }
                if (dropExpItem != null)
                {
                    if(Probability(expItemKakuritsu))
                    {
                        questController.SetDropItem(transform.position + new Vector3(0, 10f, 0), "exp");
                        questController.PlusExpItem(dropExpItem);
                    }
                }
                #endregion
                questController.DespornEnemy();
                break;
        }
    }
    
    public void LockPC(GameObject pc)
    {
        //ロック中のobjがいない　or より近いPCがいる ー＞ ロックPCを更新
        if (lockObj == null ||
           Vector3.Distance(transform.position,pc.transform.position) <
           Vector3.Distance(transform.position,lockObj.transform.position) )
        {
            SetLockPC(pc);
        }
    }
    //ロックPCを更新
    public void SetLockPC(GameObject pc)
    {
        if (lockObj == pc) return;
        lockObj = pc;
        SetState(EState.Lock);
        this.transform.LookAt(pc.transform.position);
    }
    //ロックPCがフィールド上にいるかの確認
    public void CheckLockPC()
    {
        if (lockObj == null) return;
        if( !lockObj.GetComponent<CharaController>().IsInField() )
        {
            lockObj = null;
        }
    }

    public virtual void Attack()
    {
        if(!lockObj.GetComponent<CharaController>().IsInField())
        {
            lockObj = null;
            SetState(EState.Idle);
            return;
        }
        animator.SetTrigger("Attack");
    }
    public virtual void HitAttack(GameObject pcObj)
    {
        questController.SetEnemyAttackEvent(this, pcObj.GetComponent<CharaController>());
    }
    //実際に攻撃が適用された際の処理（PCへのダメージはQuestControllerから行う）
    public void ApplyHitAttack(bool isPCDied)
    {
        es.GetSP(1);
        if (isPCDied) SetState(EState.Idle);
    }
    //ダメージを自身のHPに反映ー＞HPが０になればtrueを返す
    public bool Attacked(int damage)
    {
        if (es.isDied()) return true; ;
        es.Damage(damage);
        hpSlider.value = es.nowHP;
        if(es.isDied())
        {
            SetState(EState.Die);
            return true;
        }
        else
        {
            /*仰け反りアクションがない方がいいかもなのでコメントアウト中
            animator.SetTrigger("Damage");
            */
            return false;
        }
    }

    public void Healed(int damage)
    {
        es.Damage(damage);
        hpSlider.value = es.nowHP;
    }
    
    public bool IsInField()
    {
        return ( state != EState.Die );
    }
    
    /*
    //animator用
    public void virtual SetAttack()
    {
        attackCollider.enabled = true;
    }
    pubvoid virtual ResetAttack()
    {
        attackCollider.enabled = false;
    }
    */
    void Desporn()
    {
        Destroy(this.gameObject);
    }
    
    public void SetStatus(Enemy_Info enemy)
    {
        es.SetStatus(enemy);
        hpSlider.maxValue = es.maxHP;
        hpSlider.value = es.maxHP;
        dropExp = enemy.DropEXP;
        dropCoin = enemy.DropCoin;
        dropExpItem = enemy.DropEXPItem;
        expItemKakuritsu = enemy.ExpItemKakuritsu;
    }

    //ドロップアイテム用の確率
    bool Probability(float percent)
    {
        float fProbabilityRate = UnityEngine.Random.value * 100.0f;

        if (fProbabilityRate < percent)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
