using System;
using Microsoft.AspNetCore.Http;

namespace DocumentExplorer.Infrastructure.Commands.Files
{
    public class UploadFile : ICommand
    {
        public Guid Id { get; set; }
        public IFormFile File { get; set; }
    }
}