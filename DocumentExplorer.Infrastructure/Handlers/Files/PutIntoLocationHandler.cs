using System.Threading.Tasks;
using DocumentExplorer.Infrastructure.Commands;
using DocumentExplorer.Infrastructure.Commands.Files;
using DocumentExplorer.Infrastructure.Exceptions;
using DocumentExplorer.Infrastructure.Services;

namespace DocumentExplorer.Infrastructure.Handlers.Files
{
    public class PutIntoLocationHandler : ICommandHandler<PutIntoLocation>
    {
        private readonly IFileService _fileService;
        private readonly IHandler _handler;
        public PutIntoLocationHandler(IFileService fileService, IHandler handler)
        {
            _fileService = fileService;
            _handler = handler;
        }
        public async Task HandleAsync(PutIntoLocation command)
            => await _handler
            .Run(async () => await _fileService.PutIntoLocationAsync(command.UploadId, command.OrderId, command.FileType, command.InvoiceNumber))
            .OnCustomError(x => throw new ServiceException(x.Code), true)
            .ExecuteAsync();
    }
}