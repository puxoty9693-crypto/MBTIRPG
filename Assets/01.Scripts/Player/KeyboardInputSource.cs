using UnityEngine;
using UnityEngine.InputSystem;

public class KeyboardInputSource : MonoBehaviour, IInputSource
{
    InputAction move, jump, dash, attack, skill, ultimate, pause;

    void Awake()
    {
        var actions = InputSystem.actions;   // Project Settings > Input System Package 에 지정된 에셋
        move = actions.FindAction("Move", true);
        jump = actions.FindAction("Jump", true);
        dash = actions.FindAction("Dash", true);
        attack = actions.FindAction("Attack", true);
        skill = actions.FindAction("Skill", true);
        ultimate = actions.FindAction("Ultimate", true);
        pause = actions.FindAction("Pause", true);
    }

    void OnEnable() => InputSystem.actions.Enable();

    public float MoveX => move.ReadValue<Vector2>().x;
    public bool DownHeld => move.ReadValue<Vector2>().y < -0.5f;

    // WasPressedThisFrame은 Update에서 읽어야 놓치지 않음 (FixedUpdate에서 읽지 말 것)
    public bool JumpPressed => jump.WasPressedThisFrame();
    public bool JumpHeld => jump.IsPressed();
    public bool DashPressed => dash.WasPressedThisFrame();
    public bool AttackPressed => attack.WasPressedThisFrame();
    public bool SkillPressed => skill.WasPressedThisFrame();
    public bool UltimatePressed => ultimate.WasPressedThisFrame();
    public bool PausePressed => pause.WasPressedThisFrame();
}
