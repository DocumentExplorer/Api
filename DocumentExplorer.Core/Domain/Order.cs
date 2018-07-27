using System;
using System.Reflection;
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
        public Guid CMRId {get; private set;}
        public bool IsCMRRequired { get; private set;}
        public Guid FVKId {get; private set;}
        public bool IsFVKRequired { get; private set;}
        public Guid FVPId {get; private set;}
        public bool IsFVPRequired { get; private set;}
        public Guid NIPId {get; private set;}
        public bool IsNIPRequired { get; private set;}
        public Guid NotaId {get; private set;}
        public bool IsNotaRequired { get; private set;}
        public Guid PPId {get; private set;}
        public bool IsPPRequired { get; private set;}
        public Guid RKId {get; private set;}
        public bool IsRKRequired { get; private set;}
        public Guid ZKId {get; private set;}
        public bool IsZKRequired { get; private set;}
        public Guid ZPId {get; private set;}
        public bool IsZPRequired { get; private set; }

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
            SetDefaultRequirements();
        }

        public void SetRequirements(string fileType, bool isRequired)
        {
            var properties = typeof(FileTypes).GetProperties();
            foreach(var property in properties)
            {
                if(property.Name.ToLower()==fileType)
                {
                    if((!isRequired) && FileIsAlreadyAssigned(property.Name))
                        throw new DomainException(ErrorCodes.FileIsAlreadyAssigned);
                    GetIsFileRequiredProperty(property.Name).SetValue(this, isRequired);
                    return;
                }
            }
            throw new DomainException(ErrorCodes.InvalidFileType);
        }

        private PropertyInfo GetIsFileRequiredProperty(string propertyName)
        {
            return typeof(Order).GetProperty($"Is{propertyName}Required");
        }

        private bool FileIsAlreadyAssigned(string propertyName)
        {
            var valueOfProperty = typeof(Order).GetProperty($"{propertyName}Id").GetValue(this, null);
            if(valueOfProperty is Guid fileId)
            {
                return fileId != Guid.Empty;
            }
            else throw new InvalidCastException();
        }

        private void SetDefaultRequirements()
        {
            IsCMRRequired = true;
            IsFVKRequired = true;
            IsFVPRequired = true;
            IsNIPRequired = true;
            IsNotaRequired = true;
            IsPPRequired = true;
            IsRKRequired = true;
            IsZKRequired = true;
            IsZPRequired = true;
        }

        public string GetPathToFile(string fileType)
        {
            var properties = typeof(FileTypes).GetProperties();
            foreach(var property in properties)
            {
                if(!FileIsAlreadyAssigned(property.Name))
                    throw new DomainException(ErrorCodes.FileDoesNotExists);
                int number;
                if(fileType==FileTypes.FVK) number = InvoiceNumber;
                else number = Number;
                fileType = $"{fileType}{AddLeadingZeros(Number)}";
                return $"{GetPathToFolder()}{fileType}.pdf";
            }
            throw new DomainException(ErrorCodes.InvalidFileType);
        }

        public void LinkFile(File file, string fileType, int invoiceNumber)
        {
            switch(fileType)
            {
                case "cmr":
                    if(CMRId != Guid.Empty) throw new DomainException(ErrorCodes.FileIsAlreadyAssigned);
                    if(!IsCMRRequired) throw new DomainException(ErrorCodes.FileIsNotRequired);
                    CMRId = file.Id;
                    break;
                case "fvk":
                    if(FVKId != Guid.Empty) throw new DomainException(ErrorCodes.FileIsAlreadyAssigned);
                    if(!IsFVKRequired) throw new DomainException(ErrorCodes.FileIsNotRequired);
                    FVKId = file.Id;
                    InvoiceNumber = invoiceNumber;
                    break;
                case "fvp":
                    if(FVPId != Guid.Empty) throw new DomainException(ErrorCodes.FileIsAlreadyAssigned);
                    if(!IsFVPRequired) throw new DomainException(ErrorCodes.FileIsNotRequired);
                    FVPId = file.Id;
                    break;
                case "nip":
                    if(NIPId != Guid.Empty) throw new DomainException(ErrorCodes.FileIsAlreadyAssigned);
                    if(!IsNIPRequired) throw new DomainException(ErrorCodes.FileIsNotRequired);
                    NIPId = file.Id;
                    break;
                case "nota":
                    if(NotaId != Guid.Empty) throw new DomainException(ErrorCodes.FileIsAlreadyAssigned);
                    if(!IsNotaRequired) throw new DomainException(ErrorCodes.FileIsNotRequired);
                    NotaId = file.Id;
                    break;
                case "pp":
                    if(PPId != Guid.Empty) throw new DomainException(ErrorCodes.FileIsAlreadyAssigned);
                    if(!IsPPRequired) throw new DomainException(ErrorCodes.FileIsNotRequired);
                    PPId = file.Id;
                    break;
                case "rk":
                    if(RKId != Guid.Empty) throw new DomainException(ErrorCodes.FileIsAlreadyAssigned);
                    if(!IsRKRequired) throw new DomainException(ErrorCodes.FileIsNotRequired);
                    RKId = file.Id;
                    break;
                case "zk":
                    if(ZKId != Guid.Empty) throw new DomainException(ErrorCodes.FileIsAlreadyAssigned);
                    if(!IsZKRequired) throw new DomainException(ErrorCodes.FileIsNotRequired);
                    ZKId = file.Id;
                    break;
                case "zp":
                    if(ZPId != Guid.Empty) throw new DomainException(ErrorCodes.FileIsAlreadyAssigned);
                    if(!IsZPRequired) throw new DomainException(ErrorCodes.FileIsNotRequired);
                    ZPId = file.Id;
                    break;
                default:
                    throw new DomainException(ErrorCodes.InvalidFileType);
            }
        }

        public void UnlinkFile(string fileType)
        {
            switch(fileType)
            {
                case "cmr":
                    CMRId = Guid.Empty;
                    break;
                case "fvk":
                    CMRId = Guid.Empty;
                    InvoiceNumber = 0;
                    break;
                case "fvp":
                    FVPId = Guid.Empty;
                    break;
                case "nip":
                    NIPId = Guid.Empty;
                    break;
                case "nota":
                    NotaId = Guid.Empty;
                    break;
                case "pp":
                    PPId = Guid.Empty;
                    break;
                case "rk":
                    RKId = Guid.Empty;
                    break;
                case "zk":
                    ZKId = Guid.Empty;
                    break;
                case "zp":
                    ZPId = Guid.Empty;
                    break;
                default:
                    throw new DomainException(ErrorCodes.InvalidFileType);
            }
        }

        public string GetPathToFolder()
        {
            return OrderFolderNameGenerator.OrderToName(this);
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
