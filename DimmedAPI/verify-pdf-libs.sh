#!/bin/bash

echo "=== Verificación de librerías de DinkToPdf ==="

# Verificar si las librerías están en el directorio raíz
if [ -f "libwkhtmltox.dll" ]; then
    echo "✓ libwkhtmltox.dll encontrada en el directorio raíz"
else
    echo "✗ libwkhtmltox.dll NO encontrada en el directorio raíz"
fi

if [ -f "wkhtmltox.dll" ]; then
    echo "✓ wkhtmltox.dll encontrada en el directorio raíz"
else
    echo "✗ wkhtmltox.dll NO encontrada en el directorio raíz"
fi

if [ -f "DinkToPdf.dll" ]; then
    echo "✓ DinkToPdf.dll encontrada en el directorio raíz"
else
    echo "✗ DinkToPdf.dll NO encontrada en el directorio raíz"
fi

# Buscar las librerías en subdirectorios
echo ""
echo "=== Buscando librerías en subdirectorios ==="

LIBWKHTMLTOX_PATH=$(find . -name "libwkhtmltox.dll" 2>/dev/null | head -1)
WKHTMLTOX_PATH=$(find . -name "wkhtmltox.dll" 2>/dev/null | head -1)

if [ -n "$LIBWKHTMLTOX_PATH" ]; then
    echo "✓ libwkhtmltox.dll encontrada en: $LIBWKHTMLTOX_PATH"
    if [ ! -f "libwkhtmltox.dll" ]; then
        echo "  Copiando libwkhtmltox.dll al directorio raíz..."
        cp "$LIBWKHTMLTOX_PATH" .
    fi
else
    echo "✗ libwkhtmltox.dll NO encontrada en ningún subdirectorio"
fi

if [ -n "$WKHTMLTOX_PATH" ]; then
    echo "✓ wkhtmltox.dll encontrada en: $WKHTMLTOX_PATH"
    if [ ! -f "wkhtmltox.dll" ]; then
        echo "  Copiando wkhtmltox.dll al directorio raíz..."
        cp "$WKHTMLTOX_PATH" .
    fi
else
    echo "✗ wkhtmltox.dll NO encontrada en ningún subdirectorio"
fi

echo ""
echo "=== Estado final ==="
ls -la *.dll | grep -E "(libwkhtmltox|wkhtmltox|DinkToPdf)" || echo "No se encontraron librerías de PDF"

echo ""
echo "=== Verificación completada ==="
