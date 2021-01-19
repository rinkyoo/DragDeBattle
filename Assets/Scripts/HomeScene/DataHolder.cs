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
    private Quest_Enemy questEnemy;
    private int[] playQuest = new int[2];
    
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
    
    public void SetQuestEnemy(Quest_Enemy qe)
    {
        questEnemy = qe;
    }
    public Quest_Enemy GetQuestEnemy()
    {
        return questEnemy;
    }
    
    public int[] GetClearedQuest()
    {
        return accountManager.GetClearedQuest();
    }

    public void SetPlayQuest(int level,int questNum)
    {
        playQuest = new int[2] { level, questNum };
    }

    public void SaveClearData()
    {
        accountManager.SaveClearedData(playQuest);
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
