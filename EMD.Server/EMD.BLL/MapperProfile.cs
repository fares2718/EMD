using AutoMapper;
using EMD.BLL.DTOs;
using EMD.EF.Models;
using System;
using System.Collections.Generic;
using System.Text;

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
