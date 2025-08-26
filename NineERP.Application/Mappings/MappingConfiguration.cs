using AutoMapper;
using NineERP.Application.Dtos.Role;
using NineERP.Application.Dtos.User;
using NineERP.Domain.Entities.Identity;
using NineERP.Application.Helpers;
using NineERP.Domain.Entities.Dat;
using NineERP.Application.Dtos.Employees;
using NineERP.Domain.Entities.Mst;
using NineERP.Application.Dtos.MstDepartment;
using NineERP.Domain.Entities.Kbn;
using NineERP.Application.Dtos.KbnContractType;
using NineERP.Application.Dtos.KbnEmployeeStatus;
using NineERP.Application.Dtos.KbnLeaveType;
using NineERP.Application.Dtos.KbnHolidayType;
using NineERP.Application.Dtos.KbnDeviceType;
using NineERP.Application.Dtos.MstProgrammingLanguage;
using NineERP.Application.Dtos.MstTeam;
using NineERP.Application.Dtos.MstPosition;
using NineERP.Application.Dtos.DatCustomer;
using NineERP.Application.Dtos.MstLocation;
using NineERP.Application.Dtos.MstShift;

namespace NineERP.Application.Mappings
{
    public class MappingConfiguration : Profile
    {
        public MappingConfiguration()
        {
            #region System
            CreateMap<DateTime, string>().ConvertUsing<DateTimeToStringConverter>();
            CreateMap<string, DateTime>().ConvertUsing<StringToDateTimeConverter>();
            #endregion

            #region Identity
            CreateMap<AppUser, UserDetailDto>().ReverseMap();
            CreateMap<AppRole, RoleDetailDto>().ReverseMap();
            CreateMap<AppRole, RoleResponse>().ReverseMap();
            #endregion

            #region Entity
            CreateMap<DatEmployee, EmployeeDetailDto>().ReverseMap();
            CreateMap<DatEmployee, EmployeeGeneralDto>().ReverseMap();
            CreateMap<DatCustomer, DatCustomerDto>().ReverseMap();

            #endregion
            #region Mst
            CreateMap<MstDepartment, DepartmentDetailDto>().ReverseMap();
            CreateMap<MstProgrammingLanguage, MstProgrammingLanguageDto>().ReverseMap();
            CreateMap<MstTeam, MstTeamDto>().ReverseMap();
            CreateMap<MstPosition, PositionDetailDto>().ReverseMap();
            CreateMap<MstCountry, MstCountryDto>().ReverseMap();
            CreateMap<MstProvinces, MstProvincesDto>().ReverseMap();
            CreateMap<MstShift, MstShiftDto>().ReverseMap();
            #endregion

            #region Kbn
            CreateMap<KbnContractType, KbnContractTypeDto>().ReverseMap();
            CreateMap<KbnEmployeeStatus, KbnEmployeeStatusDto>().ReverseMap();
            CreateMap<KbnLeaveType, KbnLeaveTypeDto>().ReverseMap();
            CreateMap<KbnHolidayType, KbnHolidayTypeDto>().ReverseMap();
            CreateMap<KbnDeviceType, KbnDeviceTypeDto>().ReverseMap();
            #endregion
        }
    }
}
