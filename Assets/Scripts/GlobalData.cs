using System;
namespace GlobalData
{
    public struct PlayingCardData
    {
        public int CardCode;
        public string SpriteName;
        public int SpriteCode;
        public PlayingCardData(int cardCode,string spriteName,int spriteCode)
        {
            CardCode = cardCode;
            SpriteName = spriteName;
            SpriteCode = spriteCode;
        }
    }
    public enum EExpansion
    {
        None = -1, // 오류
        Original,  // 본판
        DodgeCity, // 닷지시티
        WWS,       // 와일드웨스트쇼
        GoldRush,  // 골드러시
        TheValleyOfShadows,  //그림자의 계곡
        Privilege, // 특전
    }

    public enum EPlayType
    {
        None = -1,    // 오류
        Consumption,  // 소비 카드
        Mounting,     // 장착 카드
    }

    [Serializable]
    public enum EPlayPhase
    {
        JoinUnReadyPhase,
        JoinReadyPhase,
        JoinWaitPhase,
        JobPhase,
        CharaterPhase,
        DeckPhase,
        GamseStartPhase,
        GameStartWaitPhase,
        CardFlipPhase,
        CardDrawPhase,
        ActionPhase,
        ClenUpPhase,
        WaitPhase,
        ReActionPhase,
        GameEndPhase,
        QuitPhase,
    }
    public enum EPlayingCardClass
    {
        None = -1, // 오류
        // 소비 카드
        Bang,       // 뱅
        Miss,       // 빗나감
        Beer,       // 맥주
        Duel,       // 결투
        Indian,     // 인디언
        Gatling,    // 기관총
        Saloon,     // 주점
        Panic,      // 강탈
        CatBalou,   // 캣 벌로우
        GeneralStore, // 잡화점
        Stagecoach,   // 역마차
        WellsFargo,   // 웰스 파고

        // 장착 카드
        Schofield,   // 스코필드
        Remington,   // 레밍턴
        Carabine,    // 카빈
        Winchester,  // 윈체스터
        Volcanic,    // 볼케닉
        Scope,       // 조준경
        Mustang,     // 야생마
        Barrel,      // 술통
        Jail,        // 감옥
        Dynamite,    // 다이너마이트
    }
    
    public enum EJobCardClass
    {
        Sceriffo, // 보안관
        Vice,     // 부관
        Fuorilegge, // 무법자
        Rinnegato,  // 배신자
    }

    public enum ECharCardClass
    {
        Jourdonnais = 23, // 주르 도네
        BartCassidy,  // 바트 캐시디
        WillyTheKid = 26,  //윌리더키드
        SidKetchum,   // 시드 케첨
        RoseDollan = 28,  // 로즈 돌란
        PaulRegret = 31,  // 폴 리그레트
        SlabTheKiller = 34,  // 슬랩 더 킬러
        SuzyLafayette, // 수지 라파예트
        CalamityJanet = 38,  //캘러미티 자넷
    }
    public enum ERaiseEvent
    {
        CardDrawSend,
        CardDrawRecv,
        ReadySend,
        JobDrawSend,
        CharDrawSend,
        DeckSetSend,
        BangAttackSend,
        BangAttackRecv,
        MissSend,
    }

    [Flags] public enum EExpansionFlags
    {
        None = 0, // 오류
        Original = 1,  // 본판
        DodgeCity = 2, // 닷지시티
        WWS = 4,       // 와일드웨스트쇼
        GoldRush = 8,  // 골드러시
        TheValleyOfShadows = 16,  //그림자의 계곡
        Privilege = 32, // 특전
    }
}