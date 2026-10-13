using UnityEngine;
using System.Collections.Generic;


public class SaveDebugPanel : MonoBehaviour
{
#if UNITY_EDITOR
    int slot;

    void OnGUI() 
    {
        SaveData d = SaveManager.Data;

        GUILayout.BeginArea(new Rect(10, 10, 240, 390), GUI.skin.box);
        GUILayout.Label("<b>Save Debug</b>");
        GUILayout.Label($"ID: {UserIdService.UserId}");

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("◀")) slot = Mathf.Max(0, slot - 1);
        GUILayout.Label($" 슬롯{slot}");
        if(GUILayout.Button("▶")) slot++;
        GUILayout.EndHorizontal();

        CharacterData c = d.GetCharacter(slot);
        if (c == null) 
        {
            GUILayout.Label("빈 슬롯");
            if (GUILayout.Button("검사 만들기"))
                d.SetCharacter(CharacterData.Create(slot, "Swordsman", new List<int> { 0, 0, 1, 0, 0, 0 }));
        }
        else 
        {
            GUILayout.Label($"{c.classId} ({c.createdAt})");
            GUILayout.Label($"재화: {c.currency} / HP 강화: {c.GetUpgradeLevel("hp")}단계");
            if (GUILayout.Button("재화 +100")) c.currency += 100;
            if (GUILayout.Button("HP 강화 +1")) c.SetUpgradeLevel("hp", c.GetUpgradeLevel("hp") + 1);
            if (GUILayout.Button("이 슬롯 지우기")) d.RemoveCharacter(slot);
        }

        GUILayout.Space(6);
        GUILayout.Label($"전체 캐릭터: {d.characters.Count}개");
        if (GUILayout.Button("저장")) SaveManager.Save();
        if (GUILayout.Button("불러오기")) SaveManager.Load();
        if (GUILayout.Button("저장 파일 삭제")) SaveManager.DeleteAll();
        if (GUILayout.Button("읽을 수 있는 JSON으로 내보내기")) 
        {
            string path = System.IO.Path.Combine(Application.persistentDataPath, "save_debug.json");
            System.IO.File.WriteAllText(path, JsonUtility.ToJson(d, true));
            GameLog.Log("Save", $"확인용 JSON 내보냄: {path}");
        }

        if (GUILayout.Button("저장 폴더 열기"))
            UnityEditor.EditorUtility.RevealInFinder(Application.persistentDataPath + "/");

        GUILayout.EndArea();

    }
#endif
}
