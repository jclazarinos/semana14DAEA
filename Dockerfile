# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar archivos de proyecto y restaurar dependencias
COPY *.csproj ./
RUN dotnet restore

# Copiar el resto del código y compilar
COPY . ./
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copiar los archivos publicados
COPY --from=build /app/publish .

# Configurar puerto
ENV ASPNETCORE_URLS=http://+:$PORT
EXPOSE $PORT

# Comando de inicio (ajusta el nombre del DLL)
ENTRYPOINT ["dotnet", "Lab10-Lazarinos.dll"]