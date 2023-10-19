using MudBlazor;

namespace Sufficit.Blazor.Client
{
    public class MudBlazorDocumentMask : RegexMask
    {
        const string regex = @"^((?<cpf>(\d{1,3}|\d{3}\.)(\d{1,3}|\d{3}\.)?(\d{1,3}|\d{3}-)?(\d{1,2})?)|(?<cnpj>(\d{1,2}|\d{2}\.)(\d{1,3}|\d{3}\.)?(\d{1,3}|\d{3}\/)?(\d{1,4}|\d{4}-)?(\d{1,2})?))$";

        public MudBlazorDocumentMask(string? mask = null) : base(regex, mask)
        {
            Delimiters = ".-/";
        }
    }
}
