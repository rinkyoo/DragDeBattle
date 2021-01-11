using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//実装済みのキャラを全て保持するデータベース
[CreateAssetMenu(fileName = "Chara_Info_Database", menuName = "Create_Chara_Info_Database")]
public class Chara_Info_Database : ScriptableObject
{
    [SerializeField]
    private List<Chara_Info> CharaList = new List<Chara_Info>();
    /*
    //IDでキャラを返す
    public Chara_Info Get_Chara(int findID)
    {
        return CharaList.Find(Chara => Chara.ID == findID);
    }
    */
    public List<Chara_Info> GetCharaList()
    {
        return CharaList;
    }

}
