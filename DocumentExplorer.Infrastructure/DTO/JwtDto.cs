namespace DocumentExplorer.Infrastructure.DTO
{
    public class JwtDto
    {
        public string Token {get; set;}
        public long Expiry {get; set;}
        public string Role {get; set;}
    }
}