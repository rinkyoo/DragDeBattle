using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[System.Serializable]
public class EXPItemData : ISerializationCallbackReceiver
{
    public Dictionary<string, int> EXPItemNum = new Dictionary<string, int>()
    {
        {"SmallEXP",0 },
        {"MediumEXP",0 },
        {"BigEXP",0 }
    };

    //DictionaryをJsonUtilityで使用するためのList
    public List<string> dicKey = new List<string>();
    public List<int> dicValue = new List<int>();

    //シリアライズ前
    public void OnBeforeSerialize()
    {
        //Dictionary（EXPItemData）をListに保存
        dicKey.Clear();
        dicValue.Clear();
        foreach(var dic in EXPItemNum)
        {
            dicKey.Add(dic.Key);
            dicValue.Add(dic.Value);
        }
    }

    //デシリアライズ後
    public void OnAfterDeserialize()
    {
        //List->Dictionaryに変換
        EXPItemNum = new Dictionary<string, int>();
        for(int i=0;i<Math.Min(dicKey.Count,dicValue.Count);i++)
        {
            EXPItemNum.Add(dicKey[i], dicValue[i]);
        }
    }
}