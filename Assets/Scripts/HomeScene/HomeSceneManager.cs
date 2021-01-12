using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HomeSceneManager : MonoBehaviour
{
    private AudioManager audioManager;

    List<Chara_Info> charaList; //所持キャラのリスト
    private GameObject homePC;
    string pcName = "";

    [SerializeField] GameObject sceneLoadPanel;
    private CanvasGroup sceneLoadCanvasGroup;
    [SerializeField] Button homePCChangeButton;

    #region 固定値の設定
    //SkyBoxの回転スピード
    private float rotateSpeed = 0.8f;
    //SkyBoxのマテリアル
    private Material skyboxMaterial;

    private Vector3 homePCScale = new Vector3(50f, 50f, 50f);
    private Vector3 homePCPosi = new Vector3(-1480f, 700f, -650f);
    private Vector3 homePCRotate = new Vector3(0f, 180f, 0f);
    private Vector3 newHomePCPosi = new Vector3(-1350f, 700f, -650f);
    private Vector3 leaveHomePCPosi = new Vector3(-1525f, 700f, -775f);
    private Vector3 leaveHomePCRotate = new Vector3(0f, 200f, 0f);
    #endregion

    void Awake()
    {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        sceneLoadCanvasGroup = sceneLoadPanel.GetComponent<CanvasGroup>();
    }
    void Start()
    {
        audioManager.HomeBGM();

        charaList = GameObject.Find("CharaInfoManager").GetComponent<CharaInfoManager>().GetCharaList();
        Chara_Info chara = charaList[Random.Range(0, charaList.Count)];
        pcName = chara.Name;
        homePC = Instantiate(chara.Prefab, homePCPosi, Quaternion.Euler(0f, 180f, 0f));
        homePC.transform.localScale = homePCScale;
        homePC.SetActive(false);

        skyboxMaterial = RenderSettings.skybox;

        Sequence seq = DOTween.Sequence();
        seq.AppendCallback(() =>
            {
                sceneLoadPanel.SetActive(true);
                sceneLoadCanvasGroup.alpha = 1f;
                homePC.SetActive(true);
            })
            .Append(sceneLoadCanvasGroup.DOFade(0f, 0.5f))
            .AppendCallback(() =>
            {
                sceneLoadPanel.SetActive(false);
            });
        seq.Play();
    }

    void Update()
    {
        skyboxMaterial.SetFloat("_Rotation", Mathf.Repeat(skyboxMaterial.GetFloat("_Rotation") + rotateSpeed * Time.deltaTime, 360f));
    }

    public void ChangeHomePC()
    {
        homePCChangeButton.enabled = false;
        Sequence Seq = DOTween.Sequence();
        GameObject newPCPrefab = homePC;
        string nowPCName = pcName;
        while (nowPCName == pcName)
        {
            Chara_Info chara = charaList[Random.Range(0, charaList.Count)];
            pcName = chara.Name;
            newPCPrefab = chara.Prefab;
        }
        GameObject newHomePC = Instantiate(newPCPrefab, newHomePCPosi, Quaternion.Euler(0f, -90f, 0f));
        newHomePC.transform.localScale = homePCScale;
        newHomePC.SetActive(false);

        Seq.Append(homePC.transform.DORotate(leaveHomePCRotate, 0.5f))
            .InsertCallback(0f, () => { newHomePC.SetActive(true); })
            .Append(homePC.transform.DOMove(leaveHomePCPosi, 3f))
            .Join(newHomePC.transform.DOMove(homePCPosi, 3f))
            .InsertCallback(0.4f, () =>
             {
                 homePC.GetComponent<HomePCController>().SetWalkTrigger(true);
                 newHomePC.GetComponent<HomePCController>().SetWalkTrigger(true);
             })
            .AppendCallback(() =>
            {
                Destroy(homePC);
                homePC = newHomePC;
                homePC.GetComponent<HomePCController>().SetWalkTrigger(false);
            })
            .Append(newHomePC.transform.DORotate(homePCRotate, 1f))
            .AppendCallback(() =>
            {
                homePCChangeButton.enabled = true;
            });
        Seq.Play();

    }
}
