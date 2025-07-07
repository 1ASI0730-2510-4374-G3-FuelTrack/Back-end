# Etapa de compilación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiar archivos del proyecto
COPY FuelTrack.Api.csproj ./
COPY *.sln ./
RUN dotnet restore

# Copiar todo el código fuente
COPY . .
RUN dotnet publish -c Release -o /out

# Imagen final
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /out .

# Arranca la app
ENTRYPOINT ["dotnet", "FuelTrack.Api.dll"]
