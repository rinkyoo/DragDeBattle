using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using QuestCommon;

public class CharaController : MonoBehaviour
{
    #region スクリプト宣言
    protected QuestController questController;
    private CharaIconController charaIconController;
    [HideInInspector] public CharaStatus cs = new CharaStatus();
    protected AudioManager audioManager; //SE用
    [HideInInspector] public Animator animator;
    #endregion

    #region バフ効果を得たときのパーティクル関係
    [SerializeField] private GameObject BuffEffect;
    private GameObject buffEffect;
    int buffNum = 0;
    #endregion

    [HideInInspector] public Toggle toggle; //Auto機能の切り替え用
    [HideInInspector] public Slider hpSlider; //hpバー
    [HideInInspector] public Slider spSlider; //spバー

    private float elapsedTime = 100f; //攻撃間隔の時間計算用
    private float waitingTime; //キャラの再召喚にかかる時間計算用
    public float GetWaitingTime()
    {
        return waitingTime;
    }

    private Vector3 movePosi;
    [HideInInspector] public GameObject lockObj; //攻撃対象の敵用
    public GameObject attackObj; //攻撃に使用するオブジェクト

    public enum State{
        Icon,
        Idle,
        Move,
        Lock,
        Freeze,
        Wait,
        Die,
        None,
    }
    State state;
    State preState;
    private bool isInAttackRange = false;
    private bool auto = false;
    [HideInInspector] public bool isHealer = false;

    public void Awake()
    {
        if (SceneManager.GetActiveScene().name == "Home")
        {
            attackObj.SetActive(false);
            gameObject.AddComponent<HomePCController>();
            Destroy(GetComponent<CharaController>());
            return;
        }
        attackObj.SetActive(true);
        questController = GameObject.Find("QuestController").GetComponent<QuestController>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        animator = this.gameObject.GetComponent<Animator>();
        animator.enabled = false;
    }

    public void Start()
    {
        charaIconController = gameObject.GetComponent<CharaIconController>();
        state = State.Icon;
        preState = State.Idle;

        #region toggleとsliderの設定
        GameObject canvas = GameObject.Find("QuestCanvas");
        toggle = canvas.transform.Find("PCDragPanel/#" + this.gameObject.name + "/Toggle").gameObject.GetComponent<Toggle>();
        //Auto機能の切り替え時の処理の設定
        toggle.onValueChanged.AddListener((bool isOn)=>
        {
            if (isOn)
            {
                auto = true;
                lockObj = null;
                toggle.gameObject.GetComponent<CanvasGroup>().alpha = 1f;
            }
            else
            {
                auto = false;
                toggle.gameObject.GetComponent<CanvasGroup>().alpha = 0.5f;
            }
        });
        
        hpSlider = canvas.transform.Find("PC_Panel/" + this.gameObject.name + "_Panel/HPSlider").GetComponent<Slider>();
        hpSlider.maxValue = cs.maxHP;
        hpSlider.value = cs.maxHP;
        spSlider = canvas.transform.Find("PC_Panel/" + this.gameObject.name + "_Panel/SPSlider").GetComponent<Slider>();
        spSlider.maxValue = cs.maxSP;
        spSlider.value = 0;
        #endregion
    }

    public void FixedUpdate()
    {
        elapsedTime += Time.deltaTime; //ここで計算していいのかは一考の余地あり?

        switch (state)
        {
            case State.Move:
                //movePosiの方に少しずつ向きが変わる
                transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (movePosi - transform.position), 0.2f);
                //movePosiに向かって進む
                transform.position += transform.forward * Time.deltaTime * 10f * cs.agi;
                
                if(Vector3.Distance(transform.position,movePosi) <= 0.1f){
                    SetState(State.Idle);
                }
                break;
            case State.Lock:
                if(lockObj == null)
                {
                    SetState(State.Idle);
                    return;
                }
                //攻撃範囲内にロックした敵キャラがいる場合
                if(Vector3.Distance(transform.position,lockObj.transform.position) <= cs.rangeAtk)
                {
                    isInAttackRange = true;
                    animator.SetBool("Walk",false);
                }
                else
                {
                    isInAttackRange = false;
                }

                //lockObjの方に少しずつ向きが変わる
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lockObj.transform.position - transform.position), 0.2f);

                if (!isInAttackRange)
                {
                    //lockObjに向かって進む
                    transform.position += transform.forward * Time.deltaTime * 10f * cs.agi;
                }
                else
                {
                    if(elapsedTime < cs.speedAtk)
                    {
                        SetState(State.Freeze);
                        return;
                    }
                    if(!cs.isDied())
                    {
                        Attack();
                        elapsedTime = 0;
                        SetState(State.Freeze);
                    }
                }
                break;
            case State.Freeze:
                if(elapsedTime > cs.speedAtk){
                    state = preState;
                }
                break;
            case State.Wait:
                waitingTime -= Time.deltaTime;
                if(waitingTime <= 0)
                {
                    charaIconController.HideWaitGageImage();
                    SetState(State.Icon);
                }
                else
                {
                    charaIconController.UpdateWaitGageImage(waitingTime);
                }
                break;
        }
        //Auto設定時は常にロック状態にする。（Enemyがフィールド上にいる場合）
        if(auto && IsInField() && state != State.None)
        {
            if(lockObj == null)
            {
                LockEnemy(questController.GetNearEnemy(transform.position));
            }
        }
    }
    public void SetState(State setState)
    {
        switch(setState)
        {
            case State.Idle:
                lockObj = null;
                animator.SetBool("Walk",false);
                animator.SetTrigger("Idle");
                break;
            case State.Move:
                animator.SetBool("Walk",true);
                break;
            case State.Lock:
                animator.SetBool("Walk",true);
                break;
            case State.Freeze:
                preState = state;
                break;
            case State.Die:
                gameObject.GetComponent<CapsuleCollider>().enabled = false;
                charaIconController.Died();
                animator.SetTrigger("Die");
                questController.DespornPC();
                break;
            case State.Wait:
                break;
            case State.Icon:
                break;

        }
        state = setState;
    }
    //攻撃時の処理
    public virtual void Attack()
    {
        //キャラ毎に違うアクションを行うので、それぞれのスクリプトで実行
    }
    //攻撃を適用したいときの処理
    public virtual void HitAttack(GameObject enemyObj)
    {
        if(enemyObj == lockObj)
            questController.SetPCAttackEvent(this, enemyObj.GetComponent<EnemyController>());
    }
    //実際に攻撃が適用された際の処理（EnemyへのダメージはQuestControllerから行う）
    public virtual void ApplyHitAttack(bool isEnemyDied)
    {
        cs.GetSP(1);
        spSlider.value = cs.nowSP;
        if(isEnemyDied) SetState(State.Idle);
    }
    public bool Attacked(int damage)
    {
        if (cs.isDied()) return true;
        cs.Damage(damage);
        hpSlider.value = cs.nowHP;
        audioManager.Damage1();
        if(cs.isDied())
        {
            SetState(State.Die);
            return true;
        }
        else
        {
            //仰け反りアクションがない方がよさげなのでコメントアウト中
            //animator.SetTrigger("Damage");
            return false;
        }
    }
    public virtual void Skill()
    {
        /*キャラ毎のスクリプトで実行*/
        questController.ResumeBattle();//スキル処理を設定していない場合は、何もせずにバトル続行
    }
    public void Healed(int heal)
    {
        cs.Heal(heal);
        hpSlider.value = cs.nowHP;
    }

    public virtual void SetMovePosi(Vector3 posi)
    {
        movePosi = posi;
        movePosi.y = this.transform.position.y;
        if(Vector3.Distance(transform.position,movePosi) <= 2.5f) return;
        this.transform.LookAt(movePosi);
        SetState(State.Move);
    }
    public virtual void LockEnemy(GameObject obj)
    {
        if (obj == null)
        {
            SetState(State.Idle);
            return;
        }
        this.transform.LookAt(obj.transform.position);
        SetState(State.Lock);
        lockObj = obj;
    }

    //PCを手持ちに戻すとき
    public void Return()
    {
        waitingTime = Define.waitTime;
        gameObject.transform.position = Define.initialPosi;
        gameObject.GetComponent<CharaController>().animator.enabled = false;
        charaIconController.SetWaitGageImage();
        questController.ReturnPC();
        StartCoroutine("AutoHeal");
        SetState(State.Wait);
    }

    public void SetStateNone()
    {
        SetState(State.None);
    }
    //召喚時のanimation内で実行
    void EndAppearAnimation()
    {
        StopCoroutine("AutoHeal");
        SetState(State.Idle);
    }

    #region PCの状態判定用関数
    public bool IsInField()
    {
        return ( state != State.Icon && state != State.Wait && state != State.Die );
    }
    public bool IsStateIcon()
    {
        return state == State.Icon;
    }
    public bool IsStateWait()
    {
        return state == State.Wait;
    }
    public bool IsStateNone()
    {
        return state == State.None;
    }
    public bool IsMaxSP()
    {
        return ( cs.nowSP == cs.maxSP);
    }
    public bool IsAuto()
    {
        return auto;
    }
    #endregion

    #region バフのエフェクト用関数
    public void SetBuffEffect()
    {
        buffNum++;
        if (buffNum > 1) return; //既にバフが存在する場合はスキップ
        buffEffect = Instantiate(BuffEffect, new Vector3(0, 0, 0), Quaternion.Euler(-90f, 0f, 0f));
        buffEffect.transform.SetParent(this.transform, false);
    }
    public void RemoveBuffEffect()
    {
        buffNum--;
        if (buffNum > 0) return; //複数バフが存在する場合は、パーティクル継続
        Destroy(buffEffect);
    }
    #endregion

    //手持ちにいるときの自動回復
    IEnumerator AutoHeal()
    {
        while(true)
        {
            yield return new WaitForSeconds(Define.healTime);

            Healed(Define.healValue);
        }
    }

    //Animation内で実行
    void Desporn()
    {
        gameObject.SetActive(false);
    }
    public void QuestClear()
    {
        auto = false;
        gameObject.GetComponent<CharaController>().animator.enabled = true;
        SetState(State.Idle);
        transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        animator.SetTrigger("Clear");
    }

    public void SetStatus(Chara_Info chara)
    {
        cs.SetStatus(chara);
    }
}
