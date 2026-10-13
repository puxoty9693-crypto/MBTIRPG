using UnityEngine;
using System;


public static class SaveManager 
{
    static ISaveStorage storage;
    static SaveData data;

    public enum LoadResult { NotLoaded, Loaded, NewData, RestoredFromBackup}
    public static LoadResult LastLoadResult { get; private set; } = LoadResult.NotLoaded;
    public static bool LastSaveFailed { get; private set; }

    public static event Action OnLoaded;
    public static event Action OnSaved;

    public static ISaveStorage Storage => storage ??= new EncryptedLocalStorage(); //암호화 저장

    public static SaveData Data 
    {
        get 
        {
            if (data == null) Load();
            return data;
        }
    }

    public static bool IsLoaded => data != null;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void RestStatics() 
    {
        storage = null;
        data = null;
        LastLoadResult = LoadResult.NotLoaded;
        LastSaveFailed = false;
        OnLoaded = null;
        OnSaved = null;
        Application.quitting -= HandleQuitting;
        Application.quitting += HandleQuitting;
    }

    static void HandleQuitting() 
    {
        if (data != null) Save();
    }

    public static void SetStorage(ISaveStorage newStorage) 
    {
        storage = newStorage;
        data = null;
    }
   
    public static void Load() 
    {
        SaveData loaded = null;
        LoadResult result = LoadResult.Loaded;

        if (Storage.TryLoadMain(out string json))
            loaded = Parse(json);

        if (loaded == null) 
        {
            if (Storage.TryLoadBackup(out string backupJson))
            {
                loaded = Parse(backupJson);
                if (loaded != null)
                {
                    result = LoadResult.RestoredFromBackup;
                    Debug.LogWarning("[Save] 최신 저장 파일이 손상되어 백업본을 불러왔습니다.");
                }
            }
        }

        if (loaded == null) 
        {
            loaded = new SaveData();
            result = LoadResult.NewData;
            GameLog.Log("Save", "저장 파일 없음 -> 새 데이터 생성");
        }
        
        else 
        {
            GameLog.Log("Save", $"불러오기 완료 (v{loaded.saveVersion}, 캐릭터 {loaded.characters.Count}개)");
        }

        loaded.EnsureValid();
        data = Migrate(loaded);
        LastLoadResult = result;
        OnLoaded?.Invoke();
    }

    public static bool Save() 
    {
        if (data == null) 
        {
            GameLog.Warn("Save", "불러온 데이터가 없어 저장하지 않음");
            return false;
        }

        try
        {
            data.saveVersion = SaveData.CurrentVersion;
            string json = JsonUtility.ToJson(data, true);
            Storage.Save(json);
            LastSaveFailed = false;
            GameLog.Log("Save", "저장 완료");
            OnSaved?.Invoke();
            return true;
        }

        catch (Exception e) 
        {
            Debug.LogError($"[Save] 저장 실패: {e}");
            LastSaveFailed = true;
            return false;
        }
    }

    // 저장 파일 전부 삭제하고 새 데이터
    public static void DeleteAll() 
    {
        Storage.Delete();
        data = new SaveData();
        GameLog.Log("Save", "저장 파일 삭제");
    }

    static SaveData Parse(string json) 
    {
        try
        {
            return JsonUtility.FromJson<SaveData>(json);
        }

        catch (Exception e) 
        {
            Debug.LogWarning($"[Save] 저장 파일 해석 실패: {e.Message}");
            return null;
        }
    }

    static SaveData Migrate(SaveData d) 
    {
        d.saveVersion = SaveData.CurrentVersion;
        return d;
    }

}
