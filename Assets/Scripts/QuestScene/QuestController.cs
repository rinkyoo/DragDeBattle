using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;
using DG.Tweening;
using StateManager;
using QuestCommon;
using System;

public class QuestController : MonoBehaviour
{
    #region マウスクリックと画面タップ両方に対応するため
    TouchManager touch_manager = new TouchManager();
    TouchManager touch_state;
    void touch()
    {
        touch_manager.update();
        touch_state = touch_manager.getTouch();
    }
    #endregion

    #region スクリプト宣言
    private DataHolder dataHolder;
    private CharaManager charaManager;
    private EnemyManager enemyManager;
    private QuestClearManager questClearManager;
    private FieldEffect fieldEffect;
    private AudioManager audioManager;
    #endregion

    #region  敵キャラの情報
    private List<Enemy_Wave> enemyWave;
    int waveNum;     //合計ウェーブ数
    int nowWave = 0; //現在のゲーブ数
    #endregion

    //攻撃イベントをキューで保持しておき、LateUpdate()で順番に反映させる。
    Queue<Action> attackEventQueue = new Queue<Action>();

    #region 画面タップ時のray関連
    private RaycastHit hit;
    int PCLayerMask = 1 << 10;
    int fANDeLayerMask = 1 << 9 | 1 << 11; //FieldとEnemy用
    #endregion

    #region 画面タップ用変数（PCをタップする時の）
    Vector3 befoPosi = new Vector3();
    Vector3 movePosi;
    bool PCTouching = false;
    bool enemyTouchFlag = false; //ドラッグ終了時に、敵キャラをロックしているかの判定用
    bool healerTouchFlag = false; //ドラッグ中のPCがヒーラーキャラかの判定用
    bool isDieProcessing = false; //敵キャラが同時に死亡した際に必要、処理を１回のみにしたいから
    string pcName; //PC+キャラ番号
    string enemyName; //Enemy+敵番号
    private float borderDownPosi;
    private float borderLeftPosi;
    private float borderRightPosi;
    #endregion

    #region 画面演出に使用するオブジェクト
    [SerializeField] GameObject GoHomePanel;
    [SerializeField] GameObject sceneLoadPanel;
    private CanvasGroup sceneLoadCanvasGroup;
    [SerializeField] GameObject PCDieImage;
    [SerializeField] GameObject tapText;
    [SerializeField] GameObject questFinCamera;
    [SerializeField] GameObject questFinManager;
    #endregion

    public GameObject clickBlock; //画面クリックを無効にする用

    [SerializeField] Toggle battleSpeedToggle; //バトルスピードの切り替え用
    private float timeScale = 1f;

    private int minute = 0;
    private float second = 0;

    #region ドロップ関連
    private int totalExp = 0; //獲得プレイヤー経験値
    private int totalCoin = 0; //獲得コイン
    private List<ExpItem_Info> expItemList = new List<ExpItem_Info>(); //獲得経験値アイテム
    [SerializeField] GameObject dropItemParticle;
    Transform dropParent;
    #endregion

    void Awake()
    {
        dataHolder = GameObject.Find("DataHolder").GetComponent<DataHolder>();
        charaManager = GameObject.Find("CharaScript").GetComponent<CharaManager>();
        enemyManager = GameObject.Find("EnemyScript").GetComponent<EnemyManager>();
        questClearManager = GameObject.Find("QuestClearManager").GetComponent<QuestClearManager>();
        fieldEffect = GameObject.Find("FieldEffect").GetComponent<FieldEffect>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        sceneLoadCanvasGroup = sceneLoadPanel.GetComponent<CanvasGroup>();
    }

    void Start()
    {
        charaManager.SetQuestChara(dataHolder.GetFormationChara()); //HomeSceneで選択した編成情報を取得
        enemyWave = dataHolder.GetQuestEnemy().GetWaveList(); //敵キャラの情報を取得
        waveNum = enemyWave.Count();
        SetBattleSpeedToggle();
        audioManager.QuestBGM();
        dropParent = new GameObject("DropParent").transform;

        #region フィールドとUIとの境目の座標を取得
        CanvasInfo canvasInfo = GameObject.Find("QuestCanvas").GetComponent<CanvasInfo>();
        borderDownPosi = canvasInfo.GetBorderDownPosi();
        borderLeftPosi = canvasInfo.GetBorderLeftPosi();
        borderRightPosi = canvasInfo.GetBorderRightPosi();
        #endregion

        #region 画面を明転かつ、敵キャラ配置
        Sequence seq = DOTween.Sequence();
        seq.AppendCallback(() =>
            {
                sceneLoadPanel.SetActive(true);
                sceneLoadCanvasGroup.alpha = 1f;
                SetQuestEnemy(); //最初の敵キャラを配置
                StartCoroutine("LockPCCoroutine"); //EnemyにPCを自動ロックさせる
            })
            .Append(sceneLoadCanvasGroup.DOFade(0f, 0.5f))
            .AppendCallback(() =>
            {
                sceneLoadPanel.SetActive(false);
            });
        seq.Play();
        #endregion
    }

    #region Start()内で実行する関数
    //Enemy死亡時にも実行する ==========================================
    public void SetQuestEnemy()
    {
        //現在が最終waveかの判定
        if (nowWave >= waveNum)
        {
            //Fieldに敵キャラが０ならクエストクリア
            if (enemyManager.GetInFieldNum() == 0)
            {
                QuestClear();
            }
            return;
        }

        //同時にEnemyが死亡したときに、処理の重複を避けるため
        if (isDieProcessing) return;

        Enemy_Wave wave = enemyWave[nowWave];
        if (wave.TIMING >= enemyManager.GetInFieldNum())
        {
            isDieProcessing = true;
            Invoke("SetQuestEnemySub", wave.TIME);
        }
    }
    void SetQuestEnemySub()
    {
        Enemy_Wave wave = enemyWave[nowWave];
        for (int i = 0; i < wave.Info.Count(); i++)
        {
            enemyManager.SetEnemy(wave.Info[i], wave.Posi[i]);
        }
        nowWave++;
        isDieProcessing = false;
    }
    //==================================================================

    void SetBattleSpeedToggle()
    {
        battleSpeedToggle.onValueChanged.AddListener((bool isOn) =>
        {
            if (isOn)
            {
                timeScale = 2f;
                battleSpeedToggle.gameObject.GetComponent<CanvasGroup>().alpha = 1f;
            }
            else
            {
                timeScale = 1f;
                battleSpeedToggle.gameObject.GetComponent<CanvasGroup>().alpha = 0.5f;
            }
            if(Time.timeScale != 0) Time.timeScale = timeScale;
        });
    }

    //0.5秒毎にEnemyのロック対象を更新 +++++++++++++++++++++++++++++++++
    IEnumerator LockPCCoroutine()
    {
        for (; ; )
        {
            LockPC();
            yield return new WaitForSeconds(0.5f);
        }
    }
    public void LockPC()
    {
        enemyManager.CheckLockPC();
        for (int i = 0; i < Define.charaNum; i++)
        {
            GameObject pc = charaManager.GetPC(i);
            if (pc != null) enemyManager.LockPC(pc);
        }
    }
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    #endregion

    //ここで、フィールド上にいるPCへのタップ操作を行う
    void Update()
    {
        touch();
        if(touch_state.touch_flag)
        {
            /*タップ開始時の処理*/
            if (touch_state.touch_phase == TouchPhase.Began && Time.timeScale != 0f)
            {   
                Vector3 posi = touch_manager.touch_position;
                Vector3 worldPosi = Camera.main.ScreenToWorldPoint(posi);
                Ray ray = new Ray(worldPosi,Camera.main.transform.forward);
                foreach(RaycastHit hit in Physics.RaycastAll(ray,PCLayerMask))
                {
                    //Field内のPCをタップした場合
                    if(hit.transform.tag == "PC_Field")
                    {
                        pcName = hit.transform.name;
                        CharaController cc = charaManager.GetCharaController(pcName);
                        if (cc == null || cc.IsAuto() || !cc.IsInField()) return;

                        PauseBattle();
                        PCTouching = true;
                        befoPosi = hit.transform.position;
                        fieldEffect.SetLineRen(befoPosi);
                        //ヒーラーの場合はFlagを立てる（ヒーラーのみPCもロック対象にできるから）
                        if(hit.transform.gameObject.GetComponent<CharaController>().isHealer)
                        {
                            healerTouchFlag = true;
                        }
                        else
                        {
                            healerTouchFlag = false;
                        }
                    }
                }
            }
            /*ドラッグ中の処理*/
            if (touch_state.touch_phase == TouchPhase.Moved)
            {
                if(PCTouching){
                    Vector3 posi = touch_manager.touch_position;
                    Vector3 worldPosi = Camera.main.ScreenToWorldPoint(posi);
                    Ray ray = new Ray(worldPosi,Camera.main.transform.forward);
                    bool flag = false;

                    foreach (RaycastHit hit in Physics.RaycastAll(ray,fANDeLayerMask))
                    {
                        //PCから出る矢印的なものを更新
                        if(hit.transform.gameObject.CompareTag("Field"))
                        {
                            fieldEffect.UpdateLineRen(hit.point);
                            movePosi = hit.point;
                        }
                        //敵キャラに触れてる場合ー＞エフェクト表示
                        if(hit.transform.tag == "Enemy")
                        {
                            flag = true;
                            enemyName = hit.transform.name;
                            fieldEffect.UpdateEnemyLock(hit.transform.position);
                        }
                        //ヒーラーキャラの場合は、味方PCもロック対象のため
                        if(healerTouchFlag && hit.transform.tag == "PC_Field")
                        {
                            flag = true;
                            enemyName = hit.transform.name;
                            fieldEffect.UpdateEnemyLock(hit.transform.position);
                        }
                    }
                    if(flag)
                    {
                        enemyTouchFlag = true;
                    }
                    else
                    {
                        enemyTouchFlag = false;
                        fieldEffect.HideEnemyLock();
                    }
                }
            }
            /*タップ終了時の処理*/
            if (touch_state.touch_phase == TouchPhase.Ended)
            {
                if(PCTouching)
                {
                    ResumeBattle();
                    PCTouching = false;
                    fieldEffect.DeleteDragEffect();
                    if(enemyTouchFlag)
                    {                        
                        //ヒーラーキャラのみ味方をロック可能
                        if (healerTouchFlag && enemyName.Contains("PC"))
                        {
                            charaManager.LockEnemy(pcName, charaManager.GetPC(enemyName));
                        }
                        else
                        {
                            charaManager.LockEnemy(pcName, enemyManager.GetEnemy(enemyName));
                        }
                    }
                    else
                    {
                        var posi = touch_manager.touch_position;
                        if (posi.y > borderDownPosi && posi.x > borderLeftPosi && posi.x < borderRightPosi)
                            charaManager.SetMovePosi(pcName,movePosi);
                    }
                }
            }
        }
        //クエスト経過時間の計算
        second += Time.deltaTime;
        if (second >= 60f)
        {
            minute++;
            second = second - 60f;
        }
    }

    void LateUpdate()
    {
        while(attackEventQueue.Count > 0)
        {
            Action attackEvent = attackEventQueue.Dequeue();
            attackEvent();
        }
    }
    #region attackEventQueueに攻撃イベントを保存する関数
    public void SetPCAttackEvent(CharaController cc, EnemyController ec)
    {
        attackEventQueue.Enqueue(() =>
        {
            if (ec.IsInField())
            {
                //Enemyにダメージを反映・それで死亡した場合true取得
                bool isEnemyDied = ec.Attacked(cc.cs.str);
                cc.ApplyHitAttack(isEnemyDied);
            }
        });
    }
    public void SetEnemyAttackEvent(EnemyController ec, CharaController cc)
    {
        attackEventQueue.Enqueue(() =>
        {
            if (cc.IsInField())
            {
                //PCにダメージを反映・それで死亡した場合true取得
                bool isPCDied = cc.Attacked(ec.es.str);
                ec.ApplyHitAttack(isPCDied);
            }
        });
    }
    #endregion

    public void SetPCInField()
    {
        charaManager.SetPCInField();
    }

    public void DespornPC()
    {
        charaManager.Desporn();
        //PC死亡時の演出
        Time.timeScale = 0.5f;
        audioManager.PCDie();
        PCDieImage.SetActive(true);
        StartCoroutine(DelayMethod(0.5f,()=>
        {
            PCDieImage.SetActive(false);
            Time.timeScale = timeScale;
        }));
        //フィールド上のPCが０の場合クエスト失敗
        if (charaManager.GetInFieldNum() <= 0)
        {
            QuestFail();
        }
    }
    //PCを手持ちに戻す処理
    public void ReturnPC()
    {
        charaManager.Return();
        if (charaManager.GetInFieldNum() <= 0)
        {
            QuestFail();
        }
    }

    public void DespornEnemy()
    {
        enemyManager.Desporn();
        SetQuestEnemy();
    }

    void QuestClear()
    {
        clickBlock.SetActive(true);

        string clearTime = minute.ToString() + "分" + second.ToString("F2") + "秒";

        audioManager.PauseBGM();
        audioManager.LastEnemyDie();

        Time.timeScale = 0.5f;
        StartCoroutine(DelayMethod(1.5f, () =>
        {
            Time.timeScale = 1f;
            audioManager.UnPauseBGM();
            questClearManager.QuestClear(clearTime, totalExp, totalCoin, expItemList);
            charaManager.QuestClear();
        }));
    }
    void QuestFail()
    {
        audioManager.PauseBGM();
        audioManager.QuestFailed();
        
        Time.timeScale = 0.5f;
        StartCoroutine(DelayMethod(1f, () =>
        {
            Time.timeScale = 1f;
            SetTapToHome();
            audioManager.UnPauseBGM();
        }));
    }
    void SetTapToHome()
    {
        GoHomePanel.SetActive(true);
        CanvasGroup group = GoHomePanel.GetComponent<CanvasGroup>();
        group.DOFade(1f, 1f);
        Sequence Seq = DOTween.Sequence();
        Seq.Append(tapText.transform.DOScale(new Vector3(1.3f, 1.3f, 1.3f), 1f))
        .Append(tapText.transform.DOScale(new Vector3(1f, 1f, 1f), 1f))
        .SetLoops(-1);
        Seq.Play();
    }

    public void PauseBattle()
    {
        Time.timeScale = 0f;
    }
    public void ResumeBattle()
    {
        Time.timeScale = timeScale;
    }

    public void StartLockPCCoroutine()
    {
        StartCoroutine("LockPCCoroutine");
    }

    public GameObject GetNearEnemy(Vector3 pcPosi)
    {
        return enemyManager.GetNearEnemy(pcPosi);
    }

    public void GoHomeButtonClicked()
    {
        Sequence seq = DOTween.Sequence();
        seq.AppendCallback(() =>
        {
            sceneLoadPanel.SetActive(true);
            sceneLoadCanvasGroup.alpha = 0f;
        })
        .Append(sceneLoadCanvasGroup.DOFade(1f, 0.5f))
        .AppendCallback(() =>
        {
            SceneManager.LoadScene("Home");
        });
        seq.Play();
    }

    /*
    全敵キャラのロックPCを強制的に引数に設定
    Mieのキャラスキル用に定義したが、１キャラのためだけにここで定義していいのかは一考！
    */
    public void ForcedLockPC(GameObject pc)
    {
        StopCoroutine("LockPCCoroutine");
        enemyManager.ForcedLockPC(pc);
    }

    private IEnumerator DelayMethod(float waitTime, Action action)
    {
        yield return new WaitForSeconds(waitTime);
        action();
    }

    public void SetDropItem(Vector3 setPosi,string itemName)
    {
        GameObject dropItem = null;
        //非アクティブなオブジェクトがある場合は再利用
        foreach (Transform t in dropParent)
        {
            if (!t.gameObject.activeSelf)
            {
                dropItem = t.gameObject;
                dropItem.transform.position = setPosi;
                dropItem.SetActive(true);
                break;
            }
        }
        //非アクティブなオブジェクトがない場合は生成
        if (dropItem == null)
            dropItem = Instantiate(dropItemParticle, setPosi, Quaternion.identity, dropParent);

        ParticleSystem.MainModule par;
        switch (itemName)
        {
            case "coin":
                par = dropItem.GetComponent<ParticleSystem>().main;
                par.startColor = Color.yellow;
                break;
            case "exp":
                par = dropItem.GetComponent<ParticleSystem>().main;
                par.startColor = Color.red;
                break;
        }
    }
    public void PlusExp(int exp)
    {
        totalExp += exp;
    }
    public void PlusCoin(int coin)
    {
        totalCoin += coin;
    }
    public void PlusExpItem(ExpItem_Info expItem)
    {
        expItemList.Add(expItem);
    }

    public int GetFieldPCNum()
    {
        return charaManager.GetInFieldNum();
    }
}
