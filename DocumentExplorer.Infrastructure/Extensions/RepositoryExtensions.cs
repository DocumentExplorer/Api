﻿using DocumentExplorer.Core.Domain;
using DocumentExplorer.Core.Repositories;
using DocumentExplorer.Infrastructure.Exceptions;
using System;
using System.IO;
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

        public static async Task<MemoryStream> GetOrFailAsync(this IRealFileRepository fileRepository, string path)
        {
            var file = await fileRepository.GetAsync(path);
            if(file == null)
            {
                throw new ServiceException(Exceptions.ErrorCodes.FileNotFound);
            }
            return file;
        }

        public static async Task<Log> GetOrFailAsync(this ILogRepository logRepository, Guid id)
        {
            var log = await logRepository.GetAsync(id);
            if(log == null)
            {
                throw new ServiceException(Exceptions.ErrorCodes.LogNotFound);
            }
            return log;
        }
    }
}
