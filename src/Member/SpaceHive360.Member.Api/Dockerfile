FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/Member/SpaceHive360.Member.Api/SpaceHive360.Member.Api.csproj", "src/Member/SpaceHive360.Member.Api/"]
COPY ["src/Member/SpaceHive360.Member.Application/SpaceHive360.Member.Application.csproj", "src/Member/SpaceHive360.Member.Application/"]
COPY ["src/Member/SpaceHive360.Member.Domain/SpaceHive360.Member.Domain.csproj", "src/Member/SpaceHive360.Member.Domain/"]
COPY ["src/Member/SpaceHive360.Member.Infrastructure/SpaceHive360.Member.Infrastructure.csproj", "src/Member/SpaceHive360.Member.Infrastructure/"]

RUN dotnet restore "src/Member/SpaceHive360.Member.Api/SpaceHive360.Member.Api.csproj"

COPY . .

WORKDIR "/src/src/Member/SpaceHive360.Member.Api"
RUN dotnet publish "SpaceHive360.Member.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SpaceHive360.Member.Api.dll"]
