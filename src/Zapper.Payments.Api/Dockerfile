FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore Zapper.Payments.sln
RUN dotnet publish src/Zapper.Payments.Api/Zapper.Payments.Api.csproj -c Release -o /app /p:UseAppHost=false
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
USER $APP_UID
EXPOSE 8080
ENTRYPOINT ["dotnet","Zapper.Payments.Api.dll"]
