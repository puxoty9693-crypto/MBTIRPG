using UnityEngine;
using System;
using System.IO;


public class LocalFileStorage : ISaveStorage
{
    readonly string mainPath;
    readonly string backupPath;
    readonly string tempPath;

    public string MainPath => mainPath;

    public LocalFileStorage(string fileName = "save.json") 
    {
        mainPath = Path.Combine(Application.persistentDataPath, fileName);
        backupPath = mainPath + ".bak";
        tempPath = mainPath + ".tmp";
    }

    public bool Exists => File.Exists(mainPath) || File.Exists(backupPath);

    public bool TryLoadMain(out string content) => TryRead(mainPath, out content);
    public bool TryLoadBackup(out string content) => TryRead(backupPath, out content);

    public void Save(string content) 
    {
        File.WriteAllText(tempPath, content);
        if (File.Exists(mainPath)) File.Copy(mainPath, backupPath, true);
        File.Copy(tempPath, mainPath, true);
        File.Delete(tempPath);
    }

    public void Delete() 
    {
        foreach (string path in new[] {mainPath, backupPath, tempPath})
            if (File.Exists(path)) File.Delete(path);
    }

    static bool TryRead(string path, out string content) 
    {
        content = null;
        try 
        {
            if (!File.Exists(path)) return false;
            content = File.ReadAllText(path);
            return !string.IsNullOrEmpty(content);
        }
        
        catch (Exception e) 
        {
            GameLog.Warn($"[Save] 파일 읽기 실패: {path}\n{e.Message}");
            return false;
        }
        
    }

}
