using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using QuestCommon;

public class CharaManager : MonoBehaviour
{   
    //クエストで使用するキャラリスト
    private Chara_Info[] charas = new Chara_Info[Define.charaNum];
    //キャラ毎のCharaControllerを保持
    CharaController[] charaController = new CharaController[Define.charaNum];
    //生存中のキャラ数
    private int aliveNum = Define.charaNum;
    //フィールド上のPCの数
    private int inFieldNum = 0;

    bool toggleFlag = true;

    GameObject obj;
    GameObject instance;

    public void SetQuestChara(Chara_Info[] setCharas)
    {
        charas = setCharas;
        //Canvas上に編成キャラを表示
        GameObject pc_icon;
        for(int i=0;i<Define.charaNum;i++){
            pc_icon = GameObject.Find("QuestCanvas/PC_Panel/PC"+(i+1).ToString()+"_Panel/PC_Icon");
            pc_icon.GetComponent<Image>().sprite = charas[i].Icon;
        }
        //全キャラ分のプレハブを作成
        for(int i=0;i<Define.charaNum;i++)
        {
            instance = (GameObject)Instantiate(charas[i].Prefab,Define.initialPosi,Quaternion.Euler(0f, 180f, 0f));
            instance.name = "PC"+(i+1).ToString();

            #region キャラアイコンに適用するEventTriggernの設定
            CharaIconController cic = instance.GetComponent<CharaIconController>();
            EventTrigger trigger = GameObject.Find("#PC"+(i+1).ToString()).GetComponent<EventTrigger>();
            //PointerClickの設定
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((eventData) => {cic.PointerClick(); });
            trigger.triggers.Add(entry);
            //BeginDragの設定
            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.BeginDrag;
            entry.callback.AddListener((eventDate) => { cic.BeginDrag(); });
            trigger.triggers.Add(entry);
            //Dragの設定
            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.Drag;
            entry.callback.AddListener((eventDate) => { cic.Dragging(); });
            trigger.triggers.Add(entry);
            //EndDragの設定
            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.EndDrag;
            entry.callback.AddListener((eventDate) => { cic.EndDrag(); });
            trigger.triggers.Add(entry);
            #endregion

            //キャラステータスの初期設定
            charaController[i] = instance.GetComponent<CharaController>();
            charaController[i].SetStatus(charas[i]);
        }

    }
    
    //キャラが移動する座標を設定
    public void SetMovePosi(string name,Vector3 posi)
    {
        int i = int.Parse(name.Replace("PC",""))-1;
        charaController[i].SetMovePosi(posi);
    }
    //キャラがロックする敵を設定
    public void LockEnemy(string name,GameObject obj)
    {
        int i = int.Parse(name.Replace("PC",""))-1;
        charaController[i].LockEnemy(obj);
    }

    public void AllCharaAutoChange()
    {
        for(int i=0;i<Define.charaNum;i++)
        {
            charaController[i].toggle.isOn = toggleFlag;
        }
        toggleFlag = !toggleFlag;
    }

    //クエストクリアした際の処理
    public void QuestClear()
    {
        Vector3[] clearPosi = { new Vector3(-139f, 0f, 16f), new Vector3(-132.5f, 0f, 7f), new Vector3(-126f, 0f, 16f), new Vector3(-119.5f, 0f, 7f), new Vector3(-113f, 0f, 16f), new Vector3(-106.5f, 0f, 7f), new Vector3(-100f, 0f, 16f), };
        for (int i = 0; i < Define.charaNum; i++)
        {
            charaController[i].gameObject.SetActive(true);
            charaController[i].gameObject.transform.position = clearPosi[i];
            charaController[i].QuestClear();
        }
    }

    public void SetPCInField()
    {
        inFieldNum++;
    }
    public void Desporn()
    {
        aliveNum--;
        inFieldNum--;
    }
    public void Return()
    {
        inFieldNum--;
    }
    public int GetAliveNum()
    {
        return aliveNum;
    }
    public int GetInFieldNum()
    {
        return inFieldNum;
    }

    #region PCオブジェクト・CharaController・CharaInfoの取得用関数
    public GameObject GetPC(int i)
    {
        if(charaController[i] == null) return null;
        if(charaController[i].IsInField())
        {
            return charaController[i].gameObject;
        }
        else{
            return null;
        }
    }
    public GameObject GetPC(string pcName)
    {
        int i = int.Parse(pcName.Replace("PC",""))-1;
        if (charaController[i] == null) return null;
        if (charaController[i].IsInField())
        {
            return charaController[i].gameObject;
        }
        else
        {
            return null;
        }
    }
    public CharaController GetCharaController(int i)
    {
        return charaController[i];
    }
    public CharaController GetCharaController(string pcName)
    {
        int i = int.Parse(pcName.Replace("PC", "")) - 1;
        return charaController[i];
    }

    public Chara_Info GetCharaInfo(string name)
    {
        int i = int.Parse(name.Replace("PC",""))-1;
        return charas[i];
    }
    #endregion

}
