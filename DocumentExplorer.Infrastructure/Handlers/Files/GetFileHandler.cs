using System;
using System.Threading.Tasks;
using DocumentExplorer.Infrastructure.Commands;
using DocumentExplorer.Infrastructure.Commands.Files;
using DocumentExplorer.Infrastructure.Services;
using Microsoft.Extensions.Caching.Memory;

namespace DocumentExplorer.Infrastructure.Handlers.Files
{
    public class GetFileHandler : ICommandHandler<GetFile>
    {
        private readonly IHandler _handler;
        private readonly IOrderService _orderService;
        private readonly IFileService _fileService;
        private readonly IMemoryCache _cache;

        public GetFileHandler(IHandler handler, IOrderService orderService, IFileService fileService,
            IMemoryCache cache)
        {
            _handler = handler;
            _orderService = orderService;
            _fileService = fileService;
            _cache = cache;
        }
        public async Task HandleAsync(GetFile command)
            => await _handler
            .Validate(async () => 
            {
                var file = await _fileService.GetFileAsync(command.FileId);
                await _orderService.ValidatePermissionsToOrder(command.Username, 
                    command.Role, file.OrderId);
            })
            .Run(async ()=>
            {
                var stream = await _fileService.GetFileStreamAsync(command.FileId);
                _cache.Set(command.CacheId, stream, TimeSpan.FromSeconds(5));
            })
            .ExecuteAsync();
    }
}