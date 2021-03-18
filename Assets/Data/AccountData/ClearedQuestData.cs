using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[System.Serializable]
public class ClearedQuestData : ISerializationCallbackReceiver
{
    //クエストグループ毎にクエストのクリア状況を保持
    public Dictionary<string, int[]> clearedQuestDic = new Dictionary<string, int[]>()
    {
        {"normal",new int[2]{0,0 } },
        {"exp",new int[2]{0,0 } },
        {"coin",new int[2]{0,0 } },
        {"expItem",new int[2]{0,0 } }
    };

    //DictionaryをJsonUtilityで使用するためのList
    public List<string> dicKey = new List<string>();
    public List<int> dicValue0 = new List<int>();
    public List<int> dicValue1 = new List<int>();

    //シリアライズ前
    public void OnBeforeSerialize()
    {
        //DictionaryをListに保存
        dicKey.Clear();
        dicValue0.Clear();
        dicValue1.Clear();
        foreach(var dic in clearedQuestDic)
        {
            dicKey.Add(dic.Key);
            dicValue0.Add(dic.Value[0]);
            dicValue1.Add(dic.Value[1]);
        }
    }

    //デシリアライズ後
    public void OnAfterDeserialize()
    {
        //List->Dictionaryに変換
        clearedQuestDic = new Dictionary<string, int[]>();
        for(int i=0;i<Math.Min(dicKey.Count,dicValue0.Count);i++)
        {
            int[] temp = new int[2] { dicValue0[i], dicValue1[i] };
            clearedQuestDic.Add(dicKey[i], temp);
        }
    }

    public void SetClearedQuest(string questType,int[] questNum)
    {
        clearedQuestDic[questType] = questNum;
    }
    public int[] GetClearedQuest(string questType)
    {
        return clearedQuestDic[questType];
    }
}