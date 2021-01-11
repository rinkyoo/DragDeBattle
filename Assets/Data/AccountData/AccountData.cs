using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AccountData
{
    //所持コイン
    public int coin;

    //[0] : クリア済みクエストのLevel ・　[1] : クリア済クエストのクエスト番号
    public int[] clearedQuest = new int[2];

    //経験値アイテムの所持データ
    public EXPItemData expItemData = new EXPItemData();


}
