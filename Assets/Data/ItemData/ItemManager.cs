using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Diagnostics;

public class ItemManager : MonoBehaviour
{
    private AccountManager accountManager;
    private AudioManager audioManager;

    [SerializeField] private List<EXPItem_Info> expItemList = new List<EXPItem_Info>(); //EXPItemの情報リスト
    private EXPItemData expItemData; //保持しているEXPItemの数

    void Awake()
    {
        accountManager = GameObject.Find("AccountManager").GetComponent<AccountManager>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }

    void Start()
    {
        expItemData = accountManager.GetExpItemData();
    }

    public void PlusEXPItemNum(string itemName, int plusNum)
    {
        accountManager.PlusExpItemNum(itemName, plusNum);
    }
    
    public List< EXPItem_Info> GetEXPItemList()
    {
        return expItemList;
    }
    
    //EXPアイテムの所持個数を取得
    public int GetEXPItemNum(string itemName)
    {
        return expItemData.EXPItemNum[itemName];
    }

}
