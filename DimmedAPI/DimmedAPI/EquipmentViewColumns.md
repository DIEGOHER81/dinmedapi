# Columnas del Procedimiento Almacenado sp_Equipment_view

## Nombres Exactos de Columnas

Basado en el procedimiento almacenado `sp_Equipment_view`, aquí están los nombres exactos de las columnas que devuelve:

| Campo en C# | Nombre en SP | Tipo | Descripción |
|-------------|--------------|------|-------------|
| `Codigo` | `Codigo` | string | Código del equipo |
| `Nombre` | `Nombre` | string | Nombre del equipo |
| `Estado` | `Estado` | string | Estado del equipo (aparece 2 veces en el SP) |
| `Sede` | `Sede` | string | Sede donde se encuentra |
| `UltimoPedido` | `UltimoPedido` | string | Número del último pedido |
| `FechaUltimaCirugia` | `FechaUltimaCirugia` | string | Fecha de la última cirugía |
| `EstadoPedido` | `EstadoPedido` | string | Estado del pedido |
| `EstadoTrazabilidad` | `EstadoTrazabilidad` | string | Estado de trazabilidad |
| `FechaTrazabilidad` | `FEchaTrazabilidad` | string | **NOTA: Hay un error de tipeo en el SP** |
| `Institucion` | `Institucion` | string | Institución asociada |
| `DireccionEntrega` | `DireccíonEntrega` | string | **NOTA: Con acento en el SP** |
| `TipoEquipo` | `TipoEquipo` | string | Tipo de equipo |
| `EquipoPrincipal` | `EquipoPrincipal` | string | Indica si es equipo principal |
| `NroCajas` | `NroCajas` | int | Número de cajas |
| `LineaProducto` | `LineaProducto` | string | Línea de producto |
| `Ciclocirugia` | `Ciclocirugia` | string | Ciclo de cirugía |
| `FechaInicio` | `FechaInicio` | DateTime | Fecha de inicio |
| `FechaRetiro` | `FechaRetiro` | string | Fecha de retiro |
| `Proveedor` | `Proveedor` | string | Proveedor del equipo |
| `Marca` | `Marca` | string | Marca del equipo |
| `Modelo` | `Modelo` | string | Modelo del equipo |
| `Clasificacion` | `Clasificacion` | string | Clasificación del equipo |
| `Alerta` | `Alerta` | string | Alertas asociadas |

## Problemas Identificados en el SP

### 1. Error de Tipeo
```sql
-- En el SP está:
ISNULL(Movimiento.FechaTrazabilidad,'') as FEchaTrazabilidad,
-- Debería ser:
ISNULL(Movimiento.FechaTrazabilidad,'') as FechaTrazabilidad,
```

### 2. Acento en Nombre de Columna
```sql
-- En el SP está:
ISNULL(Movimiento.DireccionEntrega,'') AS DireccíonEntrega,
-- Debería ser:
ISNULL(Movimiento.DireccionEntrega,'') AS DireccionEntrega,
```

### 3. Columna Estado Duplicada
El SP devuelve la columna `Estado` dos veces:
- Una del equipo: `Eq.Status as Estado`
- Otra del movimiento: `UPPER(Eq.Status) as Estado`

## Recomendaciones

1. **Corregir el SP** para evitar problemas futuros:
   ```sql
   -- Corregir tipeo
   ISNULL(Movimiento.FechaTrazabilidad,'') as FechaTrazabilidad,
   
   -- Quitar acento
   ISNULL(Movimiento.DireccionEntrega,'') AS DireccionEntrega,
   
   -- Evitar duplicación de Estado
   UPPER(Eq.Status) as EstadoEquipo,
   ```

2. **Usar el endpoint de diagnóstico** para verificar columnas:
   ```http
   GET /api/equipment/view/columns?companyCode=DINMED
   ```

3. **Mantener consistencia** en nombres de columnas sin caracteres especiales. 