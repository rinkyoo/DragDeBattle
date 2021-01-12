using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Common;
using System.Diagnostics;

//編成するキャラの選択画面を管理
public class CharaChangeManager : MonoBehaviour
{
    AudioManager audioManager;
    
    List<Chara_Info> charaList; //所持キャラのリスト
    Chara_Info newChara; //新しく編成するキャラ
    [SerializeField] GameObject selectedFrame; //選択中のキャラに付ける枠
    int firstFrameNum; //画面遷移後、最初に枠を付けるキャラ番号

    [SerializeField] GameObject charaSelectPanel;
    [SerializeField] GameObject selectContent;
    [SerializeField] Button CharaButton;

    #region キャラ情報の表示用
    [SerializeField] TextMeshProUGUI NAME;
    [SerializeField] TextMeshProUGUI HP;
    [SerializeField] TextMeshProUGUI SP;
    [SerializeField] TextMeshProUGUI STR;
    [SerializeField] TextMeshProUGUI VIT;
    [SerializeField] TextMeshProUGUI AGI;
    [SerializeField] TextMeshProUGUI RANGE;
    [SerializeField] TextMeshProUGUI ATKSPEED;
    [SerializeField] TextMeshProUGUI SKILL;
    #endregion

    void Awake()
    {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }

    void Start()
    {
        charaList = GameObject.Find("CharaInfoManager").GetComponent<CharaInfoManager>().GetCharaList();
        //所持キャラを全部表示
        foreach(Chara_Info chara in charaList)
        {
            Button charaButton = Instantiate(CharaButton,selectContent.transform.position,Quaternion.identity) as Button;
            charaButton.transform.SetParent(selectContent.transform);
            charaButton.transform.localScale = new Vector3(1f, 1f, 1f);
            charaButton.GetComponent<Image>().sprite = chara.Icon;
            charaButton.onClick.AddListener(()=>
            {
                newChara = chara;
                selectedFrame.transform.position = charaButton.transform.position;
                selectedFrame.transform.SetParent(charaButton.transform);
                SetCharaInfo(newChara);
            });
        }
    }

    public void SetPanel(Chara_Info[] formedChara,int changeNumber)
    {
        //編成中のキャラは選択不可にする
        for (int i=0;i<Define.charaNum-1;i++)
        {
            int j=0;
            foreach(Chara_Info chara in charaList)
            {
                if(formedChara[i].ID == chara.ID)
                {
                    GameObject charaButton = selectContent.transform.GetChild(j).gameObject;
                    if(i == changeNumber)
                    {
                        firstFrameNum = j;
                        newChara = chara;
                        SetCharaInfo(formedChara[i]);
                    }
                    else
                    {
                        charaButton.GetComponent<Image>().color = new Color(1f,1f,1f,0.6f);
                        charaButton.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = "編成中";
                        charaButton.GetComponent<Button>().enabled = false;
                    }
                }
                j++;
            }
        }
        charaSelectPanel.SetActive(true);
        StartCoroutine("foge");
    }

    //選択中フレームを最初のキャラに設定
    //現状、１フレームずらさないと初期位置がずれるためコルーチンを使用
    //selectContentの生成前に実行がされるからだと思う
    IEnumerator foge()
    {
        yield return null;

        GameObject charaButton = selectContent.transform.GetChild(firstFrameNum).gameObject;
        selectedFrame.transform.position = charaButton.transform.position;
        selectedFrame.transform.SetParent(charaButton.transform);
        selectedFrame.SetActive(true);
    }

    void SetCharaInfo(Chara_Info chara)
    {
        NAME.text = chara.Name;
        HP.text = "最大HP : " + chara.HP;
        SP.text = "必要SP : " + chara.SP;
        STR.text = "攻撃力 : " + chara.STR;
        VIT.text = "防御力 : " + chara.VIT;
        AGI.text = "移動速度 : " + chara.AGI;
        RANGE.text = "攻撃範囲 : " + chara.RangeATK;
        ATKSPEED.text = "攻撃速度 : " + chara.SpeedATK;
        SKILL.text = chara.SkillInfo;
    }

    public void DecideChageChara()
    {
        gameObject.GetComponent<FormationChangeManager>().ChangeChara(newChara);
        BackButtonClicked();
    }

    public void BackButtonClicked()
    {
        Reset();
        selectedFrame.SetActive(false);
        charaSelectPanel.SetActive(false);
        audioManager.Button1();
    }

    //選択不可にしたキャラの設定をリセット
    void Reset()
    {
        foreach (Transform n in selectContent.transform)
        {
            n.gameObject.GetComponent<Button>().enabled = true;
            n.gameObject.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
            n.Find("Text").GetComponent<TextMeshProUGUI>().text = "";
        }
    }
    /*
    void SetFirstFrame()
    {
        GameObject charaButton = selectContent.transform.GetChild(firstFrameNum).gameObject;
        selectedFrame.transform.position = charaButton.transform.position;
        selectedFrame.transform.SetParent(charaButton.transform);
    }
    */
}
