using Sufficit.Sales;

namespace Sufficit.Blazor.Client.Components
{
    public class IClientConverter : MudBlazor.Converter<IClient, string>
    {
        public IClientConverter()
        {
            SetFunc = x => x?.Title ?? "Desconhecido, não encontrado !";
        }

        public static IClientConverter Instance { get; } = new IClientConverter();
    }
}
