using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using NodaTime;

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
                var timeZone = DateTimeZoneProviders.Tzdb["Europe/Warsaw"];
                var instant = Instant.FromDateTimeUtc(DateTime.SpecifyKind(CreationDate, DateTimeKind.Utc));
                return instant.InZone(timeZone).ToDateTimeUnspecified().ToString(@"dd.MM.yyyy HH:mm:ss");
            }
        }

        public int InvoiceNumber { get; set; }
        public IEnumerable<File> Files 
        { 
            get
            {
                return _files;
            }
            set
            {
                _files = new HashSet<File>(value);
            }
        }

        private ISet<File> _files = new HashSet<File>();
        public Guid CMRId { get => _files.SingleOrDefault(x=>x.FileType == FileTypes.CMR).Id; }
        public bool IsCMRRequired { get => _files.SingleOrDefault(x=> x.FileType == FileTypes.CMR).IsRequired;}
        public Guid FVKId { get => _files.SingleOrDefault(x=>x.FileType == FileTypes.FVK).Id; }
        public bool IsFVKRequired { get => _files.SingleOrDefault(x=> x.FileType == FileTypes.FVK).IsRequired;}
        public Guid FVPId { get => _files.SingleOrDefault(x=>x.FileType == FileTypes.FVP).Id; }
        public bool IsFVPRequired { get => _files.SingleOrDefault(x=> x.FileType == FileTypes.FVP).IsRequired;}
        public Guid NIPId { get => _files.SingleOrDefault(x=>x.FileType == FileTypes.NIP).Id; }
        public bool IsNIPRequired { get => _files.SingleOrDefault(x=> x.FileType == FileTypes.NIP).IsRequired;}
        public Guid NotaId { get => _files.SingleOrDefault(x=>x.FileType == FileTypes.Nota).Id; }
        public bool IsNotaRequired { get => _files.SingleOrDefault(x=> x.FileType == FileTypes.Nota).IsRequired;}
        public Guid PPId { get => _files.SingleOrDefault(x=>x.FileType == FileTypes.PP).Id; }
        public bool IsPPRequired { get => _files.SingleOrDefault(x=> x.FileType == FileTypes.PP).IsRequired;}
        public Guid RKId { get => _files.SingleOrDefault(x=>x.FileType == FileTypes.RK).Id; }
        public bool IsRKRequired { get => _files.SingleOrDefault(x=> x.FileType == FileTypes.RK).IsRequired;}
        public Guid ZKId { get => _files.SingleOrDefault(x=>x.FileType == FileTypes.ZK).Id; }
        public bool IsZKRequired { get => _files.SingleOrDefault(x=> x.FileType == FileTypes.ZK).IsRequired;}
        public Guid ZPId { get => _files.SingleOrDefault(x=>x.FileType == FileTypes.ZP).Id; }
        public bool IsZPRequired { get => _files.SingleOrDefault(x=> x.FileType == FileTypes.ZP).IsRequired;}

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
            SetDefaultFilesStatus();
        }

        public void SetRequirements(string fileType, bool isRequired)
        {
            var file = _files.SingleOrDefault(x => x.FileType==fileType);
            if(file==null) throw new DomainException(ErrorCodes.InvalidFileType);
            if(isRequired)
            {
                file.SetIsRequired();
            }
            else
            {
                file.SetIsNotRequired();
            }
            
        }

        public bool FileIsAlreadyAssigned(string fileType)
        {
            var file = _files.SingleOrDefault(x => x.FileType==fileType);
            if(file==null) throw new DomainException(ErrorCodes.InvalidFileType);
            return file.Path != string.Empty;
        }

        private void SetDefaultFilesStatus()
        {
            var properties = typeof(FileTypes).GetProperties();
            foreach(var property in properties)
            {
                _files.Add(new File(Id, property.Name.ToLower()));
            }
        }

        public string GetPathToFile(string fileType)
        {
            var file = _files.SingleOrDefault(x => x.FileType==fileType);
            if(file==null) throw new DomainException(ErrorCodes.InvalidFileType);
            return file.Path;
        }

        public void LinkFile(string fileType, int invoiceNumber=0)
        {
            var file = _files.SingleOrDefault(x => x.FileType==fileType);
            if(file==null) throw new DomainException(ErrorCodes.InvalidFileType);
            int numberToAdd;
            if(fileType==FileTypes.FVK)
            {
                ValidateInvoiceNumber(invoiceNumber);
                numberToAdd = invoiceNumber;
            }
            else
            {
                numberToAdd = Number;
            }

            var path = $"{GetPathToFolder()}{fileType}{AddLeadingZeros(numberToAdd)}.pdf";
            file.SetFile(path);
            if(fileType==FileTypes.FVK)
            {
                InvoiceNumber = invoiceNumber;
            }
        }

        public void UnlinkFile(string fileType)
        {
            var file = _files.SingleOrDefault(x => x.FileType==fileType);
            if(file==null) throw new DomainException(ErrorCodes.InvalidFileType);
            file.DeleteFile();
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
            if (number <= 0 || number>=10000) throw new DomainException(ErrorCodes.InvalidNumber);
            Number = number;
        }

        private void ValidateInvoiceNumber(int invoiceNumber)
        {
            if (invoiceNumber <= 0 || invoiceNumber>=10000) 
                throw new DomainException(ErrorCodes.InvalidInvoiceNumber);
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

        public int HasLackingFilesForRole(string role, Permissions permissions)
        {
            int lackingFiles = 0;
            var rolePermissions = permissions.GetPermissionsForRole(role);
            foreach(var perm in rolePermissions)
            {
                if((IsFileRequired(perm) && (!FileIsAlreadyAssigned(perm))))
                {
                    lackingFiles++;
                }
            }
            return lackingFiles;
        }

        private bool IsFileRequired(string fileType)
        {
            var file = _files.SingleOrDefault(x => x.IsRequired);
            return file.IsRequired;
        }

    }
}
