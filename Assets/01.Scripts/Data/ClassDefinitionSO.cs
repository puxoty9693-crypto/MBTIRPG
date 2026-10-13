using UnityEngine;

[CreateAssetMenu(fileName = "Class_", menuName = "MBTI/Class Definition")]
public class ClassDefinitionSO : ScriptableObject
{
    [Header("기본 정보")]
    //저장 파일에 들어가는 고정 ID
    public string id;                                 
    public string displayName;                         //화면에 보이는 이름
    [TextArea(2, 4)] public string summary;            //성향 요약
    public Sprite icon;
    //매칭 동점일 때 숫자가 작은 클래스가 우선
    public int matchPriority;

    [Header("성향 (설문 측별)")]
    public Choice range;         // A 근거리 / B 원거리
    public Choice element;       // A 물리 / B 마법
    public Choice style;         // A 빠르고 가볍게 / B 느리고 묵직하게
    public Choice role;          // A 공격 위주 / B 생존 위주
    public Choice method;        // A 직접 / B 소환·설치물

    [Header("능력치")]
    public float maxHP = 100f;
    public float attackPower = 10f;
    public float moveSpeed = 8f;
    //기본 공격 간격(초)
    public float attackCooldown = 0.35f;
    [Range(0f, 1f)] public float defenseReduction = 0f;

    [Header("전투")]
    public AttackKind basicAttack = AttackKind.Melee;
    // 체크하면 대시 대신 순간이동 (마법사)
    public bool dashIsTeleport;
    public GameObject prefab;  // 나중에 클래스별 캐릭터 프리펩

    [Header("결과 화면 설정 (3주차 SkillDefinitionSO 전까지 암시)")]
    [TextArea(2, 4)] public string passiveText;
    [TextArea(2, 4)] public string skillText;
    [TextArea(2, 4)] public string ultimateText;

    public Choice GetTrait(SurveyAxis axis) 
    {
        switch (axis) 
        {
            case SurveyAxis.Range: return range;
            case SurveyAxis.Element: return element;
            case SurveyAxis.Style: return style;
            case SurveyAxis.Role: return role;
            case SurveyAxis.Method: return method;
            default: return Choice.A;
        }
    }
}
