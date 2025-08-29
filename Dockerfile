FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env

WORKDIR /app

COPY src/ .

WORKDIR Backend/FfkApi.API

RUN dotnet restore

RUN dotnet publish -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

COPY --from=build-env /app/out .

EXPOSE 8080

ENTRYPOINT [ "dotnet", "FfkApi.API.dll" ]

# docker build -t ffkapi:1.0 .
# docker run -d -p 7209:7209 --name ffkapi ffkapi:1.0
# http://localhost:7209/swagger/index.html

# docker run -d -p 8080:8080 --name ffkapi ffkapi:1.0
# http://localhost:8080/swagger/index.html

# docker exec -it ffkapi tail -f /app/logs/log.txt
