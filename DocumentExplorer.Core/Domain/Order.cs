using System;
using System.Text.RegularExpressions;

namespace DocumentExplorer.Core.Domain
{
    public class Order
    {
        public Guid Id { get; private set; }
        public int Number { get; private set; }
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
        public string CreationDateString 
        {
            get
            {
                var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
                DateTime cstTime = TimeZoneInfo.ConvertTimeFromUtc(CreationDate, timeZoneInfo);
                return cstTime.ToString(@"dd.MM.yyyy HH:mm:ss");
            }
        }
        public string PathToFolder { get; set; }

        protected Order()
        {

        }

        public Order(int number, string clientCountry, string clientIdentificationNumber, 
            string brokerCountry, string brokerIdentificationNumber, string owner1Name, 
            DateTime creationDate, string pathToFolder = null,string owner2Name=null, int invoiceNumber=0, 
            bool isCMR=false, bool isFVP=false)
        {
            Id = Guid.NewGuid();
            SetNumber(number);
            SetClientCountry(clientCountry);
            SetClientIdentificationNumber(clientIdentificationNumber);
            SetBrokerCountry(brokerCountry);
            SetBrokerIdentificationNumber(brokerIdentificationNumber);
            Owner1Name = SetOwner(owner1Name);
            SetOwner2Name(owner2Name);
            InvoiceNumber = invoiceNumber;
            IsCMR = isCMR;
            IsFVP = isCMR;
            SetCreationDate(creationDate);
            PathToFolder = pathToFolder;
        }


        public void SetOwner2Name(string owner, string authUsername=null)
        {
            if(Owner2Name == owner) return;
            if(Owner1Name == owner) throw new DomainException(ErrorCodes.UserIsAleardyFirstOwner);
            if(Owner2Name == authUsername) throw new DomainException(ErrorCodes.UserCannotChangeHisOwnOwnership);
            Owner2Name = SetOwner(owner);
        }

        private void SetCreationDate(DateTime date)
        {
            if (date == new DateTime()) CreationDate = DateTime.UtcNow;
            else CreationDate = date;
        }
        public void SetNumber(int number)
        {
            if (number <= 0 || number>=10000) throw new DomainException(ErrorCodes.IvalidId);
            Number = number;
        }

        public void SetClientCountry(string country)
        {
            ClientCountry = SetCountry(country);
        }

        public void SetBrokerCountry(string country)
        {
            BrokerCountry = SetCountry(country);
        }

        private string SetCountry(string country)
        {
            if (country == null)
                throw new DomainException(ErrorCodes.InvalidCountry);
            if (country.Length != 2)
                throw new DomainException(ErrorCodes.InvalidCountry);
            if (!Regex.IsMatch(country, DomainRegex.CountryRegex))
                throw new DomainException(ErrorCodes.InvalidCountry);
            return country.ToUpper();
        }

        public void SetClientIdentificationNumber(string number)
        {
            ClientIdentificationNumber = SetIdentificationNumber(number);
        }

        public void SetBrokerIdentificationNumber(string number)
        {
            BrokerIdentificationNumber = SetIdentificationNumber(number);
        }

        private string SetIdentificationNumber(string number)
        {
            if (number == null) throw new DomainException(ErrorCodes.InvalidNIP);
            return number;
        }

        private string SetOwner(string owner)
        {
            if (owner == null)
            {
                return null;
            }
            if (owner.Length != 4)
                throw new DomainException(ErrorCodes.InvalidUsername);
            return owner.ToLower();
        }
    }
}
