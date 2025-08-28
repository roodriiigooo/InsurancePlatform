FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["InsurancePlatform.sln", "."]
COPY ["PropostaService.Api/PropostaService.Api.csproj", "PropostaService.Api/"]
COPY ["PropostaService.Application/PropostaService.Application.csproj", "PropostaService.Application/"]
COPY ["PropostaService.Domain/PropostaService.Domain.csproj", "PropostaService.Domain/"]
COPY ["PropostaService.Infrastructure/PropostaService.Infrastructure.csproj", "PropostaService.Infrastructure/"]
COPY ["InsurancePlatform.Contracts/InsurancePlatform.Contracts.csproj", "InsurancePlatform.Contracts/"]
COPY ["InsurancePlatform.Shared/InsurancePlatform.Shared.csproj", "InsurancePlatform.Shared/"]

RUN dotnet restore "PropostaService.Api/PropostaService.Api.csproj"

COPY . .

WORKDIR "/src/PropostaService.Api"
RUN dotnet publish "PropostaService.Api.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PropostaService.Api.dll"]