#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["FsPolyJobs.Api/FsPolyJobs.Api.fsproj", "FsPolyJobs.Api/"]
RUN dotnet restore "FsPolyJobs.Api/FsPolyJobs.Api.fsproj"
COPY . .
WORKDIR "/src/FsPolyJobs.Api"
RUN dotnet build "FsPolyJobs.Api.fsproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FsPolyJobs.Api.fsproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FsPolyJobs.Api.dll"]