using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//シーン間で共有するデータを保持
public class DataHolder : MonoBehaviour
{
    private AccountManager accountManager;

    private static bool created = false;
    private Chara_Info[] formationChara = new Chara_Info[7];
    private Quest_Data questData;
    private int[] playQuest = new int[2];
    private string playQuestType = "";
    
    void Awake()
    {
        if(!created){
            DontDestroyOnLoad(this);
            created = true;
        }else{
            Destroy(this.gameObject);
        }
    }

    public void SetAccountManager()
    {
        accountManager = GameObject.Find("AccountManager").GetComponent<AccountManager>();
    }

    public void SetFormationChara(Chara_Info[] charas)
    { 
        formationChara = charas;
    }
    public Chara_Info[] GetFormationChara()
    { 
        return formationChara;
    }
    
    public void SetQuestData(Quest_Data quest)
    {
        questData = quest;
    }
    public Quest_Data GetQuestData()
    {
        return questData;
    }
    
    public int[] GetClearedQuest(string questType)
    {
        return accountManager.GetClearedQuest(questType);
    }

    public void SetPlayQuest(string questType,int level,int questNum)
    {
        playQuestType = questType;
        playQuest = new int[2] { level, questNum };
    }

    public void SaveClearData()
    {
        accountManager.SaveClearedData(playQuestType,playQuest);
    }

    public int GetLevel()
    {
        return accountManager.GetLevel();
    }
    public int GetExp()
    {
        return accountManager.GetExp();
    }
    public int GetNextExp()
    {
        return accountManager.GetNextExp();
    }
    public float GetExpGageValue()
    {
        return accountManager.GetExpGageValue();
    }
    public void PlusExp(int exp)
    {
        accountManager.PlusExp(exp);
    }
    public void PlusCoin(int coin)
    {
        accountManager.PlusCoin(coin);
    }
    public void PlusExpItem(List<ExpItem_Info> expItemList)
    {
        Dictionary<string, int> expItemDic = new Dictionary<string, int>();
        foreach(ExpItem_Info item in expItemList)
        {
            if(expItemDic.ContainsKey(item.Name))
                expItemDic[item.Name]++;
            else
                expItemDic.Add(item.Name,1);
        }
        foreach (string name in expItemDic.Keys)
        {
            accountManager.PlusExpItemNum(name, expItemDic[name]);
        }
    }

    public void SaveAccountData()
    {
        accountManager.SaveAccountData();
    }
}
