FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/SuperAdmin/SpaceHive360.SuperAdmin.Api/SpaceHive360.SuperAdmin.Api.csproj", "src/SuperAdmin/SpaceHive360.SuperAdmin.Api/"]
COPY ["src/SuperAdmin/SpaceHive360.SuperAdmin.Application/SpaceHive360.SuperAdmin.Application.csproj", "src/SuperAdmin/SpaceHive360.SuperAdmin.Application/"]
COPY ["src/SuperAdmin/SpaceHive360.SuperAdmin.Domain/SpaceHive360.SuperAdmin.Domain.csproj", "src/SuperAdmin/SpaceHive360.SuperAdmin.Domain/"]
COPY ["src/SuperAdmin/SpaceHive360.SuperAdmin.Infrastructure/SpaceHive360.SuperAdmin.Infrastructure.csproj", "src/SuperAdmin/SpaceHive360.SuperAdmin.Infrastructure/"]

RUN dotnet restore "src/SuperAdmin/SpaceHive360.SuperAdmin.Api/SpaceHive360.SuperAdmin.Api.csproj"

COPY . .

WORKDIR "/src/src/SuperAdmin/SpaceHive360.SuperAdmin.Api"
RUN dotnet publish "SpaceHive360.SuperAdmin.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SpaceHive360.SuperAdmin.Api.dll"]
