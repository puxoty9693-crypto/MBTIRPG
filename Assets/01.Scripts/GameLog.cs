using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

public static class GameLog
{
    // ---------- 일반 ----------
    [Conditional("UNITY_EDITOR")]
    public static void Log(string message, Object context = null)
        => Debug.Log(message, context);

    [Conditional("UNITY_EDITOR")]
    public static void Log(string tag, object message, Object context = null)
        => Debug.Log(Format(tag, message), context);

    // ---------- 경고 ----------
    [Conditional("UNITY_EDITOR")]
    public static void Warn(string message, Object context = null)
        => Debug.LogWarning(message, context);

    [Conditional("UNITY_EDITOR")]
    public static void Warn(string tag, object message, Object context = null)
        => Debug.LogWarning(Format(tag, message), context);

    // ---------- 에러 ----------
    [Conditional("UNITY_EDITOR")]
    public static void Error(string message, Object context = null)
        => Debug.LogError(message, context);

    [Conditional("UNITY_EDITOR")]
    public static void Error(string tag, object message, Object context = null)
        => Debug.LogError(Format(tag, message), context);

    // ---------- 태그 색상 ----------
    static string Format(string tag, object message)
        => $"<color=#{TagColor(tag)}><b>[{tag}]</b></color> {message}";

    // 태그 이름으로 색을 정해서 같은 태그는 항상 같은 색으로 보이게 함
    static string TagColor(string tag)
    {
        int h = 17;
        foreach (char c in tag) h = h * 31 + c;
        float hue = (h & 0x7fffffff) % 360 / 360f;
        return ColorUtility.ToHtmlStringRGB(Color.HSVToRGB(hue, 0.55f, 1f));
    }
}
