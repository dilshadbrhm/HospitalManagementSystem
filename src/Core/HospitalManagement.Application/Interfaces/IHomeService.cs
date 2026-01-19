using HospitalManagement.Application.Dtos;

namespace HospitalManagement.Application.Interfaces
{
    public interface IHomeService
    {
        Task<HomeDto> GetHomeDataAsync();
    }
}
