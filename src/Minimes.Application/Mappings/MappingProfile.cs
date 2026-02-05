using AutoMapper;
using Minimes.Application.DTOs.Employee;
using Minimes.Application.DTOs.Product;
using Minimes.Application.DTOs.WeighingRecord;
using Minimes.Domain.Entities;

namespace Minimes.Application.Mappings;

/// <summary>
/// AutoMapper映射配置
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Product映射
        CreateMap<Product, ProductResponse>();
        CreateMap<CreateProductRequest, Product>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
        CreateMap<UpdateProductRequest, Product>()
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

        // Employee映射
        CreateMap<Employee, EmployeeResponse>();
        CreateMap<CreateEmployeeRequest, Employee>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
        CreateMap<UpdateEmployeeRequest, Employee>()
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

        // WeighingRecord映射 - 简化版
        CreateMap<CreateWeighingRecordRequest, WeighingRecord>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

        CreateMap<WeighingRecord, WeighingRecordResponse>();
    }
}
