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
        public DateTime CreationDate { get; set; }
        public string CreationDateString 
        {
            get
            {
                //var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
                //DateTime cstTime = TimeZoneInfo.ConvertTimeFromUtc(CreationDate, timeZoneInfo);
                return CreationDate.ToString(@"dd.MM.yyyy HH:mm:ss");
            }
        }



        public int InvoiceNumber { get; set; }
        public string PathToFolder { get; set; }
        public Guid CMRId {get; private set;}
        public Guid FVKId {get; private set;}
        public Guid FVPId {get; private set;}
        public Guid NIPId {get; private set;}
        public Guid NotaId {get; private set;}
        public Guid PPId {get; private set;}
        public Guid RKId {get; private set;}
        public Guid ZKId {get; private set;}
        public Guid ZPId {get; private set;}

        protected Order()
        {

        }

        public Order(int number, string clientCountry, string clientIdentificationNumber, 
            string brokerCountry, string brokerIdentificationNumber, string owner1Name, 
            DateTime creationDate, string pathToFolder = null, int invoiceNumber=0)
        {
            Id = Guid.NewGuid();
            SetNumber(number);
            SetClientCountry(clientCountry);
            SetClientIdentificationNumber(clientIdentificationNumber);
            SetBrokerCountry(brokerCountry);
            SetBrokerIdentificationNumber(brokerIdentificationNumber);
            Owner1Name = SetOwner(owner1Name);
            InvoiceNumber = invoiceNumber;
            SetCreationDate(creationDate);
            PathToFolder = pathToFolder;
        }

        public string GetPathToFile(string fileType)
        {
            switch(fileType)
            {
                case "cmr":
                    if(CMRId == Guid.Empty) throw new DomainException(ErrorCodes.FileDoesNotExists);
                    fileType = $"{fileType}{AddLeadingZeros(Number)}";
                    break;
                case "fvk":
                    if(FVKId == Guid.Empty) throw new DomainException(ErrorCodes.FileDoesNotExists);
                    fileType = $"{fileType}{AddLeadingZeros(InvoiceNumber)}";
                    break;
                case "fvp":
                    if(FVPId == Guid.Empty) throw new DomainException(ErrorCodes.FileDoesNotExists);
                    fileType = $"{fileType}{AddLeadingZeros(Number)}";
                    break;
                case "nip":
                    if(NIPId == Guid.Empty) throw new DomainException(ErrorCodes.FileDoesNotExists);
                    fileType = $"{fileType}{AddLeadingZeros(Number)}";
                    break;
                case "nota":
                    if(NotaId == Guid.Empty) throw new DomainException(ErrorCodes.FileDoesNotExists);
                    fileType = $"{fileType}{AddLeadingZeros(Number)}";
                    break;
                case "pp":
                    if(PPId == Guid.Empty) throw new DomainException(ErrorCodes.FileDoesNotExists);
                    fileType = $"{fileType}{AddLeadingZeros(Number)}";
                    break;
                case "rk":
                    if(RKId == Guid.Empty) throw new DomainException(ErrorCodes.FileDoesNotExists);
                    fileType = $"{fileType}{AddLeadingZeros(Number)}";
                    break;
                case "zk":
                    if(ZKId == Guid.Empty) throw new DomainException(ErrorCodes.FileDoesNotExists);
                    fileType = $"{fileType}{AddLeadingZeros(Number)}";
                    break;
                default:
                    throw new DomainException(ErrorCodes.FileTypeNotSpecified);
            }
            return $"{PathToFolder}{fileType}.pdf";
        }

        public void LinkFile(File file, string fileType, int invoiceNumber)
        {
            switch(fileType)
            {
                case "cmr":
                    CMRId = file.Id;
                    break;
                case "fvk":
                    FVKId = file.Id;
                    InvoiceNumber = invoiceNumber;
                    break;
                case "fvp":
                    FVPId = file.Id;
                    break;
                case "nip":
                    NIPId = file.Id;
                    break;
                case "nota":
                    NotaId = file.Id;
                    break;
                case "pp":
                    PPId = file.Id;
                    break;
                case "rk":
                    RKId = file.Id;
                    break;
                case "zk":
                    ZKId = file.Id;
                    break;
            }
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
