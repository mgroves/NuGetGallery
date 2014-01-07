﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using NuGet.Services.Client;
using NuGet.Services.Work.Models;

namespace NuGet.Services.Work.Client
{
    public class InvocationsClient
    {
        private HttpClient _client;

        public InvocationsClient(HttpClient client)
        {
            _client = client;
        }

        public Task<ServiceResponse<Invocation>> Put(InvocationRequest request)
        {
            return _client.PutAsync(
                "invocations",
                new ObjectContent<InvocationRequest>(
                    request,
                    JsonFormat.Formatter))
                .AsServiceResponse<Invocation>();
        }

        public Task<ServiceResponse<IEnumerable<Invocation>>> Get(InvocationListCriteria criteria)
        {
            return _client.GetAsync(
                "invocations/" + criteria.ToString().ToLowerInvariant())
                .AsServiceResponse<IEnumerable<Invocation>>();
        }

        public Task<ServiceResponse<Invocation>> Get(string id)
        {
            return _client.GetAsync("invocations/" + id).AsServiceResponse<Invocation>();
        }

        public Task<ServiceResponse<InvocationStatistics>> GetStatistics()
        {
            return _client.GetAsync("invocations/stats").AsServiceResponse<InvocationStatistics>();
        }
    }
}