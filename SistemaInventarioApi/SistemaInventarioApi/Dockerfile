# Etapa 1: compilar y publicar
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY SistemaInventarioApi.csproj .
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /app/publish --no-restore

# Etapa 2: imagen final, solo runtime (mucho más liviana)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "SistemaInventarioApi.dll"]