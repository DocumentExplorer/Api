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
        private readonly IOrderService _orderService;
        private readonly IHandler _handler;
        private readonly IPermissionsService _permissionService;
        public PutIntoLocationHandler(IFileService fileService, IHandler handler, IOrderService orderService,
            IPermissionsService permissionService)
        {
            _fileService = fileService;
            _handler = handler;
            _orderService = orderService;
            _permissionService = permissionService;
        }
        public async Task HandleAsync(PutIntoLocation command)
            => await _handler
            .Validate(async () => 
            {
                await _orderService.ValidatePermissionsToOrder(command.Username, command.Role, command.OrderId);
                await _permissionService.Validate(command.FileType, command.Role);
            })
            .Run(async () => await _fileService.PutIntoLocationAsync(command.UploadId, 
                command.OrderId, command.FileType, command.InvoiceNumber, command.Role, command.Username))
            .OnCustomError(x => throw new ServiceException(x.Code), true)
            .ExecuteAsync();
    }
}