FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS build
WORKDIR /src

COPY Menu.API/ ./api
RUN dotnet restore api/Menu.API.csproj

COPY . .

RUN dotnet build api/Menu.API.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish api/Menu.API.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine AS final
WORKDIR /app

COPY --from=publish /app .
EXPOSE 80

CMD ["dotnet", "Menu.API.dll"]