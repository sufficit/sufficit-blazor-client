using MudBlazor;
using Sufficit.Telephony;

namespace Sufficit.Blazor.Client
{
    public static class IDestinationExtensions
    {
        public static string GetIcon(this IDestination destination)
        {
            return destination.TypeName.Trim().ToLowerInvariant() switch
            {
                "mailbox" or "freepbxmailbox" => Icons.Material.Filled.Voicemail,
                "enddestination" or "freepbxenddestination" => Icons.Material.Filled.CallEnd,
                "directextensiondialing" or "diddirect" => Icons.Material.Filled.Fax,
                "timecondition" or "condicaotempo" => Icons.Material.Filled.AccessTime,
                _ => Icons.Material.Filled.QuestionMark,
            };
        }
    }
}
