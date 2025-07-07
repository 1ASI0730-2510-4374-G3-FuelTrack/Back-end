# Etapa de compilación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiar archivos del proyecto
COPY FuelTrack.Api/*.csproj ./FuelTrack.Api/
COPY *.sln ./
RUN dotnet restore

# Copiar todo el código fuente
COPY . .
WORKDIR /app/FuelTrack.Api
RUN dotnet publish -c Release -o /out

# Imagen final
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /out .

# Elimina carpetas temporales por seguridad
RUN rm -rf /app/obj /app/bin

# Arranca la app
ENTRYPOINT ["dotnet", "FuelTrack.Api.dll"]
