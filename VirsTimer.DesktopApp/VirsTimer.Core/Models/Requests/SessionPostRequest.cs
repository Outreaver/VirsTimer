﻿using System.Text.Json.Serialization;

namespace VirsTimer.Core.Models.Requests
{
    internal class SessionPostRequest
    {
        public string UserId { get; init; } = string.Empty;

        public string EventId { get; init; } = string.Empty;

        public string Name { get; init; } = string.Empty;

        public SessionPostRequest(string userId, Session session)
        {
            UserId = userId;
            EventId = session.Event.Id;
            Name = session.Name;
        }
    }

    internal class SessionPostResponse
    {
        public string Id { get; init; } = string.Empty;
        public string UserId { get; init; } = string.Empty;
        public string EventId { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
    }
}
