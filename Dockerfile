FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env

RUN sed -i "s|MinProtocol = TLSv1.2|MinProtocol = TLSv1|g" /etc/ssl/openssl.cnf && \
    sed -i 's|CipherString = DEFAULT@SECLEVEL=2|CipherString = DEFAULT@SECLEVEL=1|g' /etc/ssl/openssl.cnf

RUN apt-get update && apt-get install -y --no-install-recommends curl
# install libgdiplus for System.Drawing
RUN apt-get update && apt-get install -y --allow-unauthenticated libgdiplus libc6-dev

WORKDIR /app

COPY Gnoss.Web/*.csproj ./

RUN dotnet restore

COPY . ./

RUN echo $(date +%s) > Gnoss.Web/Config/version.txt

RUN dotnet publish Gnoss.Web/Gnoss.Web.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:6.0

RUN sed -i "s|MinProtocol = TLSv1.2|MinProtocol = TLSv1|g" /etc/ssl/openssl.cnf && \
    sed -i 's|CipherString = DEFAULT@SECLEVEL=2|CipherString = DEFAULT@SECLEVEL=1|g' /etc/ssl/openssl.cnf

RUN apt-get update && apt-get install -y --no-install-recommends curl
# install libgdiplus for System.Drawing
RUN apt-get update && apt-get install -y --allow-unauthenticated libgdiplus libc6-dev
WORKDIR /app

COPY --from=build-env /app/out .

COPY --from=build-env /app/Gnoss.Web.Open/Views/ Views/

ARG version=latest

RUN echo "Version    : $version \nProducto   : Web  \nFecha      : $(date +"%d/%m/%Y %H:%M:%S") \n" > wwwroot/versioninfo.txt

ENTRYPOINT ["dotnet", "Gnoss.Web.dll"]

