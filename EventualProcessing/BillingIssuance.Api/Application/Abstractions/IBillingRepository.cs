﻿using BillingIssuance.Api.Domain.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BillingIssuance.Api.Application.Abstractions
{
    public interface IBillingRepository
    {
        Task InsertAsync(Billing entity, CancellationToken token);
        Task<List<Billing>> GetManyAsync(ulong cpf, byte month, ushort year, CancellationToken token);
    }
}