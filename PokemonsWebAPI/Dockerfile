FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["PokemonsWebAPI.csproj", "./"]
RUN dotnet restore "PokemonsWebAPI.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "PokemonsWebAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PokemonsWebAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PokemonsWebAPI.dll"]
