# Equipment View API

Este endpoint ejecuta el procedimiento almacenado `sp_Equipment_view` para obtener una vista completa de los equipos con su último movimiento.

## Endpoint

### GET /api/equipment/view

Obtiene la vista completa de equipos ejecutando el procedimiento almacenado `sp_Equipment_view`.

#### Parámetros de Query

- `companyCode` (requerido): Código de la compañía
- `page` (opcional): Número de página (por defecto: 1)
- `pageSize` (opcional): Tamaño de página (por defecto: 10)
- `filter` (opcional): Filtro de texto para buscar en código, nombre, estado o sede

#### Ejemplo de Request

```http
GET /api/equipment/view?companyCode=DINMED&page=1&pageSize=20&filter=equipo
```

#### Respuesta Exitosa (200 OK)

```json
{
  "total": 150,
  "page": 1,
  "pageSize": 20,
  "data": [
    {
      "codigo": "EQ001",
      "nombre": "Equipo de Cirugía",
      "estado": "Activo",
      "sede": "Sede Principal",
      "ultimoPedido": "PED-2024-001",
      "fechaUltimaCirugia": "2024-01-15T10:30:00",
      "estadoPedido": "Completado",
      "estadoTrazabilidad": "En Tránsito",
      "fechaTrazabilidad": "2024-01-16T08:00:00",
      "institucion": "Hospital General",
      "direccionEntrega": "Calle 123 #45-67",
      "tipoEquipo": "Quirúrgico",
      "equipoPrincipal": "Sí",
      "nroCajas": 3,
      "lineaProducto": "Cirugía",
      "ciclocirugia": "Completo",
      "fechaInicio": "2024-01-01T00:00:00",
      "fechaRetiro": null,
      "proveedor": "Proveedor ABC",
      "marca": "Marca XYZ",
      "modelo": "Modelo 2024",
      "clasificacion": "A",
      "alerta": "Sin alertas"
    }
  ],
  "message": "Datos obtenidos del procedimiento almacenado sp_Equipment_view"
}
```

#### Campos de Respuesta

| Campo | Tipo | Descripción |
|-------|------|-------------|
| `codigo` | string | Código del equipo |
| `nombre` | string | Nombre del equipo |
| `estado` | string | Estado actual del equipo |
| `sede` | string | Sede donde se encuentra el equipo |
| `ultimoPedido` | string | Número del último pedido |
| `fechaUltimaCirugia` | DateTime? | Fecha de la última cirugía |
| `estadoPedido` | string | Estado del pedido |
| `estadoTrazabilidad` | string | Estado de trazabilidad |
| `fechaTrazabilidad` | DateTime? | Fecha de trazabilidad |
| `institucion` | string | Institución asociada |
| `direccionEntrega` | string | Dirección de entrega |
| `tipoEquipo` | string | Tipo de equipo |
| `equipoPrincipal` | string | Indica si es equipo principal |
| `nroCajas` | int? | Número de cajas |
| `lineaProducto` | string | Línea de producto |
| `ciclocirugia` | string | Ciclo de cirugía |
| `fechaInicio` | DateTime? | Fecha de inicio |
| `fechaRetiro` | DateTime? | Fecha de retiro |
| `proveedor` | string | Proveedor del equipo |
| `marca` | string | Marca del equipo |
| `modelo` | string | Modelo del equipo |
| `clasificacion` | string | Clasificación del equipo |
| `alerta` | string | Alertas asociadas |

#### Códigos de Error

- `400 Bad Request`: Código de compañía no proporcionado
- `404 Not Found`: Compañía no encontrada
- `500 Internal Server Error`: Error interno del servidor

#### Notas

- Este endpoint ejecuta el procedimiento almacenado `sp_Equipment_view` en la base de datos de la compañía especificada
- Los datos se mapean manualmente usando ADO.NET directo para evitar problemas de mapeo automático
- Los datos se filtran en memoria después de obtenerlos del procedimiento almacenado
- La paginación se aplica después del filtrado
- El filtro busca en los campos: código, nombre, estado y sede
- No se requiere migración de base de datos ya que se ejecuta un procedimiento almacenado existente
- El campo `Codigo` es la clave única para identificar cada equipo 