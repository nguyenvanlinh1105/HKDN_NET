﻿namespace NineERP.Application.Interfaces.Common
{
    public interface IEmailService
    {
        Task SendAsync(string to, string subject, string body);
    }
}