FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["InsurancePlatform.sln", "."]
COPY ["InsurancePlatform.WebApp/InsurancePlatform.WebApp.csproj", "InsurancePlatform.WebApp/"]

RUN dotnet restore "InsurancePlatform.WebApp/InsurancePlatform.WebApp.csproj"

COPY . .

WORKDIR "/src/InsurancePlatform.WebApp"
RUN dotnet publish "InsurancePlatform.WebApp.csproj" -c Release -o /app/publish

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
COPY --from=build /app/publish/wwwroot .
COPY InsurancePlatform.WebApp/nginx.conf /etc/nginx/conf.d/default.conf

RUN apk add --no-cache dos2unix && \
    dos2unix /etc/nginx/conf.d/default.conf
