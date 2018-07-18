using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DocumentExplorer.Core.Domain;

namespace DocumentExplorer.Infrastructure.Services
{
    public class OrderFolderNameGenerator : IOrderFolderNameGenerator
    {
        public string OrderToName(Order order)
            => $"{GetYear(order)}/{GetMonth(order)}/{GetFolderBeginingName(order)}{GetInvoiceNumber(order)}{AddFVP(order)}{AddCMR(order)}{Add2OwnerName(order)}";

        private string GetYear(Order order)
            => order.CreationDate.Year.ToString();
        private string GetMonth(Order order)
        {
            string month = "";
            switch(order.CreationDate.Month)
            {
                case 1:
                    month = "1_styczen";
                    break;
                case 2:
                    month = "2_luty";
                    break;
                case 3:
                    month = "3_marzec";
                    break;
                case 4:
                    month = "4_kwiecien";
                    break;
                case 5:
                    month = "5_maj";
                    break;
                case 6:
                    month = "6_czerwiec";
                    break;
                case 7:
                    month = "7_lipiec";
                    break;
                case 8:
                    month = "8_sierpien";
                    break;
                case 9:
                    month = "9_wrzesien";
                    break;
                case 10:
                    month = "10_pazdziernik";
                    break;
                case 11:
                    month = "11_listopad";
                    break;
                case 12:
                    month = "12_grudzien";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return month;
        }

        private string GetFolderBeginingName(Order order)
            => $"zl{AddLeadingZeros(order.Id)}_k{order.ClientCountry}{order.ClientIdentificationNumber}_p{order.BrokerCountry}{order.BrokerIdentificationNumber}_{order.Owner1Name}";

        private string GetInvoiceNumber(Order order)
        {
            if (order.InvoiceNumber != 0)
                return $"_fvk{AddLeadingZeros(order.InvoiceNumber)}";
            return string.Empty;
        }

        private string AddFVP(Order order)
        {
            if (order.IsFVP) return "_fvp";
            return string.Empty;
        }

        private string AddCMR(Order order)
        {
            if (order.IsCMR) return "_cmr";
            return string.Empty;
        }

        private string Add2OwnerName(Order order)
        {
            if (order.Owner2Name != null) return $"_{order.Owner2Name}";
            return string.Empty;
        }

        private string AddLeadingZeros(int number)
        {
            string s = number.ToString();
            int zeroes = 4 - s.Length;
            for(int i=0; i<zeroes; i++)
            {
                s = $"0{s}";
            }
            return s;
        }
    }
}
