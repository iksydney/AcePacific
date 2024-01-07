#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["AcePacific/AcePacific.API.csproj", "AcePacific/"]
COPY ["AcePacific.Busines/AcePacific.Busines.csproj", "AcePacific.Busines/"]
COPY ["AcePacific.Data/AcePacific.Data.csproj", "AcePacific.Data/"]
COPY ["AcePacific.Common/AcePacific.Common.csproj", "AcePacific.Common/"]
RUN dotnet restore "AcePacific/AcePacific.API.csproj"
COPY ["AcePacific/appsettings.json", "/app/"]
COPY . .
WORKDIR "/src/AcePacific"
RUN dotnet build "AcePacific.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AcePacific.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AcePacific.API.dll"]