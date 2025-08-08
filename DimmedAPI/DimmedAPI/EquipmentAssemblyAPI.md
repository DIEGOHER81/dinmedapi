# EquipmentAssemblyAPI

API para consultar ensambles y líneas de ensamble de equipos desde Business Central, soportando multi-compañía.

## Información General
- **Tipo:** API Local (no requiere autenticación externa)
- **Base URL:** `/api/EquipmentAssemblyAPI`
- **Método:** Todos los endpoints son GET
- **Formato de respuesta:** JSON

## Parámetro obligatorio
- **companyCode** (query): Código de la compañía sobre la que se realiza la consulta.

## Endpoints

### 1. Obtener el ensamble de un equipo
- **GET** `/api/EquipmentAssemblyAPI/assembly/{equipmentCode}?companyCode={companyCode}&salesPrice={salesPrice}`
- **Descripción:** Obtiene el ensamble de un equipo específico.
- **Parámetros:**
  - `equipmentCode` (path, requerido): Código del equipo.
  - `companyCode` (query, requerido): Código de la compañía.
  - `salesPrice` (query, opcional): Código de precio de venta.
- **Respuesta exitosa:** 200 OK
- **Respuesta de error:** 400 Bad Request
- **Ejemplo de respuesta:**
```json
[
  {
    "code": "EQ-001",
    "description": "Componente 1",
    "quantity": 1,
    "unitPrice": 100.00
  }
]
```

---

### 2. Obtener el ensamble de un equipo (versión 2)
- **GET** `/api/EquipmentAssemblyAPI/assembly-v2/{equipmentCode}?companyCode={companyCode}&salesPrice={salesPrice}`
- **Descripción:** Obtiene el ensamble de un equipo específico (versión 2 con mejoras).
- **Parámetros:**
  - `equipmentCode` (path, requerido): Código del equipo.
  - `companyCode` (query, requerido): Código de la compañía.
  - `salesPrice` (query, opcional): Código de precio de venta.
- **Respuesta exitosa:** 200 OK
- **Respuesta de error:** 400 Bad Request
- **Ejemplo de respuesta:**
```json
[
  {
    "code": "EQ-001",
    "description": "Componente 1",
    "quantity": 1,
    "unitPrice": 100.00,
    "totalPrice": 100.00
  }
]
```

---

### 3. Obtener las líneas de ensamble de un equipo
- **GET** `/api/EquipmentAssemblyAPI/assembly-lines/{equipmentCode}?companyCode={companyCode}`
- **Descripción:** Obtiene las líneas de ensamble de un equipo específico.
- **Parámetros:**
  - `equipmentCode` (path, requerido): Código del equipo.
  - `companyCode` (query, requerido): Código de la compañía.
- **Respuesta exitosa:** 200 OK
- **Respuesta de error:** 400 Bad Request
- **Ejemplo de respuesta:**
```json
[
  {
    "lineNumber": 1,
    "code": "EQ-001",
    "description": "Línea 1",
    "quantity": 1
  }
]
```

---

### 4. Obtener el ensamble de equipo específico
- **GET** `/api/EquipmentAssemblyAPI/equipment-assembly/{equipmentCode}?companyCode={companyCode}`
- **Descripción:** Obtiene el ensamble de equipo específico.
- **Parámetros:**
  - `equipmentCode` (path, requerido): Código del equipo.
  - `companyCode` (query, requerido): Código de la compañía.
- **Respuesta exitosa:** 200 OK
- **Respuesta de error:** 400 Bad Request
- **Ejemplo de respuesta:**
```json
[
  {
    "code": "EQ-001",
    "description": "Componente Ensamble",
    "assemblyType": "Equipment",
    "quantity": 1
  }
]
```

---

## Códigos de Error

### 400 Bad Request
```json
{
  "mensaje": "Error al obtener el ensamble del equipo",
  "detalle": "Descripción específica del error"
}
```

**Causas comunes:**
- `companyCode` no proporcionado
- Error de conexión con Business Central
- Código de equipo no encontrado

---

## Ejemplos de Uso

### Ejemplo 1: Obtener ensamble básico
```bash
curl -X GET "https://tu-api.com/api/EquipmentAssemblyAPI/assembly/EQ-001?companyCode=COMP001"
```

### Ejemplo 2: Obtener ensamble con precio de venta
```bash
curl -X GET "https://tu-api.com/api/EquipmentAssemblyAPI/assembly/EQ-001?companyCode=COMP001&salesPrice=PRICE001"
```

### Ejemplo 3: Obtener líneas de ensamble
```bash
curl -X GET "https://tu-api.com/api/EquipmentAssemblyAPI/assembly-lines/EQ-001?companyCode=COMP001"
```

---

## Notas Importantes
- **API Local:** Esta API es local y no requiere autenticación externa.
- **Multi-compañía:** Todos los endpoints requieren el parámetro `companyCode` para operar sobre la base de datos y la configuración de Business Central de la compañía correspondiente.
- **Conexión BC:** La API utiliza el servicio `IDynamicBCConnectionService` para conectarse dinámicamente a Business Central según el `companyCode`.
- **Manejo de errores:** En caso de error, la API retorna un mensaje descriptivo y el código HTTP correspondiente.
- **Parámetro opcional:** El campo `salesPrice` es opcional y solo aplica para los endpoints de ensamble y ensamble-v2.
- **Versiones:** Existen dos versiones del endpoint de ensamble (assembly y assembly-v2) con diferentes implementaciones.

---

## Archivos Relacionados
- **Controlador:** `Controllers/EquipmentAssemblyAPIController.cs`
- **Servicio:** `Services/DynamicBCConnectionService.cs`
- **Ejemplos:** `EquipmentAssemblyAPIExamples.http` 