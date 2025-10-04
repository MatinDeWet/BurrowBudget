# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Packages.props", "."]
COPY ["Directory.Build.props", "."]
COPY ["Src/Services/WebApi/WebApi.Presentation/WebApi.Presentation.csproj", "Src/Services/WebApi/WebApi.Presentation/"]
COPY ["Src/Utilities/Identification/Identification.Core/Identification.Core.csproj", "Src/Utilities/Identification/Identification.Core/"]
COPY ["Src/Utilities/Identification/Identification.Base/Identification.Base.csproj", "Src/Utilities/Identification/Identification.Base/"]
COPY ["Src/Services/WebApi/WebApi.Infrastructure/WebApi.Infrastructure.csproj", "Src/Services/WebApi/WebApi.Infrastructure/"]
COPY ["Src/Services/Core/Infrastructure.Core/Infrastructure.Core.csproj", "Src/Services/Core/Infrastructure.Core/"]
COPY ["Src/Utilities/Repository/Repository.Core/Repository.Core.csproj", "Src/Utilities/Repository/Repository.Core/"]
COPY ["Src/Utilities/Repository/Repository.Base/Repository.Base.csproj", "Src/Utilities/Repository/Repository.Base/"]
COPY ["Src/Services/Core/Domain.Core/Domain.Core.csproj", "Src/Services/Core/Domain.Core/"]
COPY ["Src/Utilities/Domain.Base/Domain.Base.csproj", "Src/Utilities/Domain.Base/"]
COPY ["Src/Services/WebApi/WebApi.Application/WebApi.Application.csproj", "Src/Services/WebApi/WebApi.Application/"]
COPY ["Src/Utilities/CQRS/CQRS.csproj", "Src/Utilities/CQRS/"]
COPY ["Src/Utilities/Pagination/Pagination.csproj", "Src/Utilities/Pagination/"]
COPY ["Src/Utilities/Searchable/Searchable.Core/Searchable.Core.csproj", "Src/Utilities/Searchable/Searchable.Core/"]
COPY ["Src/Utilities/Searchable/Searchable.Domain/Searchable.Domain.csproj", "Src/Utilities/Searchable/Searchable.Domain/"]
COPY ["Src/Services/WebApi/WebApi.Domain/WebApi.Domain.csproj", "Src/Services/WebApi/WebApi.Domain/"]
RUN dotnet restore "./Src/Services/WebApi/WebApi.Presentation/WebApi.Presentation.csproj"
COPY . .
WORKDIR "/src/Src/Services/WebApi/WebApi.Presentation"
RUN dotnet build "./WebApi.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./WebApi.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebApi.Presentation.dll"]