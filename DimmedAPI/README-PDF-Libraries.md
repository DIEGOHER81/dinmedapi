# Configuración de Librerías de PDF (DinkToPdf)

## Descripción

Este proyecto utiliza **DinkToPdf** para generar PDFs, que requiere librerías nativas específicas para funcionar correctamente en contenedores Docker.

## Librerías Requeridas

- `libwkhtmltox.dll` - Librería nativa para Windows
- `wkhtmltox.dll` - Librería nativa para Windows (alternativa)
- `DinkToPdf.dll` - Librería .NET principal

## Configuración en el Proyecto

### 1. Archivo .csproj

Se han agregado configuraciones específicas en `DimmedAPI.csproj`:

```xml
<ItemGroup>
  <Content Include="bin\Debug\net9.0\libwkhtmltox.dll">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
  </Content>
  <Content Include="bin\Debug\net9.0\wkhtmltox.dll">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
  </Content>
  <Content Include="bin\Debug\net9.0\runtimes\**\*">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
  </Content>
</ItemGroup>
```

### 2. Dockerfile

El Dockerfile incluye:
- Verificación automática de librerías
- Script de copia automática si las librerías no están en el directorio raíz
- Logs de verificación para debugging

### 3. Script de Verificación

El archivo `verify-pdf-libs.sh` realiza:
- Verificación de presencia de librerías
- Búsqueda automática en subdirectorios
- Copia automática al directorio raíz si es necesario
- Logs detallados del proceso

## Cómo Funciona

1. **Durante el build**: Las librerías se copian desde el directorio de desarrollo
2. **Durante el runtime**: El script verifica que las librerías estén en el directorio raíz
3. **Si faltan**: Se buscan automáticamente en subdirectorios y se copian

## Verificación Manual

Para verificar manualmente que las librerías están correctamente configuradas:

```bash
# En el contenedor
./verify-pdf-libs.sh

# O verificar directamente
ls -la *.dll | grep -E "(libwkhtmltox|wkhtmltox|DinkToPdf)"
```

## Troubleshooting

### Error: "Unable to load DLL 'libwkhtmltox'"

**Causa**: La librería nativa no está en el directorio de ejecución.

**Solución**: 
1. Verificar que `libwkhtmltox.dll` esté en el directorio raíz del contenedor
2. Ejecutar el script de verificación: `./verify-pdf-libs.sh`
3. Revisar los logs del Dockerfile durante el build

### Error: "DinkToPdf.dll not found"

**Causa**: La librería principal de DinkToPdf no está disponible.

**Solución**:
1. Verificar que el paquete NuGet esté instalado correctamente
2. Revisar que `DinkToPdf.dll` esté en el directorio de salida

## Notas Importantes

- Las librerías nativas deben estar en el **directorio raíz** de la aplicación
- DinkToPdf busca las librerías en el directorio de ejecución, no en subdirectorios
- En contenedores Linux, se requieren librerías diferentes (`.so` en lugar de `.dll`)
- El script de verificación es compatible con múltiples plataformas

## Referencias

- [DinkToPdf GitHub](https://github.com/rdvojmoc/DinkToPdf)
- [Documentación de DinkToPdf](https://github.com/rdvojmoc/DinkToPdf/wiki)
