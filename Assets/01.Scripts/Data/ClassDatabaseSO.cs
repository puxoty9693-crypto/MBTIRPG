using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="ClassDatabase", menuName = "MBTI/Class Database")]
public class ClassDatabaseSO : ScriptableObject
{
    public List<ClassDefinitionSO> classes = new List<ClassDefinitionSO>();
    
    public ClassDefinitionSO FindId(string id) 
    {
        foreach (ClassDefinitionSO c in classes)
            if (c != null && c.id == id) return c;
        return null;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        var ids = new HashSet<string>();
        foreach (ClassDefinitionSO c in classes)
        {
            if (c == null) { Debug.LogWarning($"[Data] {name}: 빈 클래스 칸이 있습니다.", this); continue;}
            if (string.IsNullOrEmpty(c.id)) Debug.LogWarning($"[Data] {c.name}: ID가 비어 있습니다.", c);
            else if (!ids.Add(c.id)) Debug.LogWarning($"[Data] id '{c.id}'가 중복됩니다.", c);
        }
    }
#endif
}
