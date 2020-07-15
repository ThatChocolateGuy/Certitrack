#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:2.1-stretch-slim AS base
WORKDIR /app
EXPOSE 80
ONBUILD RUN apt-get update && apt-get install -y gnupg gconf-service libasound2 libatk1.0-0 libc6 libcairo2 libcups2 libdbus-1-3 libexpat1 libfontconfig1 libgcc1 libgconf-2-4 libgdk-pixbuf2.0-0 libglib2.0-0 libgtk-3-0 libnspr4 libpango-1.0-0 libpangocairo-1.0-0 libstdc++6 libx11-6 libx11-xcb1 libxcb1 libxcomposite1 libxcursor1 libxdamage1 libxext6 libxfixes3 libxi6 libxrandr2 libxrender1 libxss1 libxtst6 ca-certificates fonts-liberation libappindicator1 libnss3 lsb-release xdg-utils wget && \
    wget -q -O - https://dl-ssl.google.com/linux/linux_signing_key.pub | apt-key add - && \
    sh -c 'echo "deb [arch=amd64] http://dl.google.com/linux/chrome/deb/ stable main" >> /etc/apt/sources.list.d/google.list' && \
    apt-get update && \
    apt-get install -y google-chrome-unstable --no-install-recommends

ENV chrome:launchOptions:args --no-sandbox

FROM mcr.microsoft.com/dotnet/core/sdk:2.1-alpine AS build
WORKDIR /src
# copy csproj and restore as distinct layers
COPY ["certitrack-certificate-manager.csproj", ""]
RUN dotnet restore "./certitrack-certificate-manager.csproj" -r linux-musl-x64
# copy everything else and build app
COPY . .
WORKDIR "/src/."
RUN dotnet publish "certitrack-certificate-manager.csproj" -c Release -o /app/publish /p:PublishTrimmed=true /p:PublishReadyToRun=true

#FROM build AS publish
#RUN dotnet publish "certitrack-certificate-manager.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "certitrack-certificate-manager.dll"]