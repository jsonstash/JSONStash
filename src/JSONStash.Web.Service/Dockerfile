#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["JSONStash.Web.Service/JSONStash.Web.Service.csproj", "JSONStash.Web.Service/"]
COPY ["JSONStash.Common/JSONStash.Common.csproj", "JSONStash.Common/"]
RUN dotnet restore "JSONStash.Web.Service/JSONStash.Web.Service.csproj"
COPY . .
WORKDIR "/src/JSONStash.Web.Service"
RUN dotnet build "JSONStash.Web.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "JSONStash.Web.Service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "JSONStash Web Service.dll"]