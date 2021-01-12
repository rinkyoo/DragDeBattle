using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class IconTouchUI : MonoBehaviour
{
    private QuestController questController;
    private CharaManager cm;
    Chara_Info charaInfo;
    CharaController cc;
    
    [SerializeField] public GameObject PCInfoPanel;
    [SerializeField] Image IconImage;
    [SerializeField] TextMeshProUGUI PCName;
    [SerializeField] TextMeshProUGUI HP;
    [SerializeField] TextMeshProUGUI SP;
    [SerializeField] TextMeshProUGUI OtherStatus;
    [SerializeField] TextMeshProUGUI SkillInfo;
    [SerializeField] Button SkillButton; //スキル発動用ボタン
    [SerializeField] Button ReturnButton; //手持ちにPCを戻す用ボタン

    [SerializeField] GameObject SkillPanel;
    [SerializeField] TextMeshProUGUI SkillText;
    [SerializeField] TextMeshProUGUI SkillInfoText;
    
    [HideInInspector] public bool skillTextFlag = false;
    [SerializeField] Image SkillIconImage;
    
    void Start()
    {
        questController = GameObject.Find("QuestController").GetComponent<QuestController>();
        cm = GameObject.Find("CharaScript").GetComponent<CharaManager>();
    }
    
    void Update()
    {
        //スキル発動時の演出（timeScaleが０なので、unscaledDeltaTimeで操作）
        if(skillTextFlag)
        {
            Vector3 temp;
            if(SkillPanel.transform.localPosition.x < -30f)
            {
                temp = SkillPanel.transform.localPosition;
                temp.x += Time.unscaledDeltaTime * 1500f;
                SkillPanel.transform.localPosition = temp;
            }
            else if(SkillPanel.transform.localPosition.x < 30f)
            {
                temp = SkillPanel.transform.localPosition;
                temp.x += Time.unscaledDeltaTime * 50f;
                SkillPanel.transform.localPosition = temp;
            }
            else if(SkillPanel.transform.localPosition.x < 1000f)
            {
                temp = SkillPanel.transform.localPosition;
                temp.x += Time.unscaledDeltaTime * 1500f;
                SkillPanel.transform.localPosition = temp;
            }
            else if(SkillPanel.transform.localPosition.x > 1000f)
            {
                cc.Skill();
                skillTextFlag = false;
            }
        }
    }
    //タップしたアイコンのキャラ情報をパネルに反映し、表示
    public void SetCharaInfoUI(string name)
    {
        charaInfo = cm.GetCharaInfo(name);
        cc = cm.GetCharaController(name);
        
        IconImage.sprite = charaInfo.Icon;
        string temp;
        PCName.text = charaInfo.Name;
        HP.text = cc.cs.nowHP.ToString() + "  /  " + cc.cs.maxHP.ToString();
        SP.text = cc.cs.nowSP.ToString() + "  /  " + cc.cs.maxSP.ToString();
        OtherStatus.text = "攻撃力 : " + cc.cs.str.ToString() + "        防御力 : " + cc.cs.vit.ToString()
                         + "\n移動速度 : " + cc.cs.agi.ToString() + "    攻撃速度 : " + cc.cs.speedAtk.ToString();
        SkillInfo.text = charaInfo.SkillInfo;
        
        if(cc.IsMaxSP() && cc.IsInField())
        {
            SkillButton.GetComponent<Image>().color = new Color(1f,1f,1f,1f);
            SkillButton.interactable = true;
        }
        else
        {
            SkillButton.GetComponent<Image>().color = new Color(1f,1f,1f,0.5f);
            SkillButton.interactable = false;
        }
        
        if(cc.IsInField())
        {
            ReturnButton.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
            ReturnButton.interactable = true;
        }
        else
        {
            ReturnButton.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.5f);
            ReturnButton.interactable = false;
        }

        PCInfoPanel.SetActive(true);
    }
    
    public void BackClicked()
    {
        PCInfoPanel.SetActive(false);
        questController.ResumeBattle();
    }
    
    public void SkillClicked()
    {
        //スキル情報を表示するパネルの設定
        SkillPanel.transform.localPosition = new Vector3(-1000f,500f,0f);
        SkillIconImage.sprite = charaInfo.Icon;
        SkillText.text = charaInfo.Name + "\n     スキル発動！！";
        SkillInfoText.text = charaInfo.SkillInfo;
        
        skillTextFlag = true;
        PCInfoPanel.SetActive(false);
    }

    public void ReturnClicked()
    {
        cc.Return();
        BackClicked();
    }
}
