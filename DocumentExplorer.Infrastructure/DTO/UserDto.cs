using System;

namespace DocumentExplorer.Infrastructure.DTO
{
    public class UserDto
    {
        public Guid Id {get; set;}
        public string Username {get; set;}
        public string Role {get; set; }
    }
}