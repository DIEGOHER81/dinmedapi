# Items BC With Price List API

## Endpoints de Sincronización

### 1. Sincronización Normal
**POST** `/api/ItemsAPI/sincronizar-items-bc-pricelist`

Sincroniza items con price list desde Business Central.

#### Parámetros:
- `companyCode` (string, requerido): Código de la compañía
- `take` (int, opcional): Número de items a sincronizar. Si no se especifica o es 0, se sincronizan todos los items en bloques de 100.

#### Ejemplos de uso:

**Sincronizar solo 50 items:**
```bash
curl -X 'POST' \
  'https://localhost:7063/api/ItemsAPI/sincronizar-items-bc-pricelist?companyCode=6c265367-24c4-ed11-9a88-002248e00201&take=50' \
  -H 'accept: */*' \
  -d ''
```

**Sincronizar todos los items en bloques de 100:**
```bash
curl -X 'POST' \
  'https://localhost:7063/api/ItemsAPI/sincronizar-items-bc-pricelist?companyCode=6c265367-24c4-ed11-9a88-002248e00201' \
  -H 'accept: */*' \
  -d ''
```

#### Respuesta cuando `take` tiene valor:
```json
{
  "totalSincronizados": 50,
  "mensaje": "Sincronización completada. Total de items procesados: 50"
}
```

#### Respuesta cuando `take` está vacío (sincronización en bloques):
```json
{
  "totalSincronizados": 1250,
  "totalBloques": 13,
  "mensaje": "Sincronización completada exitosamente. Total de items procesados: 1250 en 13 bloques",
  "detalles": [
    {
      "bloque": 1,
      "itemsEnBloque": 100,
      "totalAcumulado": 100,
      "mensaje": "Bloque 1 procesado: 100 items"
    },
    {
      "bloque": 2,
      "itemsEnBloque": 100,
      "totalAcumulado": 200,
      "mensaje": "Bloque 2 procesado: 100 items"
    }
    // ... más bloques
  ]
}
```

### 2. Sincronización con Progreso en Tiempo Real
**POST** `/api/ItemsAPI/sincronizar-items-bc-pricelist-progreso`

Sincroniza items con price list mostrando el progreso en tiempo real usando Server-Sent Events (SSE).

#### Parámetros:
- `companyCode` (string, requerido): Código de la compañía

#### Ejemplo de uso:
```bash
curl -X 'POST' \
  'https://localhost:7063/api/ItemsAPI/sincronizar-items-bc-pricelist-progreso?companyCode=6c265367-24c4-ed11-9a88-002248e00201' \
  -H 'accept: text/event-stream'
```

#### Respuesta (Server-Sent Events):
```
data: {"tipo":"inicio","mensaje":"Iniciando sincronización de items con price list..."}

data: {"tipo":"limpieza","mensaje":"Limpiando tabla local..."}

data: {"tipo":"limpieza_completada","mensaje":"Tabla local limpiada exitosamente"}

data: {"tipo":"procesando_bloque","bloque":1,"mensaje":"Procesando bloque 1..."}

data: {"tipo":"bloque_completado","bloque":1,"itemsEnBloque":100,"totalAcumulado":100,"mensaje":"Bloque 1 completado: 100 items procesados"}

data: {"tipo":"procesando_bloque","bloque":2,"mensaje":"Procesando bloque 2..."}

data: {"tipo":"bloque_completado","bloque":2,"itemsEnBloque":100,"totalAcumulado":200,"mensaje":"Bloque 2 completado: 100 items procesados"}

data: {"tipo":"ultimo_bloque","mensaje":"Último bloque procesado"}

data: {"tipo":"completado","totalSincronizados":250,"totalBloques":3,"mensaje":"Sincronización completada exitosamente. Total de items procesados: 250 en 3 bloques"}
```

### 3. Consulta de Items Locales
**GET** `/api/ItemsAPI/items-bc-pricelist-local`

Consulta los items sincronizados en la base de datos local.

#### Parámetros:
- `companyCode` (string, requerido): Código de la compañía
- `take` (int, opcional): Número de items a retornar

#### Ejemplo de uso:
```bash
curl -X 'GET' \
  'https://localhost:7063/api/ItemsAPI/items-bc-pricelist-local?companyCode=6c265367-24c4-ed11-9a88-002248e00201&take=10' \
  -H 'accept: */*'
```

## Características de la Sincronización en Bloques

Cuando el parámetro `take` está vacío o es 0, el sistema:

1. **Limpia la tabla local** antes de comenzar la sincronización
2. **Procesa en bloques de 100 items** hasta completar toda la sincronización
3. **Informa el progreso** de cada bloque procesado
4. **Detecta automáticamente** cuando no hay más items para procesar
5. **Maneja errores** y proporciona información detallada del estado

### Ventajas de la Sincronización en Bloques:

- **Mejor rendimiento**: Evita timeouts en operaciones largas
- **Información detallada**: Proporciona estadísticas de cada bloque
- **Manejo de errores**: Si falla un bloque, se puede identificar fácilmente
- **Progreso visible**: El usuario puede ver el avance en tiempo real
- **Escalabilidad**: Funciona bien con grandes volúmenes de datos

### Tipos de Eventos en SSE:

- `inicio`: Inicio de la sincronización
- `limpieza`: Limpieza de la tabla local
- `limpieza_completada`: Limpieza completada
- `procesando_bloque`: Procesando un bloque específico
- `bloque_completado`: Bloque procesado exitosamente
- `sin_mas_items`: No hay más items para procesar
- `ultimo_bloque`: Último bloque procesado
- `completado`: Sincronización completada
- `error`: Error durante la sincronización 