using System;

namespace DocumentExplorer.Core.Domain
{
    public class User
    {
        public Guid Id { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public string Salt { get; private set; }
        public string Role { get; private set; }
        
        protected User()
        {
        } 

        public User(string username, string hash, string salt, string role)
        {
            Id = Guid.NewGuid();
            SetUsername(username);
            SetPassword(hash, salt);
            SetRole(role);
        }

        public void SetPassword(string hash, string salt)
        {
            Password = hash;
            Salt = salt;
        }

        public void SetRole(string role)
        {
            if(role != Roles.User && Roles.Admin != role && Roles.Complementer != role)
            {
                throw new DomainException(ErrorCodes.InvalidRole);
            }
            Role = role;
        }
        private void SetUsername(string username)
        {
            if(username == null)
            {
                throw new DomainException(ErrorCodes.InvalidUsername);
            }
            if(username.Length!=4)
            {
                throw new DomainException(ErrorCodes.InvalidUsername);
            }
            Username = username;
        }


    }
}