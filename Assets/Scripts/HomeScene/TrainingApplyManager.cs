using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;
using System.Linq;
using TMPro;

public class TrainingApplyManager : MonoBehaviour
{
    private ItemManager itemManager;
    private AccountManager accountManager;
    private AudioManager audioManager;

    private EXPItemData itemCounter = new EXPItemData(); //EXPItemの使用個数を保持
    private Dictionary<string, int> expValueDic = new Dictionary<string, int>(); //アイテム名と経験値量のDic
    private Dictionary<string, int> itemCoinDic = new Dictionary<string, int>(); //アイテム名と必要コインのDic

    Chara_Info trainingChara;

    [SerializeField] GameObject trainingApplyPanel;
    [SerializeField] GameObject expItemContent;
    [SerializeField] GameObject expItemPanel;
    [SerializeField] Image charaIconImage;
    [SerializeField] TextMeshProUGUI charaNameText;
    [SerializeField] GameObject levelUpObj;
    [SerializeField] GameObject levelResetPanel;

    #region 経験値アイテム選択で使用する変数
    string clickingName = "";
    int addNum = 0; //Itemの増減値
    float clickingTime = 0; //長押し中の時間
    float longPressTime = 0.5f; //長押しを適用するまでの時間
    float countTime = 0.05f; //長押し中、Item増減を適用する間隔
    bool isClicking = false;
    bool isLongPressing = false;
    #endregion
    #region 経験値アイテムの適用で使用する変数
    int nowLevel; //キャラの現在のレベル
    int sumLevel; //アイテム使用後のレベル
    int nowEXP;  //キャラが保持する現在の経験値
    int sumEXP;  //アイテム使用後のキャラが保持する経験値
    int nextEXP;
    List<int> plusNextEXPList; //各レベルで、nextEXPに加算する値のリスト
    int expSliderMin = 0; //経験値用のSliderの下限値格納用
    int sumCoin = 0; //強化に必要なコインの合計
    [SerializeField] TextMeshProUGUI sumLevelText;
    [SerializeField] TextMeshProUGUI beforeHPText;
    [SerializeField] TextMeshProUGUI afterHPText;
    [SerializeField] TextMeshProUGUI beforeSTRText;
    [SerializeField] TextMeshProUGUI afterSTRText;
    [SerializeField] TextMeshProUGUI beforeVITText;
    [SerializeField] TextMeshProUGUI afterVITText;
    [SerializeField] TextMeshProUGUI nowEXPText;
    [SerializeField] TextMeshProUGUI nextEXPText;
    [SerializeField] TextMeshProUGUI sumCoinText;
    [SerializeField] Slider expSlider;
    #endregion

    void Awake()
    {
        itemManager = GameObject.Find("ItemManager").GetComponent<ItemManager>();
        foreach(EXPItem_Info item in itemManager.GetEXPItemList())
        {
            expValueDic.Add(item.Name, item.EXPValue);
            itemCoinDic.Add(item.Name, item.Coin);
        }
        accountManager = GameObject.Find("AccountManager").GetComponent<AccountManager>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }

    //長押し判定を行い、適宜アイテム数を加算
    void Update()
    {
        if (isClicking)
        {
            clickingTime += Time.deltaTime;
            if (!isLongPressing)
            {
                if (clickingTime > longPressTime)
                {
                    isLongPressing = true;
                    clickingTime = 0;
                }
            }
            else
            {
                if (clickingTime > countTime)
                {
                    AddNum(clickingName);
                    clickingTime = 0;
                }
            }
        }
    }

    public void SetPanel(Chara_Info chara)
    {
        trainingChara = chara;
        itemCounter = new EXPItemData();
        foreach (Transform child in expItemContent.transform)
        {
            Destroy(child.gameObject);
        }
        foreach(EXPItem_Info item in itemManager.GetEXPItemList())
        {
            //所持数が０の場合はスキップ（アイテムパネルを表示しない）
            if (itemManager.GetEXPItemNum(item.Name) <= 0) continue;

            GameObject itemPanel = Instantiate(expItemPanel,expItemContent.transform.position,Quaternion.identity) as GameObject;
            itemPanel.name = item.Name + "Panel";
            itemPanel.transform.SetParent(expItemContent.transform);
            itemPanel.transform.localScale = new Vector3(1f, 1f, 1f);
            //アイテムアイコンを表示
            itemPanel.transform.Find("IconImage").gameObject.GetComponent<Image>().sprite = item.Icon;
            //アイテムの所持数を表示
            itemPanel.transform.Find("ItemNum").gameObject.GetComponent<TextMeshProUGUI>().text = itemManager.GetEXPItemNum(item.Name).ToString();
            #region Itemの増減用Event Triggerの設定
            //Item増加用のPointerClickの設定
            string itemName = item.Name;
            EventTrigger trigger = itemPanel.transform.Find("PlusImage").gameObject.GetComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerDown;
            entry.callback.AddListener((eventData) => { PlusPointerDown(itemName); });
            trigger.triggers.Add(entry);
            //Item増加用のPointerUpの設定
            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerUp;
            entry.callback.AddListener((eventDate) => { PointerUp(); });
            trigger.triggers.Add(entry);
            //Item減少用のPointerClickの設定
            trigger = itemPanel.transform.Find("MinusImage").gameObject.GetComponent<EventTrigger>();
            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerDown;
            entry.callback.AddListener((eventData) => { MinusPointerDown(itemName); });
            trigger.triggers.Add(entry);
            //Item減少用のPointerUpの設定
            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerUp;
            entry.callback.AddListener((eventDate) => { PointerUp(); });
            trigger.triggers.Add(entry);
            #endregion
        }

        sumCoin = 0;
        sumCoinText.text = sumCoin.ToString();

        SetCharaInfo(chara);

        trainingApplyPanel.SetActive(true);
        audioManager.Button1();
    }
    
    void SetCharaInfo(Chara_Info chara)
    {
        charaIconImage.sprite = chara.Icon;
        charaNameText.text = chara.Name;
        nowLevel = sumLevel = chara.Level;
        nowEXP = sumEXP = chara.NowEXP;
        nextEXP = chara.NextEXP;
        plusNextEXPList = new List<int>() { chara.PlusNextEXP };
        /*expSliderのMinの値の設定
        厳密には値が少し違うけど、視覚的に問題はなく、
        設計上の都合によりこの設定方法のまま*/
        if(expSliderMin == 0) expSliderMin = nextEXP - chara.PlusNextEXP;
        beforeHPText.text = chara.HP.ToString();
        beforeSTRText.text = chara.STR.ToString();
        beforeVITText.text = chara.VIT.ToString();
        ChangeLevel();
    }
    
    public void PlusPointerDown(string itemName)
    {
        clickingName = itemName;
        addNum = 1;
        AddNum(clickingName);
        isClicking = true;
    }
    public void MinusPointerDown(string itemName)
    {
        clickingName = itemName;
        addNum = -1;
        AddNum(clickingName);
        isClicking = true;
    }
    public void PointerUp()
    {
        clickingName = "";
        addNum = 0;
        clickingTime = 0;
        isClicking = false;
        isLongPressing = false;
    }
    //アイテム数の加算・減算（引数で正負判定）
    void AddNum(string itemName)
    {
        //キャラがMaxLevelに達している場合はアイテム使用不可
        if (addNum > 0 && sumLevel >= trainingChara.MaxLevel) return;

        int newNum = itemCounter.EXPItemNum[itemName] + addNum;
        if (newNum < 0) newNum = 0;
        else if (newNum > itemManager.GetEXPItemNum(itemName)) newNum = itemManager.GetEXPItemNum(itemName);
        else
        {
            sumCoin += itemCoinDic[itemName] * addNum;
            sumCoinText.text = sumCoin.ToString();
            AddEXP(expValueDic[itemName] * addNum);
        }

        itemCounter.EXPItemNum[itemName] = newNum;

        //アイテム数が０ならアイテム数の表記なし
        expItemContent.transform.Find(itemName + "Panel/ApplyItemNum").gameObject.SetActive(newNum != 0);
        //増減後のアイテム数を画面に反映
        expItemContent.transform.Find(itemName + "Panel/ApplyItemNum").gameObject.GetComponent<TextMeshProUGUI>().text = itemCounter.EXPItemNum[itemName].ToString();
    }
    //キャラに経験値を加算・減算
    void AddEXP(int expValue)
    {
        if (expValue > 0 && sumLevel < trainingChara.MaxLevel)
        {
            int tempEXP = nowEXP + expValue - nextEXP;
            //レベルアップする場合
            if (tempEXP >= 0)
            {
                nowEXP = nextEXP;
                plusNextEXPList.Add((int)(Math.Ceiling(plusNextEXPList.Last() * 1.1)));
                nextEXP += plusNextEXPList.Last();
                sumLevel++;
                expSliderMin = nextEXP - plusNextEXPList.Last();
                ChangeLevel();
                AddEXP(tempEXP);
            }
            //レベルアップまではいかない場合
            else
            {
                nowEXP += expValue;
                nowEXPText.text = nowEXP.ToString();
                expSlider.value = nowEXP;
            }
        }
        else if(expValue < 0)
        {
            int tempEXP;
            tempEXP = nowEXP + expValue - (nextEXP - plusNextEXPList.Last());
            //レベルが下がる場合
            if (tempEXP < 0)
            {
                nextEXP -= plusNextEXPList.Last();
                nowEXP = nextEXP;
                plusNextEXPList.RemoveAt(plusNextEXPList.Count - 1);
                sumLevel--;
                expSliderMin = nextEXP - plusNextEXPList.Last();
                ChangeLevel();
                AddEXP(tempEXP);
            }
            //レベルダウンまではいかない場合
            else
            {
                nowEXP += expValue;
                nowEXPText.text = nowEXP.ToString();
                expSlider.value = nowEXP;
            }
         }
    }

    void ChangeLevel()
    {
        sumLevelText.text = sumLevel.ToString();
        afterHPText.text = (trainingChara.HP + (sumLevel - nowLevel) * trainingChara.AddHP).ToString();
        afterSTRText.text = (trainingChara.STR + (sumLevel - nowLevel) * trainingChara.AddSTR).ToString();
        afterVITText.text = (trainingChara.VIT + (sumLevel - nowLevel) * trainingChara.AddVIT).ToString();
        SetEXPSlider(expSliderMin, nextEXP);
    }

    void SetEXPSlider(int minValue,int maxValue)
    {
        expSlider.minValue = minValue;
        expSlider.maxValue = maxValue;
        if (sumLevel >= trainingChara.MaxLevel)
        {
            expSlider.value = maxValue;
            nowEXPText.text = "Max";
            nextEXPText.text = "Level";
        }
        else
        {
            expSlider.value = nowEXP;
            nowEXPText.text = nowEXP.ToString();
            nextEXPText.text = maxValue.ToString();
        }
    }
    //経験値アイテムの使用
    public void ApplyButtonClicked()
    {
        if (sumCoin <= 0) return;

        //アカウントの経験値アイテム情報を更新
        foreach (EXPItem_Info item in itemManager.GetEXPItemList())
        {
            if (itemCounter.EXPItemNum[item.Name] != 0)
            {
                itemManager.PlusEXPItemNum(item.Name, -itemCounter.EXPItemNum[item.Name]);
            }
        }
        #region セーブ関連
        //アカウントのコイン情報のセーブ
        accountManager.PlusCoin(-sumCoin);
        //アカウント情報のセーブ
        accountManager.SaveAccountData();
        //キャラ情報のセーブ
        trainingChara.Level = sumLevel;
        trainingChara.NowEXP = nowEXP;
        trainingChara.NextEXP = nextEXP;
        trainingChara.HP += (sumLevel - nowLevel) * trainingChara.AddHP;
        trainingChara.STR += (sumLevel - nowLevel) * trainingChara.AddSTR;
        trainingChara.VIT += (sumLevel - nowLevel) * trainingChara.AddVIT;
        this.gameObject.GetComponent<TrainingCharaManager>().SaveCharaInfo(trainingChara);
        #endregion

        //ちょっとした画面上の演出（レベルが上がったときのみ）
        if (trainingChara.Level > nowLevel)
        {
            levelUpObj.SetActive(true);
            audioManager.LevelUp();
        }
        else
        {
            audioManager.Training();
        }
        Invoke("HideApplyEffect", 1f);

        expSliderMin = (int)expSlider.minValue;
        //画面上のデータ更新
        SetPanel(trainingChara);
    }
    void HideApplyEffect()
    {
        levelUpObj.SetActive(false);
    }

    public void ItemResetButtonClicked()
    {
        expSliderMin = 0;
        SetPanel(trainingChara);
    }

    #region レベルリセット関連
    public void LevelResetButtonClicked()
    {
        levelResetPanel.SetActive(true);
    }
    public void LevelReset()
    {
        //キャラ情報のリセット
        trainingChara.Level = 1;
        trainingChara.NowEXP = 0;
        trainingChara.NextEXP = trainingChara.Level1NextEXP;
        trainingChara.HP = trainingChara.Level1HP;
        trainingChara.STR = trainingChara.Level1STR;
        trainingChara.VIT = trainingChara.Level1VIT;
        this.gameObject.GetComponent<TrainingCharaManager>().SaveCharaInfo(trainingChara);

        LevelResetCancel();
    }
    public void LevelResetCancel()
    {
        expSliderMin = 0;
        SetPanel(trainingChara);
        levelResetPanel.SetActive(false);
    }
    #endregion

    public void BackButtonClicked()
    {
        expSliderMin = 0;
        trainingApplyPanel.SetActive(false);
        audioManager.Button1();
    }
}
