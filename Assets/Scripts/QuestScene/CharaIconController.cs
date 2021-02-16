using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using StateManager;
using DG.Tweening;
using QuestCommon;

public class CharaIconController : MonoBehaviour
{
    #region マウスとタップ両方に適応するため
    TouchManager touch_manager = new TouchManager();
    TouchManager touch_state;
    void touch()
    {
        touch_manager.update();
        touch_state = touch_manager.getTouch();
    }
    #endregion

    private QuestController questController;
    private CharaController charaController;
    private CharaEffect charaEffect;
    private IconTouchUI iconTouchUI; //キャラアイコンをタッチした時に使用
    private RaycastHit hit;
    
    GameObject dragPanel;
    Image waitGageImage;

    Vector3 canvasPosi = new Vector3();
    bool isDragging = false; //タッチとドラッグを区別するため
    bool ignoreDrag = false;

    int layerMask = 1 << 9;
    Vector3 initialPosi = new Vector3(-10f, 0f, 0f);
    float borderUpPosi; //UIとフィールドの境目のY座標（下側）
    float borderLeftPosi; //UIとフィールドの境目のX座標（左側）
    float borderRightPosi; //UIとフィールドの境目のX座標（右側）

    void Awake()
    {
        if (SceneManager.GetActiveScene().name == "Home")
        {
            Destroy(GetComponent<CharaIconController>());
            return;
        }
    }

    void Start()
    {
        questController = GameObject.Find("QuestController").GetComponent<QuestController>();
        charaEffect = GameObject.Find("CharaScript").GetComponent<CharaEffect>();
        iconTouchUI = GameObject.Find("CharaScript").GetComponent<IconTouchUI>();
        charaController = gameObject.GetComponent<CharaController>();

        GameObject canvas = GameObject.Find("QuestCanvas");
        CanvasInfo canvasInfo = canvas.GetComponent<CanvasInfo>();
        canvasPosi = canvasInfo.GetCanvasPosi();
        borderUpPosi = canvasInfo.GetBorderUpPosi();
        borderLeftPosi = canvasInfo.GetBorderLeftPosi();
        borderRightPosi = canvasInfo.GetBorderRightPosi();
        /*
        #region borderのスクリーン座標取得
        Camera canvasCamera = canvas.GetComponentInParent<Canvas>().worldCamera;
        var corners = new Vector3[4];
        GameObject.Find("border").GetComponent<RectTransform>().GetWorldCorners(corners);
        var temp = RectTransformUtility.WorldToScreenPoint(canvasCamera, corners[1]);
        borderPosi = temp.y;
        #endregion
        */
        dragPanel = canvas.transform.Find("PCDragPanel/#" + transform.name).gameObject;
        waitGageImage = dragPanel.transform.Find("WaitGageImage").gameObject.GetComponent<Image>();
    }

    #region アイコンに適用するEvent Trigger関連
    public void PointerClick()
    {
        if(isDragging || iconTouchUI.skillTextFlag || charaController.IsStateNone())
            return; //ドラッグ中 or スキル演出中 or PC召喚中はクリック無効
        questController.PauseBattle();
        iconTouchUI.SetCharaInfoUI(transform.name);
    }
    public void BeginDrag()
    {
        //同時召喚可能数を超える場合はドラッグ無視
        if (questController.GetFieldPCNum() >= Define.maxFieldPCNum || charaController.IsStateNone())
        {
            ignoreDrag = true;
            return;
        }
        //timeScale==0でも、PCInfoPanelがアクティブだったらドラッグ開始（操作性向上のため）
        else if(charaController.IsStateIcon() && iconTouchUI.PCInfoPanel.activeSelf)
        {
            iconTouchUI.PCInfoPanel.SetActive(false);
        }
        //PCのState!=Icon、または、timeScale==0の場合もドラッグ無視
        else if (!charaController.IsStateIcon() || Time.timeScale == 0)
        {
            ignoreDrag = true;
            return;
        }

        ignoreDrag = false;
        isDragging = true;
        questController.PauseBattle();
    }
    public void Dragging()
    {
        if(ignoreDrag)return;
        touch();
        Vector3 posi = touch_manager.touch_position;
        posi.z = 0f;
        Vector3 worldPosi = Camera.main.ScreenToWorldPoint(posi);
        Ray ray = new Ray(worldPosi, Camera.main.transform.forward);
        
        foreach (RaycastHit hit in Physics.RaycastAll(ray, layerMask))
        {
            //フィールド上でドラッグ中はPCを表示
            if (hit.transform.name == "Field" && posi.y > borderUpPosi)
            {
                transform.position = hit.point;
                return;
            }
        }
        transform.position = initialPosi;
    }
    public void EndDrag()
    {
        if(ignoreDrag)return;
        isDragging = false;
        touch();
        Vector3 posi = touch_manager.touch_position;
        posi.z = 0f;
        Vector3 worldPosi = Camera.main.ScreenToWorldPoint(posi);
        Ray ray = new Ray(worldPosi,Camera.main.transform.forward);
        foreach (RaycastHit hit in Physics.RaycastAll(ray,layerMask))
        {
            //フィールド上でドラッグ終了した場合はPC召喚
            if (hit.transform.name == "Field" && posi.y > borderUpPosi && posi.x > borderLeftPosi && posi.x < borderRightPosi)
            {
                Sequence seq = DOTween.Sequence();
                seq.AppendCallback(()=>
                {
                    charaController.SetStateNone();
                    gameObject.SetActive(false);
                    transform.position = hit.point;
                    charaEffect.SetPCAppearParticle(hit.point);
                })
                .AppendInterval(0.4f)
                .AppendCallback(()=>
                {
                    SetPCInField();
                });
                seq.Play();
                questController.ResumeBattle();
                return;
            }
        }

        transform.position = initialPosi;
        questController.ResumeBattle();
    }
    #endregion

    //通常のアイコンの色に設定（召喚可能状態）
    public void NormalIcon()
    {
        dragPanel.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
    }

    //フィールド上にキャラを召喚
    void SetPCInField()
    {
        gameObject.SetActive(true);
        charaController.animator.enabled = true;
        dragPanel.GetComponent<Image>().color = new Color(1f, 0, 0, 0.6f);
        questController.SetPCInField();
    }

    //死亡したらキャラのアイコンの色を変更
    public void Died()
    {
        dragPanel.GetComponent<Image>().color = new Color(0,0,0,0.7f);
    }

    public void SetWaitGageImage()
    {
        dragPanel.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
        waitGageImage.fillAmount = Define.waitTime;
        waitGageImage.gameObject.SetActive(true);
    }
    public void UpdateWaitGageImage(float setTime)
    {
        int i = Mathf.CeilToInt(setTime);
        waitGageImage.fillAmount = i / Define.waitTime;
    }
    public void HideWaitGageImage()
    {
        waitGageImage.gameObject.SetActive(false);
    }

}
