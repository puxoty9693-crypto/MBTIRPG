
public enum SurveyAxis 
{
    Range,    // 사거리: A 근거리 / B 원거리
    Element,  // 공격 속성: A 물리 / B 마법
    Style,    // 스타일: A 빠르고 가볍게 / B 느리고 묵직하게
    Role,     // 운용: A 공격 위주 / B 생존 위주
    Method    // 전투 방식: A 직접 싸움 / B 소환 설치물
}

public enum Choice 
{
    A = 0,
    B = 1,
}

public enum AttackKind 
{
    Melee,     // 근접  (Hitbox)
    Ranged     // 원거리 (투사체, 3주차)
}
