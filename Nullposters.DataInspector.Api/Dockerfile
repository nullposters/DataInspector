FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Nullposters.DataInspector.Api/Nullposters.DataInspector.Api.csproj", "Nullposters.DataInspector.Api/"]

RUN dotnet restore "Nullposters.DataInspector.Api/Nullposters.DataInspector.Api.csproj"

COPY . .

RUN dotnet publish "Nullposters.DataInspector.Api/Nullposters.DataInspector.Api.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Nullposters.DataInspector.Api.dll"]
EXPOSE 80