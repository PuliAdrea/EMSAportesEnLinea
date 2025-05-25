using AutoMapper;
using EMS.API.DTOs.DepartmentDTOs;
using EMS.API.DTOs.EmployeeDTOs;
using EMS.Domain.Entities;

namespace EMS.API.Helpers;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<EmployeeCreateDTO, Employee>();
        CreateMap<Employee, EmployeeResponseDTO>()
            .ForMember(dest => dest.CalculatedSalary, opt => opt.MapFrom(src => src.BaseSalary))
            .ForMember(dest => dest.Position, opt => opt.MapFrom(src => src.Position.ToString()));
        CreateMap<EmployeeUpdateDTO, Employee>()
           .ForMember(dest => dest.Department, opt => opt.Ignore());
        CreateMap<Employee, EmployeeResponseDTO>()
           .ForMember(dest => dest.CalculatedSalary, opt => opt.MapFrom(src => src.BaseSalary))
           .ForMember(dest => dest.Position, opt => opt.MapFrom(src => src.Position.ToString()))
           .ForMember(dest => dest.DepartmentId, opt => opt.MapFrom(src => src.DepartmentId));

        CreateMap<DepartmentCreateDTO, Department>();
        CreateMap<Department, DepartmentResponseDTO>();
        CreateMap<Department, DepartmentUpdateDTO>();
        CreateMap<DepartmentUpdateDTO, Department>()
            .ForMember(dest => dest.Employees, opt => opt.Ignore());

    }
}