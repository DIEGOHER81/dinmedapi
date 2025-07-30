# EntryRequest Report API

## Descripción
Este endpoint permite obtener un reporte detallado de las solicitudes de entrada utilizando el procedimiento almacenado `sp_GetEntryRequestsReport`.

## Endpoint

### GET /api/EntryRequest/report

Obtiene un reporte detallado de las solicitudes de entrada con filtros opcionales.

#### Parámetros de consulta

| Parámetro | Tipo | Requerido | Descripción |
|-----------|------|-----------|-------------|
| companyCode | string | Sí | Código de la compañía |
| noEntryRequest | int | No | Número de solicitud de entrada específico |
| dateIni | DateTime | No | Fecha de inicio para el filtro (formato: YYYY-MM-DD) |
| dateEnd | DateTime | No | Fecha de fin para el filtro (formato: YYYY-MM-DD) |
| pageNumber | int | No | Número de página (por defecto: 1) |
| pageSize | int | No | Cantidad de registros por página (por defecto: 50, máximo: 1000) |

#### Ejemplo de solicitud

```http
GET /api/EntryRequest/report?companyCode=COMP001&dateIni=2024-01-01&dateEnd=2024-12-31
```

#### Respuesta exitosa

```json
{
  "data": [
    {
      "pedido": 12345,
      "consumo": 1,
      "fechaCirugia": "2024-01-15T08:00:00",
      "fechaSolicitud": "2024-01-10T10:30:00",
      "estado": "Activo",
      "estadoTrazabilidad": "En proceso",
      "cliente": "Cliente Ejemplo",
      "equipos": "Equipo 1, Equipo 2",
      "direccionEntrega": "Dirección de entrega",
      "prioridadEntrega": "Alta",
      "observaciones": "Observaciones generales",
      "observacionesComerciales": "Observaciones comerciales",
      "nombrePaciente": "Juan Pérez",
      "nombreMedico": "Dr. García",
      "nombreAtc": "ATC Ejemplo",
      "sede": "Sede Principal",
      "servicio": "Servicio de Cirugía",
      "tipodePedido": "Urgente",
      "causalesdenocirugia": "Causales de no cirugía",
      "detallescausalesdenocirugia": "Detalles de causales",
      "aseguradora": "Aseguradora Ejemplo",
      "tipodeaseguradora": "Tipo Aseguradora",
      "solicitante": "Solicitante Ejemplo",
      "ladoExtremidad": "Derecho",
      "fechaTerminacion": "2024-01-16T17:00:00",
      "esReposicion": false,
      "imprimir": true,
      "reporteTrazabilidad": "Reporte de trazabilidad"
    }
  ],
  "totalRecords": 150,
  "pageNumber": 1,
  "pageSize": 50,
  "totalPages": 3,
  "hasPreviousPage": false,
  "hasNextPage": true
}
```

#### Códigos de respuesta

| Código | Descripción |
|--------|-------------|
| 200 | OK - Reporte obtenido exitosamente |
| 400 | Bad Request - Código de compañía requerido |
| 500 | Internal Server Error - Error interno del servidor |

#### Ejemplos de uso

1. **Obtener todas las solicitudes de una compañía (paginado por defecto):**
   ```http
   GET /api/EntryRequest/report?companyCode=COMP001
   ```

2. **Filtrar por número de solicitud específico:**
   ```http
   GET /api/EntryRequest/report?companyCode=COMP001&noEntryRequest=12345
   ```

3. **Filtrar por rango de fechas:**
   ```http
   GET /api/EntryRequest/report?companyCode=COMP001&dateIni=2024-01-01&dateEnd=2024-01-31
   ```

4. **Combinar filtros con paginación personalizada:**
   ```http
   GET /api/EntryRequest/report?companyCode=COMP001&noEntryRequest=12345&dateIni=2024-01-01&dateEnd=2024-01-31&pageNumber=2&pageSize=25
   ```

5. **Obtener segunda página con 100 registros:**
   ```http
   GET /api/EntryRequest/report?companyCode=COMP001&pageNumber=2&pageSize=100
   ```

## Notas

- El endpoint es multicompañía y requiere el parámetro `companyCode`
- Todos los parámetros de filtro son opcionales
- Las fechas se deben enviar en formato ISO 8601 (YYYY-MM-DD)
- El endpoint utiliza caché para mejorar el rendimiento
- Los datos se obtienen del procedimiento almacenado `sp_GetEntryRequestsReport` 