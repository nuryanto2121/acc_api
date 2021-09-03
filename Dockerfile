#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 8787

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["Acc.Api/Acc.Api.csproj", "Acc.Api/"]
COPY ["GenerateFunctionPostgres/GenerateFunctionPostgres.csproj", "GenerateFunctionPostgres/"]
RUN dotnet restore "Acc.Api/Acc.Api.csproj"
COPY . .
WORKDIR "/src/Acc.Api"
RUN dotnet build "Acc.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Acc.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Acc.Api.dll"]