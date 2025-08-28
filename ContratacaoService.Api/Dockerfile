FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src


COPY ["InsurancePlatform.sln", "."]
COPY ["ContratacaoService.Api/ContratacaoService.Api.csproj", "ContratacaoService.Api/"]
COPY ["ContratacaoService.Application/ContratacaoService.Application.csproj", "ContratacaoService.Application/"]
COPY ["ContratacaoService.Domain/ContratacaoService.Domain.csproj", "ContratacaoService.Domain/"]
COPY ["ContratacaoService.Infrastructure/ContratacaoService.Infrastructure.csproj", "ContratacaoService.Infrastructure/"]
COPY ["InsurancePlatform.Contracts/InsurancePlatform.Contracts.csproj", "InsurancePlatform.Contracts/"]
COPY ["InsurancePlatform.Shared/InsurancePlatform.Shared.csproj", "InsurancePlatform.Shared/"]

RUN dotnet restore "ContratacaoService.Api/ContratacaoService.Api.csproj"

COPY . .

WORKDIR "/src/ContratacaoService.Api"
RUN dotnet publish "ContratacaoService.Api.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ContratacaoService.Api.dll"]