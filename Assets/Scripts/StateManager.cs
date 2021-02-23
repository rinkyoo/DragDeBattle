using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StateManager {
    //複数のプラットフォームに対応できるよう、画面のタッチ情報をそれぞれの処理で取得
    public class TouchManager {
        public bool touch_flag;      // タッチ有無
        public Vector2 touch_position;   // タッチ座標
        public TouchPhase touch_phase;   // タッチ状態
        
        public TouchManager(bool flag = false, Vector2? position = null, TouchPhase phase = TouchPhase.Began) {
            this.touch_flag = flag;
            if (position == null) {
                this.touch_position = new Vector2(0, 0);
            } else {
                this.touch_position = (Vector2)position;
            }
            this.touch_phase = phase;
        }
        
        public void update() {
            this.touch_flag = false;

            // 画面タップの場合（端末想定）
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);
                    this.touch_position = touch.position;
                    this.touch_phase = touch.phase;
                    this.touch_flag = true;
                }
            }
            //マウス使用の場合
            else
            {
                // 押した瞬間
                if (Input.GetMouseButtonDown(0))
                {
                    this.touch_flag = true;
                    this.touch_phase = TouchPhase.Began;
                    this.touch_position = Input.mousePosition;
                    return;
                }

                // 離した瞬間
                if (Input.GetMouseButtonUp(0))
                {
                    this.touch_flag = true;
                    this.touch_phase = TouchPhase.Ended;
                    this.touch_position = Input.mousePosition;
                    return;
                }

                // 押しっぱなし
                if (Input.GetMouseButton(0))
                {
                    this.touch_flag = true;
                    this.touch_phase = TouchPhase.Moved;
                    this.touch_position = Input.mousePosition;
                    return;
                }
                // タップ使用（端末想定）
            }
        }
            /*
            // マウス使用
            if (Application.isEditor) {
                // 押した瞬間
                if (Input.GetMouseButtonDown(0)) {
                    this.touch_flag = true;
                    this.touch_phase = TouchPhase.Began;
                    this.touch_position = Input.mousePosition;
                    return;
                }

                // 離した瞬間
                if (Input.GetMouseButtonUp(0)) {
                    this.touch_flag = true;
                    this.touch_phase = TouchPhase.Ended;
                    this.touch_position = Input.mousePosition;
                    return;
                }

                // 押しっぱなし
                if (Input.GetMouseButton(0)) {
                    this.touch_flag = true;
                    this.touch_phase = TouchPhase.Moved;
                    this.touch_position = Input.mousePosition;
                    return;
                }
            // タップ使用（端末想定）
            } else {
                if (Input.touchCount > 0) {
                    Touch touch = Input.GetTouch(0);
                    this.touch_position = touch.position;
                    this.touch_phase = touch.phase;
                    this.touch_flag = true;
                }
            }
            */

        //タッチ状態を取得
        public TouchManager getTouch()
        {
            return new TouchManager(this.touch_flag, this.touch_position, this.touch_phase);
        }
    }
}
