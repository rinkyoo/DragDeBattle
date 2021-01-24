using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using StateManager;

public class HomePCController : MonoBehaviour
{
    #region マウスクリックと画面タップ両方に対応するため
    TouchManager touch_manager = new TouchManager();
    TouchManager touch_state;
    void touch()
    {
        touch_manager.update();
        touch_state = touch_manager.getTouch();
    }
    #endregion

    private Animator animator;
    private QuerySDEmotionalController emotionalController;

    private RaycastHit hit;

    private List<string> animationList = new List<string>() {"Waiwai","Clione","Tukkomi"};

    //public void SetHomePC()
    void OnEnable()
    {
        animator = this.gameObject.GetComponent<Animator>();
        emotionalController = this.gameObject.GetComponent<QuerySDEmotionalController>();

        if (GetComponent<EventTrigger>() == null)
        {
            gameObject.AddComponent<EventTrigger>();
            EventTrigger trigger = GetComponent<EventTrigger>();
            //PointerClickの設定
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((eventData) => { TouchPC(); });
            trigger.triggers.Add(entry);
        }

        animator.SetTrigger("Idle");
    }

    public void TouchPC()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            string temp = animationList[Random.Range(0, animationList.Count)];
            animator.SetTrigger(temp);
        }
    }

    public void SetWalkTrigger(bool flag)
    {
        animator.SetBool("Walk", flag);
    }

    void SetDefaultFace()
    {
        emotionalController.ChangeEmotion(0);
    }
    void SetSmileFace()
    {
        emotionalController.ChangeEmotion(5);
    }
    void SetAngerFace()
    {
        emotionalController.ChangeEmotion(1);
    }
}
