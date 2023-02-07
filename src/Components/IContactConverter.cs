using Sufficit.Contacts;
using System;

namespace Sufficit.Blazor.Client.Components
{
    public class IContactConverter : MudBlazor.Converter<IContact, string>
    {
        public IContactConverter()
        {
            SetFunc = x => x == null || x.Id == Guid.Empty ? string.Empty : x.Title ?? "Desconhecido, não encontrado !";
        }

        public static IContactConverter Instance { get; } = new IContactConverter();
    }
}
