FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /app

COPY *.csproj ./
COPY *.githubactions ./
COPY Gnoss.Web/Gnoss.Web.csproj ./
COPY Gnoss.Web/Gnoss.Web.csproj.githubactions ./

COPY . ./

RUN dotnet restore Gnoss.Web/Gnoss.Web.csproj.githubactions

COPY . ./
RUN dotnet publish Gnoss.Web/Gnoss.Web.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "Gnoss.Web.dll"]