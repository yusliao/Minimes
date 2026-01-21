using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Minimes.Application.DTOs.Customer;
using Minimes.Application.DTOs.Product;
using Minimes.Application.DTOs.User;
using Minimes.Application.Interfaces;
using Minimes.Application.Mappings;
using Minimes.Application.Services;
using Minimes.Application.Validators.Customer;
using Minimes.Application.Validators.Product;
using Minimes.Application.Validators.User;

namespace Minimes.Application;

/// <summary>
/// 应用层依赖注入扩展
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// 注册应用层服务
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // 注册AutoMapper
        services.AddAutoMapper(typeof(MappingProfile).Assembly);

        // 注册验证器 - 客户
        services.AddScoped<IValidator<CreateCustomerRequest>, CreateCustomerRequestValidator>();
        services.AddScoped<IValidator<UpdateCustomerRequest>, UpdateCustomerRequestValidator>();

        // 注册验证器 - 商品
        services.AddScoped<IValidator<CreateProductRequest>, CreateProductRequestValidator>();
        services.AddScoped<IValidator<UpdateProductRequest>, UpdateProductRequestValidator>();

        // 注册验证器 - 用户
        services.AddScoped<IValidator<RegisterRequest>, RegisterRequestValidator>();
        services.AddScoped<IValidator<LoginRequest>, LoginRequestValidator>();
        services.AddScoped<IValidator<ChangePasswordRequest>, ChangePasswordRequestValidator>();

        // 注册服务 - 业务逻辑
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IWeighingRecordService, WeighingRecordService>();
        services.AddScoped<IReportService, ReportService>();

        return services;
    }
}
