using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using TMPro;

public class QuestClearManager : MonoBehaviour
{
    private AudioManager audioManager;

    #region Inspectorで取得するGameObject関連
    [SerializeField] GameObject questFinCamera;
    [SerializeField] GameObject questFinUICamera;
    [SerializeField] GameObject questFinCanvas;
    [SerializeField] GameObject clearText;
    [SerializeField] GameObject goHomeButton;
    [SerializeField] GameObject sceneLoadPanel;
    private CanvasGroup sceneLoadCanvasGroup;
    //クエスト結果用
    [SerializeField] GameObject resultObj;
    [SerializeField] GameObject resultPanel;
    [SerializeField] GameObject titleObj;
    [SerializeField] GameObject timeObj;
    [SerializeField] TextMeshProUGUI timeText;
    [SerializeField] GameObject coinObj;
    [SerializeField] TextMeshProUGUI coinText;
    [SerializeField] GameObject itemScrollView;
    [SerializeField] GameObject dropItemContent;
    [SerializeField] Image dropItemImage;
    #endregion

    void Start()
    {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        sceneLoadCanvasGroup = sceneLoadPanel.GetComponent<CanvasGroup>();
    }

    public void QuestClear(string clearTime,int totalCoin,List<EXPItem_Info> expItemList)
    {
        audioManager.QuestClearBGM();

        timeText.text = clearTime;

        coinText.text = totalCoin.ToString();
        foreach(EXPItem_Info item in expItemList)
        {
            Image itemImage = Instantiate(dropItemImage, dropItemContent.transform.position, Quaternion.identity) as Image;
            itemImage.gameObject.transform.SetParent(dropItemContent.transform);
            itemImage.transform.localScale = new Vector3(1f, 1f, 1f);
            itemImage.sprite = item.Icon;
        }

        questFinCanvas.SetActive(true);
        questFinCamera.SetActive(true);
        questFinUICamera.SetActive(true);
        questFinCamera.GetComponent<Animator>().SetTrigger("QuestFin");
        clearText.SetActive(true);
        Invoke("AppearResult", 5.5f);
    }

    void AppearResult()
    {
        clearText.SetActive(false);

        Sequence seq = DOTween.Sequence();
        seq.Append(resultObj.transform.DOLocalMoveY(0, 1))
            .Append(resultPanel.GetComponent<Image>().DOFillAmount(1, 1f))
            .InsertCallback(2f, () =>
             {
                 audioManager.System24();
                 titleObj.SetActive(true);
             })
            .InsertCallback(2.5f, () =>
             {
                 audioManager.System24();
                 timeObj.SetActive(true);
             })
            .InsertCallback(3f, () =>
             {
                 audioManager.System24();
                 coinObj.SetActive(true);
             })
            .InsertCallback(3.5f,()=>
            {
                audioManager.System24();
                itemScrollView.SetActive(true);
            })
            .InsertCallback(4f, () =>
            {
                audioManager.System24();
                goHomeButton.SetActive(true);
            });
        seq.Play();
    }

    public void GoHomeButtonClicked()
    {
        Sequence seq = DOTween.Sequence();
        seq.AppendCallback(() =>
        {
            sceneLoadPanel.SetActive(true);
            sceneLoadCanvasGroup.alpha = 0f;
        })
        .Append(sceneLoadCanvasGroup.DOFade(1f, 0.5f))
        .AppendCallback(() =>
        {
            SceneManager.LoadScene("Home");
        });
        seq.Play();
    }
}
