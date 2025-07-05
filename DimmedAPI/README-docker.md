# Despliegue de DimmedAPI en Docker

## Requisitos
- Docker instalado

## Construcción de la imagen

```
docker build -t dimmedapi .
```

## Ejecución del contenedor

```
docker run -d -p 8080:80 --name dimmedapi dimmedapi
```

La API estará disponible en http://localhost:8080

## Notas
- Si necesitas conectar a una base de datos externa, ajusta la cadena de conexión en `appsettings.json` o usa variables de entorno.
- Para despliegues productivos, revisa la configuración de puertos, variables de entorno y secretos. 