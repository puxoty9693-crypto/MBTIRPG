using UnityEngine;

public interface IInputSource
{
    float MoveX { get; }          // -1 ~ 1
    bool DownHeld { get; }        // 아래 + 점프 = 발판 하강

    bool JumpPressed { get; }     // 이번 프레임에 눌림
    bool JumpHeld { get; }        // 누르고 있는 중
    bool DashPressed { get; }
    bool AttackPressed { get; }
    bool SkillPressed { get; }
    bool UltimatePressed { get; }
    bool PausePressed { get; }
}
