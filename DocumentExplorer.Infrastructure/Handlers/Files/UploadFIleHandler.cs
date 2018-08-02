using System.Threading.Tasks;
using DocumentExplorer.Infrastructure.Commands;
using DocumentExplorer.Infrastructure.Commands.Files;
using DocumentExplorer.Infrastructure.Exceptions;
using DocumentExplorer.Infrastructure.Services;

namespace DocumentExplorer.Infrastructure.Handlers.Files
{
    public class UploadFileHandler : ICommandHandler<UploadFile>
    {
        private readonly IHandler _handler;
        private readonly IFileService _fileService;
        public UploadFileHandler(IHandler handler, IFileService fileService)
        {
            _handler = handler;
            _fileService = fileService;
        }
        public async Task HandleAsync(UploadFile command)
            => await _handler
            .Validate(async () =>
            {
                _fileService.Validate(command.File);
                await Task.CompletedTask;
            })
            .Run(async () => await _fileService.UploadAsync(command.File,command.Id))
            .OnCustomError(x => throw new ServiceException(x.Code), true)
            .ExecuteAsync();
    }
}