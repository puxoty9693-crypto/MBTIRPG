using UnityEngine;
using System.Collections.Generic;

public static class ClassMatcher
{
    public static ClassDefinitionSO Match(IReadOnlyList<int> answers, SurveyDatabaseSO survey, ClassDatabaseSO classDb) 
    {
        return Match(answers, survey, classDb, out _);
    }

    public static ClassDefinitionSO Match(IReadOnlyList<int> answers, SurveyDatabaseSO survey, ClassDatabaseSO classDb, out int bestDistance) 
    {
        bestDistance = int.MaxValue;

        if (survey == null || classDb == null || classDb.classes.Count == 0) 
        {
            Debug.LogWarning("[Survey] 매칭 실패: 설문 또는 클래스 데이터가 비어 있습니다.");
            return null;
        }

        if (answers == null || answers.Count != survey.Count)
            GameLog.Warn("Survey", $"답변 개수({answers?.Count ?? 0})가 문항 수({survey.Count})와 다릅니다. 빠진 답은 A로 계산합니다.");
         
        ClassDefinitionSO best = null;
        foreach (ClassDefinitionSO c in classDb.classes)
        {
            if (c == null) continue;

            int d = Distance(answers, survey, c);
            bool better = best == null || d < bestDistance || (d == bestDistance && c.matchPriority < best.matchPriority);

            if (better) 
            {
                best = c;
                bestDistance = d;
            }
        }
        return best;
    }

    // 답변과 클래스 성향의 거리 (작을 수록 비슷)
    public static int Distance(IReadOnlyList<int> answers, SurveyDatabaseSO survey, ClassDefinitionSO c) 
    {
        int distance = 0;
        for (int i = 0; i < survey.Count; i++) 
        {
            SurveyQuestionSO q = survey.questions[i];
            if (q == null) continue;

            int answer = (answers != null && i < answers.Count) ? answers[i] : 0;
            int trait = (int)c.GetTrait(q.axis);

            if (answer != trait) distance += Mathf.Max(1, q.weight);
        }
        return distance;
    }

    // 저장된 classId로 클래스 찾기, 없으면 첫 번째 클래스로 대화.
    public static ClassDefinitionSO ResolveSC(string classId, ClassDatabaseSO classDb) 
    {
        if (classDb == null || classDb.classes.Count == 0) return null;

        ClassDefinitionSO found = classDb.FindId(classId);
        if (found != null) return found;

        Debug.LogWarning($"[Survey] 저장된 클래스     {classId}를 찾을 수 없어 {classDb.classes[0].id}로 대체합니다.");
        return classDb.classes[0];
    }
}
