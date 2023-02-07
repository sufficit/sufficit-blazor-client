using Sufficit.Contacts;
using System;

namespace Sufficit.Blazor.Client.Components
{
    public class IIdTitlePairContactConverter : MudBlazor.Converter<IIdTitlePair, string>
    {
        public IIdTitlePairContactConverter()
        {
            SetFunc = x => x == null || x.Id == Guid.Empty ? string.Empty : x.Title ?? "Desconhecido, não encontrado !";
        }

        public static IIdTitlePairContactConverter Instance { get; } = new IIdTitlePairContactConverter();
    }
}
