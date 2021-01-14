using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using System.Diagnostics;
using TMPro;
using AccountCommonData;

public class AccountManager : MonoBehaviour
{
    [SerializeField, Multiline(5)] private string[] str;
    string path = "";

    private AccountData accountData;
    private AudioManager audioManager;

    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] Image levelGageImage;
    [SerializeField] TextMeshProUGUI coinText;

    void Awake()
    {
        //アカウントデータのPathを設定
        path = Application.dataPath + "/Data/AccountData/AccountData.json";
        //アカウント情報をロード
        string datastr = "";
        StreamReader reader;
        reader = new StreamReader(path);
        datastr = reader.ReadToEnd();
        reader.Close();

        accountData =  JsonUtility.FromJson<AccountData>(datastr);

        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }


    void Start()
    {
        //起動時のアカウントパネルの値を設定
        PlusEXP(0);
        coinText.text = accountData.coin.ToString();
    }
    public void SaveAccountData()
    {
        StreamWriter writer;
        string jsonstr = JsonUtility.ToJson(accountData);
        writer = new StreamWriter(path, false);
        writer.Write(jsonstr);
        writer.Flush();
        writer.Close();
    }

    public void PlusEXP(int plusEXP)
    {
        accountData.exp += plusEXP;
        while(accountData.exp >= accountData.nextEXP)
        {
            if(accountData.level < AccountDefine.maxLevel)
                accountData.level++;
            accountData.plusNextEXP = (int)(Math.Ceiling(accountData.plusNextEXP * AccountDefine.rateNextEXP));
            accountData.nextEXP += accountData.plusNextEXP;
        }
        levelText.text = accountData.level.ToString();
        float temp = accountData.exp - (accountData.nextEXP - accountData.plusNextEXP);
        levelGageImage.fillAmount = temp / accountData.plusNextEXP;
    }

    public void PlusCoin(int plusNum)
    {
        accountData.coin += plusNum;
        if (accountData.coin < 0) accountData.coin = 0;
        coinText.text = accountData.coin.ToString();
    }
    public int GetCoin()
    {
        return accountData.coin;
    }

    public void SaveClearedData(int[] questData)
    {
        if(accountData.clearedQuest[0] < questData[0])
        {
            accountData.clearedQuest = questData;
            SaveAccountData();
        }
        else if (accountData.clearedQuest[0] == questData[0])
        {
            if(accountData.clearedQuest[1] <= questData[1])
            {
                accountData.clearedQuest = questData;
                SaveAccountData();
            }
        }
    }

    public void PlusEXPItemNum(string itemName,int plusNum)
    {
        accountData.expItemData.EXPItemNum[itemName] += plusNum;
    }
    public EXPItemData GetEXPItemData()
    {
        return accountData.expItemData;
    }

    public int[] GetClearedQuest()
    {
        return accountData.clearedQuest;
    }

    public void AllQuestClearClicked()
    {
        audioManager.Button1();
        SaveClearedData(new int[2] { 100, 100 });
        SceneManager.LoadScene("Home");
    }
    public void ResetClearedQuestClicked()
    {
        audioManager.Button1();
        accountData.clearedQuest = new int[2] { 0, 0 };
        SaveAccountData();
        SceneManager.LoadScene("Home");
    }
}
