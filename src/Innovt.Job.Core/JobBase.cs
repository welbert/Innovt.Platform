﻿// INNOVT TECNOLOGIA 2014-2021
// Author: Michel Magalhães
// Project: Innovt.Job.Core
// Solution: Innovt.Platform
// Date: 2021-06-02
// Contact: michel@innovt.com.br or michelmob@gmail.com

using System;
using System.Threading;
using System.Threading.Tasks;
using Innovt.Core.CrossCutting.Log;
using Timer = System.Timers.Timer;

namespace Innovt.Job.Core
{
    public abstract class JobBase
    {
        private readonly Timer heartBeat;
        protected readonly ILogger Logger;

        protected JobBase(string jobName, ILogger logger, double heartBeatIntervalInMiliSeconds)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));

            Name = jobName;
            heartBeat = new Timer(heartBeatIntervalInMiliSeconds)
            {
                Enabled = true,
                AutoReset = true
            };

            heartBeat.Elapsed += (sender, args) => OnHeartBeat();
        }

        public string Name { get; set; }

        protected virtual void OnHeartBeat()
        {
            Logger.Info($"{Name}.HeartBeat");
        }

        public Task Start()
        {
            Logger.Info($"Job [{Name}] starting at {DateTimeOffset.Now}");
            return OnStart();
        }

        public Task Stop()
        {
            Logger.Info($"Job [{Name}] stopping at {DateTimeOffset.Now}");

            return OnStop();
        }

        protected abstract Task OnStart(CancellationToken cancellationToken = default);
        protected abstract Task OnStop(CancellationToken cancellationToken = default);
    }
}