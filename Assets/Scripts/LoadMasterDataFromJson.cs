using UnityEngine;

public class LoadMasterDataFromJson {

    /// <summary>
    /// ジェネリクス
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T LoadFromJson<T>() {
        // Jsonファイルを読み込んでSenarioMasterDataを作成する
        return JsonUtility.FromJson<T>(JsonHelper.GetJsonFile("/", "senario.json"));
    }

    /// <summary>
    /// JsonファイルからSenarioMasterDataを取得
    /// </summary>
    /// <returns></returns>
    public static SenarioMasterData LoadSenarioMasterDataFromJson() {
        // Jsonファイルを読み込んでSenarioMasterDataを作成する
        return JsonUtility.FromJson<SenarioMasterData>(JsonHelper.GetJsonFile("/", "senario.json"));
    }
}