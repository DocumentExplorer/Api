using DocumentExplorer.Core.Domain;
using DocumentExplorer.Core.Repositories;
using DocumentExplorer.Infrastructure.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DocumentExplorer.Infrastructure.Extensions
{
    public static class RepositoryExtensions
    {
        public static async Task<User> GetOrFailAsync(this IUserRepository userRepository, string username)
        {
            var user = await userRepository.GetAsync(username);
            if (user == null)
            {
                throw new ServiceException(Exceptions.ErrorCodes.UserNotFound);
            }
            return user;
        }

        public static async Task<User> GetOrFailAsync(this IUserRepository userRepository, Guid id)
        {
            var user = await userRepository.GetAsync(id);
            if (user == null)
            {
                throw new ServiceException(Exceptions.ErrorCodes.UserNotFound);
            }
            return user;
        }

        public static async Task<Order> GetOrFailAsync(this IOrderRepository orderRepository, Guid id)
        {
            var order = await orderRepository.GetAsync(id);
            if(order == null)
            {
                throw new ServiceException(Exceptions.ErrorCodes.OrderNotFound);
            }
            return order;
        }

        public static async Task<File> GetOrFailAsync(this IFileRepository fileRepository, Guid id)
        {
            var file = await fileRepository.GetAsync(id);
            if(file == null)
            {
                throw new ServiceException(Exceptions.ErrorCodes.FileNotFound);
            }
            return file;
        }
    }
}
