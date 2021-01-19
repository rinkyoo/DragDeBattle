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
        PlusExp(0);
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

    public int GetLevel()
    {
        return accountData.level;
    }
    public int GetExp()
    {
        return accountData.exp;
    }
    public int GetNextExp()
    {
        return accountData.nextExp;
    }
    public void PlusExp(int plusExp)
    {
        accountData.exp += plusExp;
        while(accountData.exp >= accountData.nextExp)
        {
            if(accountData.level < AccountDefine.maxLevel)
                accountData.level++;
            accountData.plusNextExp = (int)(Math.Ceiling(accountData.plusNextExp * AccountDefine.rateNextExp));
            accountData.nextExp += accountData.plusNextExp;
        }
        if (levelText != null)
        {
            levelText.text = accountData.level.ToString();
            levelGageImage.fillAmount = GetExpGageValue();
        }
    }
    public float GetExpGageValue()
    {
        float temp = accountData.exp - (accountData.nextExp - accountData.plusNextExp);
        return temp / accountData.plusNextExp;
    }
    public void ResetLevelClicked()
    {
        accountData.level = 1;
        accountData.exp = 0;
        accountData.nextExp = AccountDefine.firstNextExp;
        accountData.plusNextExp = AccountDefine.firstPlusNextExp;
        SaveAccountData();
        SceneManager.LoadScene("Home");
    }

    public void PlusCoin(int plusNum)
    {
        accountData.coin += plusNum;
        if (accountData.coin < 0) accountData.coin = 0;
        if (coinText != null)  coinText.text = accountData.coin.ToString();
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

    public void PlusExpItemNum(string itemName,int plusNum)
    {
        accountData.expItemData.ExpItemNum[itemName] += plusNum;
    }
    public ExpItemData GetExpItemData()
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
