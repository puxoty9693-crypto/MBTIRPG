using UnityEngine;

public class PlayerInputDebug : MonoBehaviour
{
    IInputSource input;

    void Awake()
    {
        input = GetComponent<IInputSource>();   // 인터페이스는 인스펙터 직렬화가 안 돼서 GetComponent로 찾음
        if (input == null) GameLog.Error("Input", "IInputSource 컴포넌트가 없습니다.", this);
    }

    void Update()
    {
        if (input == null) return;
        if (Mathf.Abs(input.MoveX) > 0.01f) GameLog.Log("Input", $"Move {input.MoveX:F2}");
        if (input.DownHeld) GameLog.Log("Input", "Down");
        if (input.JumpPressed) GameLog.Log("Input", "Jump");
        if (input.DashPressed) GameLog.Log("Input", "Dash");
        if (input.AttackPressed) GameLog.Log("Input", "Attack");
        if (input.SkillPressed) GameLog.Log("Input", "Skill");
        if (input.UltimatePressed) GameLog.Log("Input", "Ultimate");
        if (input.PausePressed) GameLog.Log("Input", "Pause");
    }
}