# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.sln .
COPY "MessagingService.csproj" .
RUN dotnet restore "MessagingService.csproj"

# copy everything else and build app
COPY . .
WORKDIR /source
RUN dotnet publish -c release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
EXPOSE 80
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "MessagingService.dll"]
