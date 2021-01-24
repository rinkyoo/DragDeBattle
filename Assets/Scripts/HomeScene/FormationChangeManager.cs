using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using StateManager;
using QuestCommon;

//変更する編成の選択画面を管理
public class FormationChangeManager : MonoBehaviour
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

    //編成情報を取得、保存する用
    private CharaInfoManager charaInfoManager;
    //SE用
    AudioManager audioManager;
    
    //編成キャラ情報を保持
    private Chara_Info[][] formationChara = new Chara_Info[Define.ptNum][];
    
    float preXPosi; //編成Panelのドラッグ時に使用
    int nowFormation; //画面中心に表示中の編成番号
    int changeFormation; //キャラ変更を行う編成番号
    int changeNumber; //変更するキャラの編成内での番号

    const float dragMaxValue = 840f; //編成パネルのドラッグ可能範囲
    const float slideXValue = 420f; //編成パネルを切り替える基準値

    #region GameObject関連
    [SerializeField] GameObject[] charaPanel;//編成キャラを表示するPanel// = new GameObject[Define.ptNum];
    [SerializeField] GameObject formationPanel;
    [SerializeField] Button CharaButton; //変更するキャラを選択するボタン
    [SerializeField] Image shutterImage; //画面切り替え時に使用
    #endregion

    void Awake()
    {
        charaInfoManager = GameObject.Find("CharaInfoManager").GetComponent<CharaInfoManager>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();

        //編成キャラ用・ジャグ配列の初期化
        for (int i=0;i<Define.ptNum;i++)
        {
            formationChara[i]=new Chara_Info[Define.charaNum];
        }
    }
    
    void Start()
    {
        SetFormation();
    }
    //編成選択画面の初期設定
    public void SetFormation()
    {
        formationChara = charaInfoManager.GetFormationChara();
        GameObject charaButton;

        nowFormation = 0;
        for(int i=0;i<Define.charaNum-1;i++)
        {
            //ボタンに各キャラの画像を表示
            charaButton = charaPanel[1].transform.Find("Chara"+(i+1).ToString()).gameObject;
            //pc_icon_texture = formationChara[0][i].Icon;
            charaButton.GetComponent<Image>().sprite = formationChara[0][i].Icon;
            //変更するキャラのボタンが押された時の処理
            int j = i;
            charaButton.GetComponent<Button>().onClick.AddListener(()=>
            {
                changeFormation = 0;
                changeNumber = j; //変更キャラの番号を格納
                gameObject.GetComponent<CharaChangeManager>().SetPanel(formationChara[0],j); //キャラ選択画面へ移行
                audioManager.Button1();
            });
        }
        charaPanel[1].transform.Find("FormationName").gameObject.GetComponent<Text>().text = "＜編成"+(0+1).ToString()+"＞";
        
        SetRightFormation();
        SetLeftFormation();
        
    }
    
    //表示中の編成パネルの右側に位置するパネルをセット
    void SetRightFormation()
    {
        GameObject charaButton;
        Sprite pc_icon_texture;
        int temp;
        if(nowFormation == Define.ptNum-1) temp = 0;
        else temp = nowFormation+1;
        for(int i=0;i<Define.charaNum-1;i++)
        {
            charaButton = charaPanel[2].transform.Find("Chara"+(i+1).ToString()).gameObject;
            //pc_icon_texture = Resources.Load("PC_Image/"+formationChara[temp][i].Icon,typeof(Sprite)) as Sprite;
            pc_icon_texture = formationChara[temp][i].Icon;
            charaButton.GetComponent<Image>().sprite = pc_icon_texture;
            charaButton.GetComponent<Button>().onClick.RemoveAllListeners(); //処理が重複しないようにマウスイベントを全消去
            int j = i;
            charaButton.GetComponent<Button>().onClick.AddListener(()=>
            {
                changeFormation = temp;
                changeNumber = j; //変更キャラの番号を格納
                gameObject.GetComponent<CharaChangeManager>().SetPanel(formationChara[temp],j); //キャラ選択画面へ移行
                audioManager.Button1();
            });
        }
        charaPanel[2].transform.Find("FormationName").gameObject.GetComponent<Text>().text = "＜編成"+(temp+1).ToString()+"＞";
    }
    //表示中の編成パネルの左側に位置するパネルをセット
    void SetLeftFormation()
    {
        GameObject charaButton;
        int temp;
        if(nowFormation == 0) temp = Define.ptNum-1;
        else temp = nowFormation-1;
        for(int i=0;i<Define.charaNum-1;i++)
        {
            charaButton = charaPanel[0].transform.Find("Chara"+(i+1).ToString()).gameObject;
            charaButton.GetComponent<Image>().sprite = formationChara[temp][i].Icon;
            charaButton.GetComponent<Button>().onClick.RemoveAllListeners(); //処理が重複しないようにマウスイベントを全消去
            int j = i;
            charaButton.GetComponent<Button>().onClick.AddListener(()=>
            {
                changeFormation = temp;
                changeNumber = j; //変更キャラの番号を格納
                gameObject.GetComponent<CharaChangeManager>().SetPanel(formationChara[temp],j); //キャラ選択画面へ移行
                audioManager.Button1();
            });
        }
        charaPanel[0].transform.Find("FormationName").gameObject.GetComponent<Text>().text = "＜編成"+(temp+1).ToString()+"＞";
    }
    
    public void ChangeChara(Chara_Info newChara)
    {
        //キャラの入れ替え
        formationChara[changeFormation][changeNumber] = newChara;
        //パネル表示の更新
        GameObject charaButton;
        charaButton = charaPanel[1].transform.Find("Chara"+(changeNumber+1).ToString()).gameObject;
        charaButton.GetComponent<Image>().sprite = formationChara[changeFormation][changeNumber].Icon;
    }
    
    public void BeginDrag()
    {
        touch();
        preXPosi = touch_manager.touch_position.x;
    }
    //マウスドラッグにあわせてパネルを移動
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
    //ドラッグ終了時のパネル位置で、表示編成を変更
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
            Sequence Seq = DOTween.Sequence();
            for(int i=0;i<3;i++)
            {
                Seq.Join(charaPanel[i].transform.DOLocalMoveX(-dragMaxValue + (i*dragMaxValue),0.5f));
            }
            Seq.Play();
        }
    }
    //編成パネルの移動
    void RightMove()
    {
        if(nowFormation == 0) nowFormation = Define.ptNum-1;
        else nowFormation -= 1;
        
        Sequence Seq = DOTween.Sequence();
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
    void LeftMove()
    {
        if(nowFormation == Define.ptNum-1) nowFormation = 0;
        else nowFormation += 1;
        
        Sequence Seq = DOTween.Sequence();
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
    
    public void FormationClicked()
    {
        audioManager.Button1();
        formationPanel.SetActive(true);
    }
    public void BackHomeClicked()
    {
        for (int i = 0; i < Define.ptNum; i++)
        {
            charaInfoManager.SaveFormationInfo(formationChara[i], i + 1);
        }
        audioManager.Button1();
        formationPanel.SetActive(false);
    }
}
