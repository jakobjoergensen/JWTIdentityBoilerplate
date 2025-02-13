FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080


# This stage is used to build the project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["JWT.Api/JWT.Api.csproj", "JWT.Api/"]
RUN dotnet restore "JWT.Api/JWT.Api.csproj"
COPY . .
WORKDIR "/src/JWT.Api"
RUN dotnet build "JWT.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build


# This stage is used to publish the project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "JWT.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false


# This stage is used in production
FROM base AS final
WORKDIR /app


# Use build argument to copy the correct configuration file
ARG ENVIRONMENT=Production
COPY --from=publish /app/publish .


# Copy the appropriate appsettings file based on the build argument
COPY JWT.Api/appsettings.${ENVIRONMENT}.json ./appsettings.json


ENTRYPOINT ["dotnet", "JWT.Api.dll"]