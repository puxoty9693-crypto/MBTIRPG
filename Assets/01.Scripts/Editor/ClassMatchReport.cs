using UnityEngine;
using System.Text;
using System.Collections.Generic;
using UnityEditor;


public static class ClassMatchReport
{
    [MenuItem("MBTI/매칭 결과표 출력")]
    static void Run() 
    {
        SurveyDatabaseSO survey = FindAsset<SurveyDatabaseSO>();
        ClassDatabaseSO classDb = FindAsset<ClassDatabaseSO>();
        if (survey == null || classDb == null) 
        {
            Debug.LogError("[Match] SurveyDatabase 또는 ClassDatabase 에셋을 찾을 수 없습니다.");
            return;
        }

        int n = survey.Count;
        int total = 1 << n;
        var counts = new Dictionary<string, int>();
        foreach (ClassDefinitionSO c in classDb.classes) if (c != null) counts[c.displayName] = 0;

        int ties = 0, failed = 0;
        var sb = new StringBuilder();
        sb.AppendLine($"[Match] 매칭 결과표 ({n}문항, {total}가지)");

        for (int combo = 0; combo < total; combo++) 
        {
            var answers = new List<int>();
            for (int i = n - 1; i >= 0; i--) answers.Add((combo >> i) & 1);

            ClassDefinitionSO result = ClassMatcher.Match(answers, survey, classDb, out int dist);
            if (result == null) { failed++; continue; }

            //같은 거리의 클래스가 또 있으면 동점
            int sameDist = 0;
            foreach (ClassDefinitionSO c in classDb.classes)
                if (c != null && ClassMatcher.Distance(answers, survey, c) == dist) sameDist++;
            bool tie = sameDist > 1;
            if (tie) ties++;

            counts[result.displayName]++;

            var letters = new StringBuilder();
            foreach (int a in answers) letters.Append(a == 0 ? "A" : "B");
            sb.AppendLine($"{combo + 1:00}. {letters} -> {result.displayName} (거리 {dist}){(tie ? "동점" : "")}");
        }

        sb.AppendLine("----------------------------");
        foreach (var kv in counts) sb.AppendLine($"{kv.Key}: {kv.Value}개");
        sb.AppendLine($"동점 {ties}개 / 매칭 실패 {failed}개");

        if (failed > 0) Debug.LogError(sb.ToString());
        else Debug.Log(sb.ToString());
    }

    static T FindAsset<T>() where T : Object 
    {
        string[] guids = AssetDatabase.FindAssets($"t: {typeof(T).Name}");
        if (guids.Length == 0) return null;
        if (guids.Length > 1) Debug.LogWarning($"[Match] {typeof(T).Name} 에셋이 {guids.Length}개 있습니다. 첫 번째를 사용합니다.");
        return AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guids[0]));
    }
}
