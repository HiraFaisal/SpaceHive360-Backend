FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/Admin/SpaceHive360.Admin.Api/SpaceHive360.Admin.Api.csproj", "src/Admin/SpaceHive360.Admin.Api/"]
COPY ["src/Admin/SpaceHive360.Admin.Application/SpaceHive360.Admin.Application.csproj", "src/Admin/SpaceHive360.Admin.Application/"]
COPY ["src/Admin/SpaceHive360.Admin.Domain/SpaceHive360.Admin.Domain.csproj", "src/Admin/SpaceHive360.Admin.Domain/"]
COPY ["src/Admin/SpaceHive360.Admin.Infrastructure/SpaceHive360.Admin.Infrastructure.csproj", "src/Admin/SpaceHive360.Admin.Infrastructure/"]

RUN dotnet restore "src/Admin/SpaceHive360.Admin.Api/SpaceHive360.Admin.Api.csproj"

COPY . .

WORKDIR "/src/src/Admin/SpaceHive360.Admin.Api"
RUN dotnet publish "SpaceHive360.Admin.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SpaceHive360.Admin.Api.dll"]
