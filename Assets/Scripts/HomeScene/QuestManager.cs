using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;
using StateManager;
using QuestCommon;
using AccountCommonData;

public class QuestManager : MonoBehaviour
{
    #region 画面タッチ関連
    //マウスとタップ両方に適応するため
    TouchManager touch_manager = new TouchManager();
    TouchManager touch_state;
    void touch()
    {
        touch_manager.update();
        touch_state = touch_manager.getTouch();
    }
    #endregion

    //QuestSceneでも使用するデータを保持
    private DataHolder dataHolder;
    //編成情報を取得、保存する用
    private CharaInfoManager charaInfoManager;
    //SE用
    AudioManager audioManager;

    //クエスト情報をInspectorから設定*****************************
    [System.SerializableAttribute]
    public class QuestList
    {
        public List<Quest_Group> questList = new List<Quest_Group>();
    }
    [SerializeField] List<QuestList> allQuestGroup;
    //************************************************************
    //編成するサポートキャラを保持
    private Chara_Info supportChara;
    //編成キャラ情報
    private Chara_Info[][] formationChara = new Chara_Info[Define.ptNum][];

    Sequence Seq;
    float preXPosi; //編成Panelのドラッグ時に使用
    int nowFormation; //表示中の編成番号
    const float dragMaxValue = 840f; //編成パネルのドラッグ可能範囲
    const float slideXValue = 320f; //編成パネルの切り替え基準値

    private AsyncOperation async;

    #region GameObject関連
    [SerializeField] GameObject[] charaPanel = new GameObject[Define.ptNum];
    [SerializeField] GameObject QuestPanel;
    [SerializeField] GameObject groupView;
    [SerializeField] GameObject groupContent;
    [SerializeField] GameObject questView;
    [SerializeField] GameObject questContent;
    [SerializeField] Button QButton;
    [SerializeField] GameObject supportView;
    [SerializeField] GameObject supportContent;
    [SerializeField] Button SButton;
    [SerializeField] GameObject supportIcon;
    [SerializeField] GameObject formationSelectPanel;
    [SerializeField] GameObject sceneLoadPanel;
    [SerializeField] Slider scenLoadSlider;
    #endregion

    void Awake()
    {
        dataHolder = GameObject.Find("DataHolder").GetComponent<DataHolder>();
        charaInfoManager = GameObject.Find("CharaInfoManager").GetComponent<CharaInfoManager>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }

    void QuestInit(string questType)
    {
        int questTypeNum = AccountDefine.questType[questType];
        List<Quest_Group> questGroup = allQuestGroup[questTypeNum].questList;
        dataHolder.SetAccountManager();

        //編成キャラ用・ジャグ配列の初期化
        for (int formNum = 0; formNum < Define.ptNum; formNum++)
        {
            formationChara[formNum] = new Chara_Info[Define.charaNum];
        }

        #region クリア済みのクエストレベル（clearedLevel）を取得
        int[] clearedQuest = dataHolder.GetClearedQuest(questType);
        int clearedLevel = clearedQuest[0];
        if (clearedLevel <= questGroup.Count && clearedLevel > 0)
        {
            if (clearedQuest[1] != questGroup[clearedLevel - 1].GetQuestList().Count)
            {
                clearedLevel -= 1;
            }
        }
        #endregion
        //グループ選択画面のボタンを設定
        int i =0;
        foreach (Quest_Group group in questGroup)
        {
            i++;
            Button levelButton = Instantiate(QButton,groupContent.transform.position,Quaternion.identity) as Button;
            levelButton.GetComponentInChildren<TextMeshProUGUI>().text = group.SetName;
            levelButton.transform.SetParent(groupContent.transform);
            levelButton.transform.localScale = new Vector3(1f, 1f, 1f);
            #region 全クリしてないレベルボタンの設定
            if (i > clearedLevel)
            {
                levelButton.transform.Find("ClearedImage").gameObject.SetActive(false);
                if (i > clearedLevel + 1)
                {
                    levelButton.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.6f);
                    levelButton.enabled = false;
                }
            }
            #endregion
            Quest_Group tempGroup = group;
            int j=i;
            string tempType = questType;
            levelButton.onClick.AddListener(()=>
            {
                DeleteChildren(questContent.transform);
                SetQuestView(tempGroup.GetQuestList(),j,tempType);
            });
        }
        
    }

    void SetQuestView(List<Quest_Data> quests,int level,string questType)
    {
        //クエスト選択画面のボタンを設定
        int i =0;
        int[] clearedQuest = dataHolder.GetClearedQuest(questType);
        foreach (Quest_Data quest in quests)
        {
            i++;
            Button questButton = Instantiate(QButton,questContent.transform.position,Quaternion.identity) as Button;
            questButton.GetComponentInChildren<TextMeshProUGUI>().text = quest.QuestName;
            questButton.transform.SetParent(questContent.transform);
            questButton.transform.localScale = new Vector3(1f, 1f, 1f);
            #region クリアしてないクエストボタンの設定
            if ( (level > clearedQuest[0]) || (clearedQuest[0] == level && clearedQuest[1] < i) )
            {
                //各レベルの先頭クエストと、クリア済クエストの次のクエストは、clear表記なしで解放
                if(i == 1 || ( level == clearedQuest[0] && i == clearedQuest[1]+1) )
                {
                    questButton.transform.Find("ClearedImage").gameObject.SetActive(false);
                }
                //上記以外のクエストはロック
                else if (i != 1)
                {
                    questButton.transform.Find("ClearedImage").gameObject.SetActive(false);
                    questButton.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.6f);
                    questButton.enabled = false;
                }
            }
            #endregion
            Quest_Data setQuest = quest;
            int j = i;
            string tempType = questType;
            questButton.onClick.AddListener(()=>
            {
                DeleteChildren(supportContent.transform);
                dataHolder.SetPlayQuest(questType, level, j);
                SetQuestButton(setQuest);
            });
        }

        RightMovePanel(groupView.gameObject,questView.gameObject);
    }

    void SetQuestButton(Quest_Data quest)
    {
        dataHolder.SetQuestData(quest);
        SetSupportView();

        RightMovePanel(questView.gameObject,supportView.gameObject);
    }

    //サポート選択画面のボタンを設定
    public void SetSupportView()
    {
        List<Chara_Info> charaList = charaInfoManager.GetSupportList();//GameObject.Find("CharaInfoManager").GetComponent<CharaInfoManager>().GetSupportList();
        int i=0;
        foreach(Chara_Info chara in charaList)
        {
            Button supportButton = Instantiate(SButton,supportContent.transform.position,Quaternion.identity) as Button;
            supportButton.transform.SetParent(supportContent.transform);
            supportButton.transform.localScale = new Vector3(1f, 1f, 1f);
            supportButton.transform.SetSiblingIndex(i);
            supportButton.transform.Find("Icon").GetComponent<Image>().sprite = chara.Icon;
            supportButton.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = "サポートキャラ "+(i+1).ToString();
            Chara_Info sChara = chara;
            supportButton.onClick.AddListener(()=>
            {
                SetFormation(sChara);
                audioManager.Button1();
            });
            i++;
        }
    }

    #region 編成選択画面の設定
    public void SetFormation(Chara_Info schara)
    {
        formationChara = charaInfoManager.GetFormationChara();
        supportChara = schara; //使用するサポートキャラ情報を保持
        GameObject charaIcon;
        
        nowFormation = 0;
        for(int i=0;i<Define.charaNum-1;i++)
        {
            charaIcon = charaPanel[1].transform.Find("Chara"+(i+1).ToString()).gameObject;
            charaIcon.GetComponent<Image>().sprite = formationChara[0][i].Icon;
        }
        //サポートキャラ情報の表示
        supportIcon.GetComponent<Image>().sprite = schara.Icon;
        //編成番号の表示
        charaPanel[1].transform.Find("FormationName").gameObject.GetComponent<Text>().text = "＜編成"+(0+1).ToString()+"＞";
        
        //左右の編成を設定
        SetRightFormation();
        SetLeftFormation();
        
        //Panelの切り替え
        supportView.SetActive(false);
        formationSelectPanel.SetActive(true);
        
    }
    //表示中の編成パネルの右側に位置するパネルをセット
    void SetRightFormation()
    {
        GameObject charaIcon;
        int temp;
        if(nowFormation == Define.ptNum-1) temp = 0;
        else temp = nowFormation+1;
        for(int i=0;i<Define.charaNum-1;i++)
        {
            charaIcon = charaPanel[2].transform.Find("Chara"+(i+1).ToString()).gameObject;
            charaIcon.GetComponent<Image>().sprite = formationChara[temp][i].Icon;//pc_icon_texture;
        }
        charaPanel[2].transform.Find("FormationName").gameObject.GetComponent<Text>().text = "＜編成"+(temp+1).ToString()+"＞";
    }
    //表示中の編成パネルの左側に位置するパネルをセット
    void SetLeftFormation()
    {
        GameObject charaIcon;
        int temp;
        if(nowFormation == 0) temp = Define.ptNum-1;
        else temp = nowFormation-1;
        for(int i=0;i<Define.charaNum-1;i++)
        {
            charaIcon = charaPanel[0].transform.Find("Chara"+(i+1).ToString()).gameObject;
            charaIcon.GetComponent<Image>().sprite = formationChara[temp][i].Icon;// pc_icon_texture;
        }
        charaPanel[0].transform.Find("FormationName").gameObject.GetComponent<Text>().text = "＜編成"+(temp+1).ToString()+"＞";
    }

    //編成選択画面のドラッグ用
    public void BeginDrag()
    {
        touch();
        preXPosi = touch_manager.touch_position.x;
    }
    public void Drag()
    {
        touch();
        float tempX = charaPanel[1].transform.localPosition.x;
        tempX += touch_manager.touch_position.x - preXPosi;
        if(tempX < -dragMaxValue || tempX > dragMaxValue) return;
        for(int i=0;i<3;i++)
        {
            Vector3 temp = charaPanel[i].transform.localPosition;
            temp.x += touch_manager.touch_position.x - preXPosi;
            charaPanel[i].transform.localPosition = temp;
        }
        preXPosi = touch_manager.touch_position.x;
    }
    public void EndDrag()
    {
        if(charaPanel[1].transform.localPosition.x < -slideXValue)
        {
            LeftMove();
            
        }
        else if(charaPanel[1].transform.localPosition.x > slideXValue)
        {
            RightMove();
        }
        else
        {
            Seq = DOTween.Sequence();
            for(int i=0;i<3;i++)
            {
                Seq.Join(charaPanel[i].transform.DOLocalMoveX(-dragMaxValue + (i*dragMaxValue),0.5f));
            }
            Seq.Play();
        }
    }
    //編成Panelが右にずれる
    void RightMove()
    {
        if(nowFormation == 0) nowFormation = Define.ptNum-1;
        else nowFormation -= 1;
        
        Seq = DOTween.Sequence();
        //右方向に配列要素を移動
        GameObject tempPanel = charaPanel[1];
        charaPanel[1] = charaPanel[0];
        charaPanel[0] = charaPanel[2];
        charaPanel[2] = tempPanel;
        Seq.AppendCallback(()=>{charaPanel[0].SetActive(false);});
        for(int i=0;i<3;i++)
        {
            Seq.Join(charaPanel[i].transform.DOLocalMoveX(-840f + (i*840f),0.5f));
        }
        Seq.OnComplete(()=>
        {
            charaPanel[0].SetActive(true);
            SetLeftFormation();
        });
        Seq.Play();
    }
    //編成Panelが左にずれる
    void LeftMove()
    {
        if(nowFormation == Define.ptNum-1) nowFormation = 0;
        else nowFormation += 1;
        
        Seq = DOTween.Sequence();
        //左方向に配列要素を移動
        GameObject tempPanel = charaPanel[1];
        charaPanel[1] = charaPanel[2];
        charaPanel[2] = charaPanel[0];
        charaPanel[0] = tempPanel;
        Seq.AppendCallback(()=>{charaPanel[2].SetActive(false);});
        for(int i=0;i<3;i++)
        {
            Seq.Join(charaPanel[i].transform.DOLocalMoveX(-840f + (i*840f),0.5f));
        }
        Seq.OnComplete(()=>
        {
            charaPanel[2].SetActive(true);
            SetRightFormation();
        });
        Seq.Play();
    }
    //編成画面右側に位置するボタンの処理
    public void RightClicked()
    {
        if(charaPanel[1].transform.localPosition.x == 0)
        {
            LeftMove();
            audioManager.Button1();
        }
    }
    //編成画面左側に位置するボタンの処理
    public void LeftClicked()
    {
        if(charaPanel[1].transform.localPosition.x == 0)
        {
            RightMove();
            audioManager.Button1();
        }
    }
    #endregion

    //クエスト開始ボタン
    public void StartClicked()
    {
        charaInfoManager.SetSupportChara(supportChara, nowFormation);
        charaInfoManager.SetQuestChara(nowFormation);
        audioManager.Button1();
        sceneLoadPanel.SetActive(true);
        StartCoroutine("LoadQuestScene");
    }
    IEnumerator LoadQuestScene()
    {
        async = SceneManager.LoadSceneAsync("Quest");

        //クエストシーンの読み込みが終わるまで継続
        while (!async.isDone)
        {
            var progressVal = Mathf.Clamp01(async.progress / 0.9f);
            scenLoadSlider.value = progressVal;
            yield return null;
        }
    }

    //ホーム画面上のクエストボタンを押した際の処理
    public void NormalQuestClicked()
    {
        audioManager.Button1();
        QuestInit("normal");
        QuestPanel.SetActive(true);
    }
    public void TrainingQuestClicked()
    {
        audioManager.Button1();
        QuestInit("exp");
        QuestInit("coin");
        QuestInit("expItem");
        QuestPanel.SetActive(true);
    }

    #region 選択画面それぞれでの「戻るボタン」の設定
    //レベル選択画面ー＞ホーム画面へ遷移
    public void BackToHomeClicked()
    {
        audioManager.Button1();
        DeleteChildren(groupContent.transform);
        QuestPanel.SetActive(false);
    }
    //クエスト選択画面ー＞レベル選択画面へ遷移
    public void BackToLevelClicked()
    {
        LeftMovePanel(questView.gameObject, groupView.gameObject);
    }
    //サポート選択画面ー＞クエスト選択画面へ遷移
    public void BackToQuestClicked()
    {
        LeftMovePanel(supportView.gameObject, questView.gameObject);
    }
    //編成選択画面ー＞サポート選択画面へ遷移
    public void BackToSupportClicked()
    {
        audioManager.Button1();
        supportView.SetActive(true);
        formationSelectPanel.SetActive(false);
    }
    #endregion

    //引数のGameObjectの位置をスライド（画面遷移の際に使用）
    void RightMovePanel(GameObject nowPanel,GameObject newPanel)
    {
        audioManager.Button1();
        if (nowPanel.transform.localPosition.x != 0) return;
        Seq = DOTween.Sequence();
        Seq
        .Append(nowPanel.transform.DOLocalMoveX(828f,0.7f))
        .Join(newPanel.transform.DOLocalMoveX(0f,0.7f));
        Seq.Play();
    }
    void LeftMovePanel(GameObject nowPanel,GameObject newPanel)
    {
        audioManager.Button1();
        if (nowPanel.transform.localPosition.x != 0) return;
        Seq = DOTween.Sequence();
        Seq
        .Append(nowPanel.transform.DOLocalMoveX(-828f,0.7f))
        .Join(newPanel.transform.DOLocalMoveX(0f,0.7f));
        Seq.Play();
    }

    void DeleteChildren(Transform delete)
    {
        foreach (Transform n in delete)
        {
            GameObject.Destroy(n.gameObject);
        }
    }
}
