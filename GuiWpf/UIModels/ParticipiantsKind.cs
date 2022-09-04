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

        [EnumDescription("מישראל")]
        FromIsrael,

        [EnumDescription("מהתפוצות")]
        FromWorld,

        [EnumDescription("מישראל בלי חברותא")]

        FromIsraelWithoutPair,

        [EnumDescription("מהתפוצות בלי חברותא")]
        FromWorldWithoutPair,
    }
}
