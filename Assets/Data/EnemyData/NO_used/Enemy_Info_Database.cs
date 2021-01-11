using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName="Enemy_Info_Database",menuName="Create_Enemy_Info_Database")]
public class Enemy_Info_Database : ScriptableObject
{
    [SerializeField]
    private List<Enemy_Info> enemyList = new List<Enemy_Info>();
    
    //指定した要素番号のキャラを返す
    public Enemy_Info Get_Enemy(int i)
    {
        return enemyList[i];
    }
    //名前でキャラを返す
    public Enemy_Info Get_Enemy(string name)
    {
        return enemyList.Find(enemy => enemy.chara_name == name);
    }
    
    public void Set_Enemy(Enemy_Info enemy,int i)
    {
        enemyList[i] = enemy;
    }
    
}
