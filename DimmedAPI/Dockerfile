# Etapa 1: build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copia los archivos de proyecto y restaura dependencias
COPY *.sln ./
COPY DimmedAPI/*.csproj ./DimmedAPI/
RUN dotnet restore

# Copia el resto del código y publica
COPY DimmedAPI/. ./DimmedAPI/
WORKDIR /app/DimmedAPI
RUN dotnet publish -c Release -o out

# Etapa 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80
COPY --from=build /app/DimmedAPI/out ./
ENTRYPOINT ["dotnet", "DimmedAPI.dll"] 