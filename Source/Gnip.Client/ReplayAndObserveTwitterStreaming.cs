﻿using System;
using System.Reactive.Linq;
using System.Threading.Tasks;

using Krowiorsch.Gnip.Model;
using Krowiorsch.Model;

namespace Krowiorsch.Gnip
{
    public class ReplayAndObserveTwitterStreaming : IHttpStreaming<Activity>
    {
        readonly ReplayTwitterHttpStreaming _replayStreaming;
        readonly ObserveTwitterHttpStreaming _observeActivityStreaming;

        /// <summary>
        /// Created a stream, that start at a specific point and resume the with live events
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="accessToken"></param>
        /// <param name="startDate"></param>
        public ReplayAndObserveTwitterStreaming(string endpoint, GnipAccessToken accessToken, DateTime startDate)
        {
            _replayStreaming = new ReplayTwitterHttpStreaming(endpoint, accessToken, startDate);
            _observeActivityStreaming = new ObserveTwitterHttpStreaming(endpoint, accessToken);

            Endpoint = new Uri(endpoint);
            Stream = _replayStreaming.Stream.Merge(_observeActivityStreaming.Stream);
        }

        public async Task ReadAsync()
        {
            await _replayStreaming.ReadAsync();
            await _observeActivityStreaming.ReadAsync();
        }

        public void StopStreaming()
        {
            _replayStreaming.StopStreaming();
            _observeActivityStreaming.StopStreaming();
        }

        public void Dispose()
        {
            _replayStreaming.Dispose();
            _observeActivityStreaming.Dispose();
        }

        public IObservable<Activity> Stream { get; set; }
        public Uri Endpoint { get; private set; }
    }
}