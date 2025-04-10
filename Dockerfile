FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["FitnessApp.Api/FitnessApp.Api.csproj", "FitnessApp.Api/"]
COPY ["FitnessApp.BLL/FitnessApp.BLL.csproj", "FitnessApp.BLL/"]
COPY ["FitnessApp.Core/FitnessApp.Core.csproj", "FitnessApp.Core/"]
COPY ["FitnessApp.DAL/FitnessApp.DAL.csproj", "FitnessApp.DAL/"]
RUN dotnet restore "FitnessApp.Api/FitnessApp.Api.csproj"

# Copy all source code and build
COPY . .
RUN dotnet build "FitnessApp.Api/FitnessApp.Api.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "FitnessApp.Api/FitnessApp.Api.csproj" -c Release -o /app/publish

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FitnessApp.Api.dll"]