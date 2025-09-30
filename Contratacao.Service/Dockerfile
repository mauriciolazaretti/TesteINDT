FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source

COPY . .
RUN dotnet restore ./Contratacao.Service.API/Contratacao.Service.API.csproj

RUN dotnet publish ./Contratacao.Service.API/Contratacao.Service.API.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080
EXPOSE 8081

ENTRYPOINT ["dotnet", "Contratacao.Service.API.dll"]