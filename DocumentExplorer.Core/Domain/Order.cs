using System;
using System.Text.RegularExpressions;

namespace DocumentExplorer.Core.Domain
{
    public class Order
    {
        private readonly string CountryRegex = @"^[a-zA-Z]+$";

        public int Id { get; private set; }
        public string ClientCountry { get; private set; }
        public string ClientIdentificationNumber { get; private set; }
        public string BrokerCountry { get; private set; }
        public string BrokerIdentificationNumber { get; private set; }
        public string Owner1Name { get; private set; }
        public string Owner2Name { get; private set; }
        public int InvoiceNumber { get; set; }
        public bool IsFVP { get; set; }
        public bool IsCMR { get; set; }
        public DateTime CreationDate { get; set; }

        public Order(int id, string clientCountry, string clientIdentificationNumber, string brokerCountry, string brokerIdentificationNumber, string owner1Name, 
            DateTime creationDate,string owner2Name=null, int invoiceNumber=0, bool isCMR=false, bool isFVP=false)
        {
            SetId(id);
            SetClientCountry(clientCountry);
            SetClientIdentyficationNumber(clientIdentificationNumber);
            SetBrokerCountry(brokerCountry);
            SetBrokerIdentyficationNumber(brokerIdentificationNumber);
            Owner1Name = SetOwner(owner1Name);
            SetOwner2Name(owner2Name);
            InvoiceNumber = invoiceNumber;
            IsCMR = isCMR;
            IsFVP = isCMR;
            SetCreationDate(creationDate);
        }


        public void SetOwner2Name(string owner)
        {
            Owner2Name = SetOwner(owner);
        }

        private void SetCreationDate(DateTime date)
        {
            if (date == new DateTime()) CreationDate = DateTime.UtcNow;
            else CreationDate = date;
        }

        private void SetId(int id)
        {
            if (id <= 0) throw new DomainException(ErrorCodes.IvalidId);
            Id = id;
        }

        private void SetClientCountry(string country)
        {
            ClientCountry = SetCountry(country);
        }

        private void SetBrokerCountry(string country)
        {
            BrokerCountry = SetCountry(country);
        }

        private string SetCountry(string country)
        {
            if (country == null) throw new DomainException(ErrorCodes.InvalidCountry);
            if (country.Length != 2) throw new DomainException(ErrorCodes.InvalidCountry);
            if (!Regex.IsMatch(country, CountryRegex)) throw new DomainException(ErrorCodes.InvalidCountry);
            return country.ToUpper();
        }

        private void SetClientIdentyficationNumber(string number)
        {
            ClientIdentificationNumber = SetIdentyficationNumber(number);
        }

        private void SetBrokerIdentyficationNumber(string number)
        {
            BrokerIdentificationNumber = SetIdentyficationNumber(number);
        }

        private string SetIdentyficationNumber(string number)
        {
            if (number == null) throw new DomainException(ErrorCodes.InvalidNIP);
            return number;
        }

        private string SetOwner(string owner)
        {
            if (owner == null) throw new DomainException(ErrorCodes.InvalidUsername);
            if (owner.Length != 4) throw new DomainException(ErrorCodes.InvalidUsername);
            return owner.ToLower();
        }
    }
}
