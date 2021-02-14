using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Diagnostics;
using QuestCommon;

public class CharaInfoManager : MonoBehaviour
{
    
    private DataHolder dataHolder;
    
    [SerializeField, Multiline(5)] private string[] str;

    //実装済み全キャラのリスト
    [SerializeField] private List<Chara_Info> AllChara = new List<Chara_Info>();
    //所持しているキャラのリスト
    private List<Chara_Info> charaList = new List<Chara_Info>();
    //編成キャラ情報
    private Chara_Info[][] formationChara = new Chara_Info[Define.ptNum][];
    //サポートキャラのリスト
    [SerializeField] private List<Chara_Info> supportList = new List<Chara_Info>();

    void Awake()
    {
        dataHolder = GameObject.Find("DataHolder").GetComponent<DataHolder>();
        //編成キャラ用・ジャグ配列の初期化
        for(int i=0;i<Define.ptNum;i++)
        {
            formationChara[i] = new Chara_Info[Define.charaNum];
        }

        //全所持キャラ情報のロード
        string path = Application.persistentDataPath + "/CharaData/MyChara/";

        if (!System.IO.Directory.Exists(path))
            System.IO.Directory.CreateDirectory(path);

        string[] files = Directory.GetFiles(path, "*.json", SearchOption.AllDirectories);
        //データがある場合
        if (files.Length > 0)
        {
            for (int i = 0; i < files.Length; i++)
            {
                charaList.Add(LoadMyChara(files[i]));
            }
        }
        //データがない場合（新しくキャラデータを作成します）
        else
        {
            foreach (Chara_Info chara in AllChara)
            {
                #region レベル１でのステを設定
                chara.Level = 1;
                chara.NowEXP = 0;
                chara.NextEXP = chara.Level1NextEXP;
                chara.PlusNextEXP = chara.Level1PlusNextEXP;
                chara.HP = chara.Level1HP;
                chara.STR = chara.Level1STR;
                chara.VIT = chara.Level1VIT;
                #endregion
                SaveCharaInfo(chara);
                charaList.Add(chara);
            }
        }
        //編成情報のロード
        if (!System.IO.Directory.Exists(Application.persistentDataPath + "/FormationData"))
            System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/FormationData");
        for (int i=0;i<Define.ptNum;i++)
        {
            FormationInfo temp = LoadFormationInfo(i+1);
            for(int j=0;j<Define.charaNum-1;j++)
            {
                formationChara[i][j] = charaList.Find(chara => chara.Name == temp.charaName[j]);
            }
        }
    }
    //キャラ情報の保存
    public void SaveCharaInfo(Chara_Info setCharaInfo)
    {
        MyChara setMyChara = new MyChara();
        setMyChara.Name = setCharaInfo.Name;
        setMyChara.Level = setCharaInfo.Level;
        setMyChara.NowEXP = setCharaInfo.NowEXP;
        setMyChara.NextEXP = setCharaInfo.NextEXP;
        setMyChara.PlusNextEXP = setCharaInfo.PlusNextEXP;
        setMyChara.HP = setCharaInfo.HP;
        setMyChara.SP = setCharaInfo.SP;
        setMyChara.STR = setCharaInfo.STR;
        setMyChara.VIT = setCharaInfo.VIT;
        setMyChara.AGI = setCharaInfo.AGI;
        setMyChara.SpeedATK = setCharaInfo.SpeedATK;
        setMyChara.RangeATK = setCharaInfo.RangeATK;
        StreamWriter writer;
        string jsonstr = JsonUtility.ToJson(setMyChara);
        string name = setMyChara.Name;
        writer = new StreamWriter(Application.persistentDataPath + "/CharaData/MyChara/MyChara" + name + ".json", false);
        writer.Write(jsonstr);
        writer.Flush();
        writer.Close();
    }

    Chara_Info SetCharaInfoStatus(Chara_Info setCharaInfo, MyChara setMyChara)
    {
        setCharaInfo.Level = setMyChara.Level;
        setCharaInfo.NowEXP = setMyChara.NowEXP;
        setCharaInfo.NextEXP = setMyChara.NextEXP;
        setCharaInfo.PlusNextEXP = setMyChara.PlusNextEXP;
        setCharaInfo.HP = setMyChara.HP;
        setCharaInfo.STR = setMyChara.STR;
        setCharaInfo.VIT = setMyChara.VIT;
        setCharaInfo.AGI = setMyChara.AGI;
        setCharaInfo.SpeedATK = setMyChara.SpeedATK;
        setCharaInfo.RangeATK = setMyChara.RangeATK;

        return setCharaInfo;
    }

    //MyCharaをロードし、ステータスを設定（Pathで）
    Chara_Info LoadMyChara(string path)
    {
        string datastr = "";
        StreamReader reader;
        reader = new StreamReader(path);
        datastr = reader.ReadToEnd();
        reader.Close();

        MyChara setMyChara = JsonUtility.FromJson<MyChara>(datastr);
        Chara_Info setCharaInfo = AllChara.Find(chara => chara.Name == setMyChara.Name);
        return SetCharaInfoStatus(setCharaInfo.Clone(), setMyChara);
    }
    //編成情報の保存
    public void SaveFormationInfo(Chara_Info[] charas,int i)
    {
        formationChara[i-1] = charas;
        //編成中のCharaInfoから、nameのみを抽出
        FormationInfo newFormation = new FormationInfo();
        for(int j=0;j<Define.charaNum-1;j++)
        {
            newFormation.charaName[j] = charas[j].Name;
        }
        
        StreamWriter writer;
        string jsonstr = JsonUtility.ToJson (newFormation);
        writer = new StreamWriter(Application.persistentDataPath + "/FormationData/Formation"+i.ToString()+".json", false);
        writer.Write (jsonstr);
        writer.Flush ();
        writer.Close ();
        
    }
    //編成情報のロード（編成番号で）
    FormationInfo LoadFormationInfo(int i)
    {
        //編成情報がない場合は、手持ちの先頭から適当に編成する
        if (!System.IO.File.Exists(Application.persistentDataPath + "/FormationData/Formation" + i.ToString() + ".json"))
        {
            for (int j = 0; j < 6; j++)
            {
                formationChara[i - 1][j] = charaList[j];
            }
            SaveFormationInfo(formationChara[i - 1], i);
        }

        string datastr = "";
        StreamReader reader;
        reader = new StreamReader(Application.persistentDataPath + "/FormationData/Formation" + i.ToString() + ".json");
        datastr = reader.ReadToEnd();
        reader.Close();

       return JsonUtility.FromJson<FormationInfo>(datastr);
    }
    //***************************************************
    
    public void AddCharaInfo(Chara_Info setChara)
    {
        SaveCharaInfo(setChara);
        charaList.Add(setChara);
    }
    
    public void SetSupportChara(Chara_Info setChara,int i)
    {
        formationChara[i][6] = setChara;
    }
    
    public List<Chara_Info> GetCharaList()
    {
        return charaList;
    }
    public List<Chara_Info> GetSupportList()
    {
        return supportList;
    }
    public Chara_Info[][] GetFormationChara()
    {
        return formationChara;
    }
    
    public void SetQuestChara(int i)
    {
        dataHolder.SetFormationChara(formationChara[i]);
    }

}
