using System.Collections.Generic;

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
        public int bgmNo;
        public string branchMessageString;
        public int autoScenarioNo;

        // 読み込んだDataを配列に置き換えて代入
        public string[] messages;
        public CHARA_NAME_TYPE[] charaTypes;
        public int[] branchs;
        public Dictionary<int, CHARA_NAME_TYPE[]> displayCharas;
        public string[] branchMessages;
        public List<int> conditionalBranchNo;
    }
}
