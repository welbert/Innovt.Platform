﻿// INNOVT TECNOLOGIA 2014-2021
// Author: Michel Magalhães
// Project: Innovt.Notification.Core
// Solution: Innovt.Platform
// Date: 2021-06-02
// Contact: michel@innovt.com.br or michelmob@gmail.com

using System.Threading;
using System.Threading.Tasks;
using Innovt.Notification.Core.Domain;

namespace Innovt.Notification.Core
{
    public interface INotificationHandler
    {
        Task<dynamic> SendAsync(NotificationMessage message, CancellationToken cancellationToken = default);
    }
}