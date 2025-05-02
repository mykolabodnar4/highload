# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.sln .
COPY "LoggingService.csproj" .
RUN dotnet restore "LoggingService.csproj"

# copy everything else and build app
COPY . .
WORKDIR /source
RUN dotnet publish -c release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 80
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "LoggingService.dll"]
