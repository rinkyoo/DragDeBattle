using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common
{
    public static class Define
    {
        public const int ptNum = 4; //編成数
        public const int charaNum = 7; //クエストで使用するキャラ数

        public const int maxFieldPCNum = 5; //同時に召喚できるPC数

        public static Vector3 initialPosi = new Vector3(-10f, 0f, 0f); //フィールドに召喚前の待機座標

        public const float waitTime = 30f; //再召喚までに必要な時間
    }
}
