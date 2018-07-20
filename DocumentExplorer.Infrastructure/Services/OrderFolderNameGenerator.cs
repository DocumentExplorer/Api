using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DocumentExplorer.Core.Domain;

namespace DocumentExplorer.Infrastructure.Services
{
    public class OrderFolderNameGenerator : IOrderFolderNameGenerator
    {
        public string OrderToName(Order order)
            => $"{GetYear(order)}/{GetMonth(order)}/{GetFolderBeginingName(order)}{GetInvoiceNumber(order)}{AddFVP(order)}{AddCMR(order)}{Add2OwnerName(order)}/";

        public List<Order> ListOfOrders(List<string> orders)
        {
            List<Order> orderList = new List<Order>();

            foreach(var order in orders)
            {
                orderList.Add(NameToOrder(order));
            }

            return orderList;
        }

        public Order NameToOrder(string order)
        {
            var tab = order.Split("/");
            var creationDate = GetCreationDate(tab[0], tab[1]);
            tab = tab[3].Split("_");
            var orderNumber = GetOrderNumber(tab[0]);
            var clientCountry = GetClientCountry(tab[1]);
            var clientIdentificationNumber = GetClientIdentificationNumber(tab[1]);
            var brokerCountry = GetBrokerCountry(tab[2]);
            var brokerIdentificationNumber = GetBrokerIdentificationNumber(tab[2]);

            int invoiceNumber = 0;
            bool isCMR = false;
            bool isFVP = false;
            string secondOwnerName = null;

            for(int i = 4; i<tab.Length; i++)
            {
                switch(tab[i].Length)
                {
                    case 7:
                        invoiceNumber = GetInvoiceNumber(tab[i]);
                        break;
                    case 3:
                        if (tab[i] == "cmr") isCMR = true;
                        if (tab[i] == "fvp") isFVP = true;
                        break;
                    case 4:
                        secondOwnerName = tab[i];
                        break;
                }
            }

            return new Order(orderNumber, clientCountry, clientIdentificationNumber, brokerCountry, brokerIdentificationNumber, tab[3], creationDate,order,secondOwnerName,invoiceNumber, isCMR, isFVP);
        }

        private int GetInvoiceNumber(string tabElement)
        {
            var onlyDigits = tabElement.Replace("fvk", string.Empty);
            var withoutLeadingZeroes = onlyDigits.TrimStart(new Char[] { '0' });
            int number;
            int.TryParse(withoutLeadingZeroes, out number);
            return number;
        }

        private string GetBrokerIdentificationNumber(string brokerString)
        {
            var withoutBegining = brokerString.Replace("k", string.Empty);
            return withoutBegining.Replace(GetBrokerCountry(brokerString), string.Empty);
        }

        private string GetClientIdentificationNumber(string clientString)
        {
            var withoutBegining = clientString.Replace("k", string.Empty);
            return withoutBegining.Replace(GetClientCountry(clientString), string.Empty);
        }

        private string GetBrokerCountry(string brokerString)
        {
            var withoutBegining = brokerString.Replace("p", string.Empty);
            var country = withoutBegining.Substring(0, 2);
            return country;
        }

        private string GetClientCountry(string clientString)
        {
            var withoutBegining = clientString.Replace("k", string.Empty);
            var country = withoutBegining.Substring(0, 2);
            return country;
        }

        private int GetOrderNumber(string tabElement)
        {
            var onlyDigits = tabElement.Replace("zl", string.Empty);
            var withoutLeadingZeroes = onlyDigits.TrimStart(new Char[] { '0' });
            int number;
            int.TryParse(withoutLeadingZeroes, out number);
            return number;
        }

        private DateTime GetCreationDate(string year, string month)
        {
            var yearInt = GetYear(year);
            var monthInt = GetMonth(month);
            return new DateTime(yearInt, monthInt, 1);
        }

        private int GetYear(string tabElement)
        {
            int year;
            int.TryParse(tabElement, out year);
            return year;
        }

        private int GetMonth(string tabElement)
        {
            int month;
            int.TryParse(tabElement.Split("_")[0], out month);
            return month;
        }

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
            => $"zl{AddLeadingZeros(order.Number)}_k{order.ClientCountry}{order.ClientIdentificationNumber}_p{order.BrokerCountry}{order.BrokerIdentificationNumber}_{order.Owner1Name}";

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
