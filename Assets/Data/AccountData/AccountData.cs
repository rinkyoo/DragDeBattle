using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AccountData
{
    //プレイヤーレベル
    public int level;
    //合計経験値
    public int exp;
    //レベルアップに必要な経験値
    public int nextExp;
    //nextExpに加算する経験値量
    public int plusNextExp;

    //所持コイン
    public int coin;

    //クエストのクリア状況データ
    public ClearedQuestData clearedQuest = new ClearedQuestData();

    //経験値アイテムの所持データ
    public ExpItemData expItemData = new ExpItemData();
}
