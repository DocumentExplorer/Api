﻿using System;
using System.Threading.Tasks;

namespace DocumentExplorer.Infrastructure.Services
{
    public interface IHandler
    {
        IHandlerTask Run(Func<Task> run);
        IHandlerTaskRunner Validate(Func<Task> validate);
        Task ExecuteAllAsync();
    }
}
