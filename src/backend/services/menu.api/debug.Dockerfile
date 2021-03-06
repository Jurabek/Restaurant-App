FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS build-env
ARG buildconfig
WORKDIR /app
COPY ServiceApp.WebApi.csproj .
RUN dotnet restore
COPY . .
RUN if [ "${buildconfig}" = "Debug" ]; then \
        dotnet publish -o /publish -c Debug; \
    else \
        dotnet publish -o /publish -c Release; \
    fi

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine
ARG buildconfig
ENV DEBIAN_FRONTEND noninteractive
WORKDIR /publish
COPY --from=build-env /publish .
RUN if [ "${buildconfig}" = "Debug" ]; then \
        apt-get update && \
        apt-get install -y --no-install-recommends apt-utils && \
        apt-get install curl unzip procps mongodb -y && \
        curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v latest -l /publish/vsdbg; \
     else \
        echo "*Whistling*"; \
    fi
ENV DEBIAN_FRONTEND teletype
ENTRYPOINT [ "dotnet","ServiceApp.WebApi.dll" ]