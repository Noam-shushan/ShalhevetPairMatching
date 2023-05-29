using PairMatching.Tools;

namespace GuiWpf.UIModels
{
    public enum ParticipiantsKind
    {
        [EnumDescription("כל המשתתפים")]
        All,

        [EnumDescription("עם חברותא")]
        WithPair,

        [EnumDescription("בלי חברותא")]
        WithoutPair,

        [EnumDescription("ארכיון")]
        Archive
    }

    public enum ParticipiantsFrom
    {
        [EnumDescription("כל מקום")]
        All,

        [EnumDescription("מישראל")]
        FromIsrael,

        [EnumDescription("מהתפוצות")]
        FromWorld
    }

    public enum PairKind
    {
        [EnumDescription("כל החברותות")]
        All,

        [EnumDescription("חברותות פעילות")]
        Active,

        [EnumDescription("לומדות בפועל")]
        Learning
    }
}
