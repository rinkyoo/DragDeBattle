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

    [SerializeField] private List<ExpItem_Info> expItemList = new List<ExpItem_Info>(); //EXPItemの情報リスト
    private ExpItemData expItemData; //保持しているExpItemの数

    void Awake()
    {
        accountManager = GameObject.Find("AccountManager").GetComponent<AccountManager>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }

    void Start()
    {
        expItemData = accountManager.GetExpItemData();
    }

    public void PlusExpItemNum(string itemName, int plusNum)
    {
        accountManager.PlusExpItemNum(itemName, plusNum);
    }
    
    public List<ExpItem_Info> GetExpItemList()
    {
        return expItemList;
    }
    
    //EXPアイテムの所持個数を取得
    public int GetExpItemNum(string itemName)
    {
        return expItemData.ExpItemNum[itemName];
    }

}
