using Sufficit.Telephony;

namespace Sufficit.Blazor.Client.Components
{
    public class IDestinationConverter : MudBlazor.Converter<IDestination, string>
    {
        public IDestinationConverter()
        {
            SetFunc = x => x?.Title!;
        }

        public static IDestinationConverter Instance { get; } = new IDestinationConverter();
    }
}
