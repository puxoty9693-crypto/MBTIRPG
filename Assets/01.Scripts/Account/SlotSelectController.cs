using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEditor.Rendering;



public class SlotSelectController : MonoBehaviour
{
    [SerializeField] ClassDatabaseSO classDb;
    [SerializeField] List<SlotView> slotViews = new List<SlotView>();
    [SerializeField] Button slotButton;
    [SerializeField] TMP_Text noticeText;

    //씬 이름
    [SerializeField] string surSceneName = "SurveyScene";
    [SerializeField] string gameSceneName = "TestScene";

#if UNITY_EDITOR
    //DLC 테스트
    [SerializeField] int editorExtraSlots = 0;
#endif


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        string id = UserIdService.UserId;      //최초 1회 고유 ID 발급
        GameLog.Log("Slot", $"유저 ID: {id}");


#if UNITY_EDITOR
        if (EntitlementService.Provider is LocalEntitlementService local)
            local.DebugExtraSlots = editorExtraSlots;
#endif

        if (slotButton != null) slotButton.onClick.AddListener(OnAddSlotClicked);

        ShowLoadNotice();
        Refresh();
    }

    public void Refresh() 
    {
        int unlocked = Mathf.Min(EntitlementService.UnlockedSlotCount, slotViews.Count);

        for (int i = 0; i < slotViews.Count; i++) 
        {
            SlotView view = slotViews[i];
            if (view == null) continue;

            bool open = i < unlocked;
            view.gameObject.SetActive(open);
            if (!open) continue;

            int index = i;
            view.Button.onClick.RemoveAllListeners();
            view.Button.onClick.AddListener(() => OnSlotClicked(index));

            CharacterData data = SaveManager.Data.GetCharacter(i);
            if (data == null) view.ShowEmpty(i);
            else view.ShowUsed(i, classDb != null ? classDb.FindId(data.classId) : null, data);

        }

        if (slotButton != null)
            slotButton.gameObject.SetActive(unlocked < Mathf.Min(EntitlementService.MaxSlots, slotViews.Count));

    }

    void OnAddSlotClicked() 
    {
#if UNITY_EDITOR

        if (EntitlementService.Provider is LocalEntitlementService local)
        {
            local.DebugExtraSlots++;
            GameLog.Log("Slot", $"[테스트] 슬롯 추가 -> {EntitlementService.UnlockedSlotCount}칸");
            Refresh();
            return;
        }
#endif
        SetNotice("추가 슬롯은 DLC로 구매할 수 없습니다.");
    }



    void OnSlotClicked(int index)
    {
        if (index >= EntitlementService.UnlockedSlotCount) return;

        GameSession.SelectedSlot = index;
        bool isEmpty = SaveManager.Data.GetCharacter(index) == null;
        string scene = isEmpty ? surSceneName : gameSceneName;

        GameLog.Log("Slot", $"슬롯 {index + 1} 선택 -> {(isEmpty ? "설문" : "게임")}({scene})");
        if (!string.IsNullOrEmpty(scene)) SceneManager.LoadScene(scene);
    }

    void ShowLoadNotice() 
    {
        SetNotice("");
        if (SaveManager.LastLoadResult == SaveManager.LoadResult.RestoredFromBackup)
            SetNotice("저장 파일이 손상되어 이전 저장본을 불러왔습니다.");
    }

    void SetNotice(string message) 
    {
        if (noticeText == null) return;
        noticeText.text = message;
        noticeText.gameObject.SetActive(!string.IsNullOrEmpty(message));
    }

#if UNITY_EDITOR
    // ===== 에디터 테스트 전용: 빌드에는 포함 안 됨 =====
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(Screen.width - 220, 10, 210, 120), GUI.skin.box);
        GUILayout.Label("[에디터 테스트]");

        if (GUILayout.Button("전체 초기화 (세이브 + 슬롯)", GUILayout.Height(30)))
            EditorResetAll();

        GUILayout.EndArea();
    }

    void EditorResetAll()
    {
        SaveManager.DeleteAll();   // 세이브 파일 + 백업 삭제, 데이터 새로 만들기

        if (EntitlementService.Provider is LocalEntitlementService local)
            local.DebugExtraSlots = 0;   // + 로 늘린 칸도 원래대로

        GameLog.Log("Slot", "[테스트] 전체 초기화");
        SetNotice("");
        Refresh();
    }
#endif


}
