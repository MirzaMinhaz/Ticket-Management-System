// TMS.Application/Services/RouteService.cs
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMS.Application.DTOs.Route;
using TMS.Application.Interfaces.Persistence;
using TMS.Application.Interfaces.Services;
using TMS.Domain.Entities;

namespace TMS.Application.Services
{
    public class RouteService : IRouteService
    {
        private readonly IRouteRepository _routeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RouteService(IRouteRepository routeRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _routeRepository = routeRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RouteDto>> GetAllRoutesAsync()
        {
            var routes = await _routeRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<RouteDto>>(routes);
        }

        public async Task<RouteDto> GetRouteByIdAsync(int id)
        {
            var route = await _routeRepository.GetByIdAsync(id);
            return _mapper.Map<RouteDto>(route);
        }

        public async Task<RouteDto> GetRouteByCodeAsync(string routeCode)
        {
            var route = await _routeRepository.GetByCodeAsync(routeCode);
            return _mapper.Map<RouteDto>(route);
        }

        public async Task<RouteDto> CreateRouteAsync(CreateRouteDto createDto)
        {
            try
            {
                var existingRoutes = await _routeRepository
                    .GetRoutesByLocationAsync(createDto.DepartureLocationCode, createDto.DestinationLocationCode);

                if (existingRoutes.Any())
                {
                    throw new ApplicationException("A route with the same departure and destination already exists.");
                }

                var route = new Route
                {
                    DepartureLocationCode = createDto.DepartureLocationCode,
                    DestinationLocationCode = createDto.DestinationLocationCode,
                    RouteName = createDto.RouteName,
                    EstimatedDurationHours = createDto.EstimatedDurationHours,
                    RouteCode = await GenerateUniqueRouteCode(),
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "SystemUser",
                    LastModifiedAt = DateTime.UtcNow,
                    LastModifiedBy = "SystemUser"
                };

                await _routeRepository.AddAsync(route);
                await _unitOfWork.CompleteAsync();

                return new RouteDto
                {
                    Id = route.Id,
                    DepartureLocationCode = route.DepartureLocationCode,
                    DestinationLocationCode = route.DestinationLocationCode,
                    RouteName = route.RouteName,
                    EstimatedDurationHours = route.EstimatedDurationHours,
                    RouteCode = route.RouteCode,
                    CreatedAt = route.CreatedAt,
                    CreatedBy = route.CreatedBy,
                    LastModifiedAt = route.LastModifiedAt,
                    LastModifiedBy = route.LastModifiedBy
                };
            }
            catch (ApplicationException) // ✅ Let duplicate message bubble up
            {
                throw;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[CreateRouteAsync] Error: {ex.Message}");
                throw new ApplicationException("Unexpected error occurred while creating route.", ex);
            }
        }


        public async Task UpdateRouteAsync(int id, UpdateRouteDto updateDto)
        {
            var existingRoute = await _routeRepository.GetByIdAsync(id);
            if (existingRoute == null)
            {
                throw new Exception($"Route with ID {id} not found.");
            }

            _mapper.Map(updateDto, existingRoute);

            existingRoute.LastModifiedAt = DateTime.UtcNow;
            existingRoute.LastModifiedBy = "SystemUser";

            _routeRepository.Update(existingRoute);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteRouteAsync(int id)
        {
            var existingRoute = await _routeRepository.GetByIdAsync(id);
            if (existingRoute == null)
            {
                throw new Exception($"Route with ID {id} not found.");
            }

            await _routeRepository.DeleteAsync(existingRoute);
            await _unitOfWork.CompleteAsync();
        }

        private async Task<string> GenerateUniqueRouteCode()
        {
            var allRoutes = await _routeRepository.GetAllAsync();
            string lastCode = allRoutes
                                .Select(r => r.RouteCode)
                                .Where(code => code != null && code.StartsWith("RTE-"))
                                .OrderByDescending(code => code)
                                .FirstOrDefault();

            int nextNumber = 1;
            if (!string.IsNullOrEmpty(lastCode))
            {
                int lastHyphenIndex = lastCode.LastIndexOf('-');
                if (lastHyphenIndex != -1 && lastCode.Length > lastHyphenIndex + 1)
                {
                    string numericPart = lastCode.Substring(lastHyphenIndex + 1);
                    if (int.TryParse(numericPart, out int lastNumber))
                    {
                        nextNumber = lastNumber + 1;
                    }
                }
            }
            return $"RTE-{nextNumber:D4}"; // Formats as RTE-0001, RTE-0002, etc.
        }
    }
}
