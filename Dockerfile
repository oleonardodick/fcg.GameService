# BUILD STAGE
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src

# restore
COPY *.sln .
COPY src/fcg.GameService.API/fcg.GameService.API.csproj src/fcg.GameService.API/
COPY src/fcg.GameService.Application/fcg.GameService.Application.csproj src/fcg.GameService.Application/
COPY src/fcg.GameService.Domain/fcg.GameService.Domain.csproj src/fcg.GameService.Domain/
COPY src/fcg.GameService.Infrastructure/fcg.GameService.Infrastructure.csproj src/fcg.GameService.Infrastructure/
COPY src/fcg.GameService.Presentation/fcg.GameService.Presentation.csproj src/fcg.GameService.Presentation/ 
RUN dotnet restore src/fcg.GameService.API/fcg.GameService.API.csproj

#Copia o restante do código
COPY . .

# Realiza o publish em modo release
RUN dotnet publish src/fcg.GameService.API/fcg.GameService.API.csproj -c Release -o /app/publish

# RUNTIME STAGE
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copia os arquivos publicados da etapa de build
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "fcg.GameService.API.dll"]