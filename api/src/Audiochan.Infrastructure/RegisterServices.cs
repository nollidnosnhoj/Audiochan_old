﻿using Amazon.S3;
using Audiochan.Common.Services;
using Audiochan.Core.Persistence;
using Audiochan.Core.Persistence.Repositories;
using Audiochan.Core.Services;
using Audiochan.Infrastructure.Persistence;
using Audiochan.Infrastructure.Persistence.Repositories;
using Audiochan.Infrastructure.Security;
using Audiochan.Infrastructure.Shared;
using Audiochan.Infrastructure.Storage.AmazonS3;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Audiochan.Infrastructure
{
    public static class RegisterServices
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,
            IConfiguration configuration,
            IHostEnvironment environment)
        {
            services.AddCaching(configuration, environment);
            services.AddStorage();
            services.AddTransient<IDateTimeProvider, DateTimeProvider>();
            services.AddTransient<IRandomIdGenerator, NanoidGenerator>();
            services.AddTransient<IPasswordHasher, BCryptHasher>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            return services;
        }

        private static IServiceCollection AddCaching(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
            if (!environment.IsProduction())
            {
                services.AddDistributedMemoryCache();
            }
            else
            {
                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = configuration.GetConnectionString("Redis");
                    options.InstanceName = "audiochan_redis";
                });
            }

            return services;
        }

        private static IServiceCollection AddStorage(this IServiceCollection services)
        {
            services.AddAWSService<IAmazonS3>();
            services.AddTransient<IStorageService, AmazonS3Service>();
            return services;
        }
    }
}