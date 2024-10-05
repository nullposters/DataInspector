FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Nullposters.DataInspector.UI/Nullposters.DataInspector.UI.csproj", "Nullposters.DataInspector.UI/"]

RUN dotnet restore "Nullposters.DataInspector.UI/Nullposters.DataInspector.UI.csproj"

COPY . .

RUN dotnet publish "Nullposters.DataInspector.UI/Nullposters.DataInspector.UI.csproj"

FROM nginx:alpine
WORKDIR /usr/share/nginx/html
COPY --from=build /app/publish/wwwroot .
EXPOSE 80
