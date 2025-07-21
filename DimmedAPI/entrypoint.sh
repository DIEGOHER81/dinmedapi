#!/bin/bash
set -e

# Variables de entrada
CERT_PATH=${TLS_CERT:-/etc/tls/tls.crt}
KEY_PATH=${TLS_KEY:-/etc/tls/tls.key}
PFX_PATH=/tmp/cert.pfx
PFX_PASSWORD=${PFX_PASSWORD:-SuperSecreta123}

# Convierte PEM a PFX si existen los archivos
if [[ -f "$CERT_PATH" && -f "$KEY_PATH" ]]; then
  echo "Convirtiendo $CERT_PATH y $KEY_PATH a $PFX_PATH"
  openssl pkcs12 -export \
    -out "$PFX_PATH" \
    -inkey "$KEY_PATH" \
    -in "$CERT_PATH" \
    -password pass:"$PFX_PASSWORD"
else
  echo "No se encontraron los archivos PEM, ejecutando sin HTTPS"
fi

# Ejecuta la app .NET
if [[ -f "$PFX_PATH" ]]; then
  export ASPNETCORE_Kestrel__Certificates__Default__Path="$PFX_PATH"
  export ASPNETCORE_Kestrel__Certificates__Default__Password="$PFX_PASSWORD"
  export ASPNETCORE_URLS="https://+:443;http://+:80"
else
  export ASPNETCORE_URLS="http://+:80"
fi

exec dotnet DimmedAPI.dll 