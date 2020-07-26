using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SenarioMasterData {

    public List<SenarioData> senario = new List<SenarioData>();   // JsonのTitleと合わせること

    [System.Serializable]
    public class SenarioData {
        // Jsonで読み込む
        public int senarioNo;
        public string messageString;
        public string charaNoString;
        public string branchString;
        public string displayCharaString;
        public int backgroundImageNo;

        // 読み込んだDataを配列に置き換えて代入
        public string[] messages;
        public CHARA_NAME_TYPE[] charaTypes;
        public int[] branchs;
        public Dictionary<int, CHARA_NAME_TYPE[]> displayCharas;
    }
}
