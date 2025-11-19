# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar la solución y todos los .csproj
COPY *.sln ./
COPY **/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p ${file%.*}/ && mv $file ${file%.*}/; done

# Restaurar dependencias
RUN dotnet restore

# Copiar todo el código
COPY . ./

# Publicar el proyecto principal (ajusta el nombre)
RUN dotnet publish ./Lab10-Lazarinos/Lab10-Lazarinos.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:${PORT}
EXPOSE ${PORT}

ENTRYPOINT ["dotnet", "Lab10-Lazarinos.dll"]