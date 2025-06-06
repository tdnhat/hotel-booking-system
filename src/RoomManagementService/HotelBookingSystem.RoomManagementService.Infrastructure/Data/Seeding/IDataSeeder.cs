using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HotelBookingSystem.RoomManagementService.Domain.Repositories;

namespace HotelBookingSystem.RoomManagementService.Infrastructure.Data.Seeding
{
    public interface IDataSeeder
    {
        Task SeedAsync(IUnitOfWork unitOfWork, CancellationToken cancellationToken = default);
    }
}