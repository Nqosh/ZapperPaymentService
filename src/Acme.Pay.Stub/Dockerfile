FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore src/Acme.Pay.Stub/Acme.Pay.Stub.csproj
RUN dotnet publish src/Acme.Pay.Stub/Acme.Pay.Stub.csproj -c Release -o /app /p:UseAppHost=false
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
USER $APP_UID
EXPOSE 8080
ENTRYPOINT ["dotnet","Acme.Pay.Stub.dll"]
