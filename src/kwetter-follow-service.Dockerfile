FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["Kwetter.Services/Kwetter.Services.FollowService/Kwetter.Services.FollowService.API/Kwetter.Services.FollowService.API.csproj", "Kwetter.Services/Kwetter.Services.FollowService/Kwetter.Services.FollowService.API/"]
RUN dotnet restore "Kwetter.Services/Kwetter.Services.FollowService/Kwetter.Services.FollowService.API/Kwetter.Services.FollowService.API.csproj"
COPY . .
WORKDIR "/src/Kwetter.Services/Kwetter.Services.FollowService/Kwetter.Services.FollowService.API"
RUN dotnet build "Kwetter.Services.FollowService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Kwetter.Services.FollowService.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Kwetter.Services.FollowService.API.dll"]