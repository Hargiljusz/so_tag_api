FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.sln .
COPY DataCommon/*.csproj ./DataCommon/
COPY DataService/*.csproj ./DataService/
COPY SOTagsAPI/*.csproj ./RestAPI/
#RUN dotnet restore ./RestAPI/RestAPI.csproj

# copy everything else and build app
COPY DataCommon/. ./DataCommon/
COPY DataService/. ./DataService/
COPY SOTagsAPI/. ./SOTagsAPI/
WORKDIR /source/SOTagsAPI
RUN dotnet publish SOTagsAPI.WebAPI.csproj -c release -o /app 

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "SOTagsAPI.WebAPI.dll"]