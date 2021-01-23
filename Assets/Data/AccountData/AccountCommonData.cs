using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AccountCommonData
{
    public static class AccountDefine
    {
        public const int maxLevel = 100; //最大プレイヤーレベル
        public const float rateNextExp = 1.1f;

        public const int firstNextExp = 100;
        public const int firstPlusNextExp = 100;

        public static readonly Dictionary<string, int> questType = new Dictionary<string, int>{ { "normal", 0 }, { "training", 1 } };
    }
}

