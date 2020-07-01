using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace MarsRover.PhotoDownload.Api.Extensions
{
    public static class MarsRoverPhotoDownloadApiExtensions
    {
        public static IHostBuilder ConfigureMarsRoverPhotoDownloadApi(this IHostBuilder hostBuilder, Action<HostBuilderContext, IMarsRoverPhotoDownloadApiBuilder> configureMarsPhotoDownloadApi)
        {
            return hostBuilder.ConfigureServices((context, collection) => collection.ConfigurePhotoDownloadApi(builder => configureMarsPhotoDownloadApi(context, builder)));
        }

        public static IServiceCollection ConfigurePhotoDownloadApi(this IServiceCollection services, Action<IMarsRoverPhotoDownloadApiBuilder> configure)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddOptions();
            services.TryAddEnumerable(ServiceDescriptor.Singleton((IConfigureOptions<MarsRoverPhotoDownloadApiOptions>) new DefaultMarsRoverPhotoDownloadApiOptions(null)));
            configure(new MarsRoverPhotoDownloadApiBuilder(services));
            return services;
        }


    }
}
