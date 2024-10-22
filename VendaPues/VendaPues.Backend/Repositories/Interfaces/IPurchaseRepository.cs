﻿using VendaPues.Shared.DTOs;
using VendaPues.Shared.Entities;
using VendaPues.Shared.Responses;

namespace VendaPues.Backend.Repositories.Interfaces
{
    public interface IPurchaseRepository
    {
        Task<ActionResponse<Purchase>> AddAsync(Purchase entity);

        Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination);

        Task<ActionResponse<Purchase>> GetAsync(int id);

        Task<ActionResponse<IEnumerable<Purchase>>> GetAsync(PaginationDTO pagination);
    }
}
