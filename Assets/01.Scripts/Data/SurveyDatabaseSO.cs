using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SurveyDatabase", menuName = "MBTI/Survey Database")]
public class SurveyDatabaseSO : ScriptableObject
{
    public List<SurveyQuestionSO> questions = new List<SurveyQuestionSO>();

    public int Count => questions.Count;

#if UNITY_EDITOR
    private void OnValidate()
    {
        var seen = new HashSet<SurveyAxis>();
        foreach (SurveyQuestionSO q in questions) 
        {
            if (q == null) { Debug.LogWarning($"[Data] {name}: 빈 문항 칸이 있습니다.", this); continue;}
            if (!seen.Add(q.axis)) Debug.LogWarning($"[Data] {name}: '{q.axis}' 축 문항이 2개 이상입니다.", this);
        }
    }
#endif
}
