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
RUN apt-get update && apt-get install -y openssl
EXPOSE 80
EXPOSE 443
COPY --from=build /app/DimmedAPI/out ./

# Copiar y ejecutar script de verificación de librerías de PDF
COPY verify-pdf-libs.sh ./
RUN chmod +x verify-pdf-libs.sh
RUN ./verify-pdf-libs.sh

COPY entrypoint.sh ./
RUN chmod +x entrypoint.sh
ENTRYPOINT ["./entrypoint.sh"] 