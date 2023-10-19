using Sufficit.Sales;
using System.Linq;

namespace Sufficit.Blazor.Client.Components
{
    public class StringToNumbersConverter : MudBlazor.Converter<string?, string>
    {
        public StringToNumbersConverter()
        {
            SetFunc = x => new string(x?.Where(s => char.IsDigit(s)).ToArray());
        }

        public static StringToNumbersConverter Instance { get; } = new StringToNumbersConverter();
    }
}
