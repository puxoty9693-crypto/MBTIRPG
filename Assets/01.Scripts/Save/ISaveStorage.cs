using UnityEngine;

public interface ISaveStorage
{
    bool Exists {  get; }
    bool TryLoadMain(out string content); //최신 저장본
    bool TryLoadBackup(out string content); // 직전 저장본
    void Save(string content);
    void Delete();
}
