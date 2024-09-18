using MudBlazor;
using Sufficit.Gateway.PhoneVox;
using Sufficit.Gateway.ReceitaNet;
using Sufficit.Telefonia;
using Sufficit.Telephony;
using Sufficit.Telephony.FreePBX;

namespace Sufficit.Blazor.Client
{
    public static class IDestinationExtensions
    {
        public static string GetIcon(this IDestination? destination)
        {
            var typename = destination?.TypeName?.Trim();
            if (!string.IsNullOrWhiteSpace(typename)) 
            {
                if (typename.Equals(nameof(URA), System.StringComparison.InvariantCultureIgnoreCase) ||
                   typename.Equals(nameof(IVR), System.StringComparison.InvariantCultureIgnoreCase))
                    return Sufficit.Blazor.Client.Pages.Telephony.IVR.DashBoard.Icon!;

                if (typename.Equals(nameof(FilaEspera), System.StringComparison.InvariantCultureIgnoreCase) ||
                   typename.Equals(nameof(CallQueue), System.StringComparison.InvariantCultureIgnoreCase))
                    return Icons.Material.Filled.ReduceCapacity;

                if (typename.Equals(nameof(Anuncio), System.StringComparison.InvariantCultureIgnoreCase))
                    return Icons.Material.Filled.Campaign;

                if (typename.Equals(nameof(CaixaPostal), System.StringComparison.InvariantCultureIgnoreCase) || 
                    typename.Equals(nameof(MailBox), System.StringComparison.InvariantCultureIgnoreCase) ||
                    typename.Equals(nameof(FreePBXMailBox), System.StringComparison.InvariantCultureIgnoreCase))
                    return Icons.Material.Filled.Voicemail;

                if (typename.Equals(nameof(EndDestination), System.StringComparison.InvariantCultureIgnoreCase) ||
                    typename.Equals(nameof(FreePBXEndDestination), System.StringComparison.InvariantCultureIgnoreCase))
                    return Icons.Material.Filled.CallEnd;

                if (typename.Equals(nameof(CondicaoTempo), System.StringComparison.InvariantCultureIgnoreCase) ||
                    typename.Equals(nameof(TimeInterval), System.StringComparison.InvariantCultureIgnoreCase))
                    return Icons.Material.Filled.AccessTime;                

                if (typename.Equals(nameof(FreePBXDirectCall), System.StringComparison.InvariantCultureIgnoreCase) ||
                    typename.Equals(nameof(FreePBXExtensionDestination), System.StringComparison.InvariantCultureIgnoreCase) || 
                    typename.Equals(nameof(FreePBXExtLocal), System.StringComparison.InvariantCultureIgnoreCase) ||
                    typename.Equals(nameof(FreePBXDIDDirect), System.StringComparison.InvariantCultureIgnoreCase)
                    )
                    return Icons.Material.Filled.Fax;

                if (typename.Equals(nameof(PhoneVoxDestination), System.StringComparison.InvariantCultureIgnoreCase) ||
                    typename.Equals(nameof(RNOptions), System.StringComparison.InvariantCultureIgnoreCase) ||
                    typename.Equals(nameof(Sufficit.Gateway.ReceitaNet.RNDestination), System.StringComparison.InvariantCultureIgnoreCase))
                    return Sufficit.Blazor.Client.Pages.Gateway.DashBoard.Icon!;
            }

            return Icons.Material.Filled.QuestionMark;
        }
    }
}
