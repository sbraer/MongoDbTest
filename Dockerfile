FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY MongoDbTest/*.csproj .
RUN dotnet restore

# copy everything else and build app
COPY MongoDbTest/. .
WORKDIR /app
RUN dotnet publish -c Release -o out


FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine AS runtime
WORKDIR /app
LABEL maintainer="az"
ENV ASPNETCORE_Environment=Production
ENV ASPNETCORE_URLS http://+:5000
EXPOSE 5000/tcp
RUN apk --no-cache add curl
COPY --from=build /app/out ./
HEALTHCHECK --start-period=5s --interval=10s --timeout=3s --retries=5 CMD curl -f http://localhost:5000/api/info || exit 1
ENTRYPOINT ["dotnet", "MongoDbTest.dll"]

# https://docs.docker.com/engine/examples/dotnetcore/