﻿namespace NineERP.Application.Dtos.Mail
{
    public class MailMultiRequest
    {
        public List<string> To { get; set; } = default!;
        public string Subject { get; set; } = default!;
        public string Body { get; set; } = default!;
        public string From { get; set; } = default!;
    }
}