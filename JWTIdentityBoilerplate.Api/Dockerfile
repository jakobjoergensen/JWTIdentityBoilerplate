FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080


# This stage is used to build the project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["JWTIdentityBoilerplate.Api/JWTIdentityBoilerplate.Api.csproj", "JWTIdentityBoilerplate.Api/"]
RUN dotnet restore "JWTIdentityBoilerplate.Api/JWTIdentityBoilerplate.Api.csproj"
COPY . .
WORKDIR "/src/JWTIdentityBoilerplate.Api"
RUN dotnet build "JWTIdentityBoilerplate.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build


# This stage is used to publish the project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "JWTIdentityBoilerplate.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false


# This stage is used in production
FROM base AS final
WORKDIR /app


# Use build argument to copy the correct configuration file
ARG ENVIRONMENT=Production
COPY --from=publish /app/publish .


# Copy the appropriate appsettings file based on the build argument
COPY JWTIdentityBoilerplate.Api/appsettings.${ENVIRONMENT}.json ./appsettings.json


ENTRYPOINT ["dotnet", "JWTIdentityBoilerplate.Api.dll"]