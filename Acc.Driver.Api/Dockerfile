#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["Acc.Driver.Api/Acc.Driver.Api.csproj", "Acc.Driver.Api/"]
RUN dotnet restore "Acc.Driver.Api/Acc.Driver.Api.csproj"
COPY . .
WORKDIR "/src/Acc.Driver.Api"
RUN dotnet build "Acc.Driver.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Acc.Driver.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Acc.Driver.Api.dll"]