using AutoMapper;
using EMD.BLL.DTOs;
using EMD.EF.DTOs;
using EMD.EF.Models;

namespace EMD.BLL
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Employee, EmployeeDTO>();
            CreateMap<Session, SesstionDTO>();
        }
    }
}
