#!/bin/bash

# Script para copiar librerías adicionales después del publish
echo "Copiando librerías adicionales..."

# Crear directorios si no existen
mkdir -p /app/runtimes/linux-x64/native
mkdir -p /app/runtimes/win-x64/native
mkdir -p /app/lib

# Copiar librerías nativas (ejemplos)
# cp /tmp/libs/libmi-libreria.so /app/runtimes/linux-x64/native/
# cp /tmp/libs/mi-libreria.dll /app/runtimes/win-x64/native/

# Copiar librerías .NET
# cp /tmp/libs/MiLibreria.dll /app/lib/

echo "Librerías copiadas exitosamente"
