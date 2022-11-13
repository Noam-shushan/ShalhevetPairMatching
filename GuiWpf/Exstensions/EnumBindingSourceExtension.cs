using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using static PairMatching.Tools.Extensions;


namespace GuiWpf.Exstensions
{
    public class EnumBindingSourceExtension : MarkupExtension
    {
        public EnumBindingSourceExtension() { }

        public Type EnumType { get; private set; }

        public EnumBindingSourceExtension(Type enumType)
        {
            if(enumType is null || !enumType.IsEnum)
            {
                throw new ArgumentNullException(nameof(enumType));
            }
            EnumType = enumType;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var enumValues = Enum.GetValues(EnumType);
            var result = enumValues
                .Cast<Enum>()
                .Select(value => value.GetDescriptionFromEnumValue());
            return from v in result
                   where v != string.Empty
                   select v;
        }
    }
}
