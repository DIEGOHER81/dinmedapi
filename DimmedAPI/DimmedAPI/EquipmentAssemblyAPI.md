# EquipmentAssemblyAPI

API para consultar ensambles y líneas de ensamble de equipos desde Business Central, soportando multi-compañía.

## Parámetro obligatorio
- **companyCode** (query): Código de la compañía sobre la que se realiza la consulta.

## Endpoints

### Obtener el ensamble de un equipo
- **GET** `/api/EquipmentAssemblyAPI/assembly/{equipmentCode}?companyCode={companyCode}&salesPrice={salesPrice}`
- **Descripción:** Obtiene el ensamble de un equipo específico.
- **Parámetros:**
  - `equipmentCode` (path, requerido): Código del equipo.
  - `companyCode` (query, requerido): Código de la compañía.
  - `salesPrice` (query, opcional): Código de precio de venta.
- **Respuesta:**
```json
[
  {
    "code": "EQ-001",
    "description": "Componente 1",
    ...
  }
]
```

---

### Obtener el ensamble de un equipo (versión 2)
- **GET** `/api/EquipmentAssemblyAPI/assembly-v2/{equipmentCode}?companyCode={companyCode}&salesPrice={salesPrice}`
- **Descripción:** Obtiene el ensamble de un equipo específico (versión 2).
- **Parámetros:**
  - `equipmentCode` (path, requerido): Código del equipo.
  - `companyCode` (query, requerido): Código de la compañía.
  - `salesPrice` (query, opcional): Código de precio de venta.
- **Respuesta:**
```json
[
  {
    "code": "EQ-001",
    "description": "Componente 1",
    ...
  }
]
```

---

### Obtener las líneas de ensamble de un equipo
- **GET** `/api/EquipmentAssemblyAPI/assembly-lines/{equipmentCode}?companyCode={companyCode}`
- **Descripción:** Obtiene las líneas de ensamble de un equipo específico.
- **Parámetros:**
  - `equipmentCode` (path, requerido): Código del equipo.
  - `companyCode` (query, requerido): Código de la compañía.
- **Respuesta:**
```json
[
  {
    "code": "EQ-001",
    "description": "Línea 1",
    ...
  }
]
```

---

### Obtener el ensamble de equipo específico
- **GET** `/api/EquipmentAssemblyAPI/equipment-assembly/{equipmentCode}?companyCode={companyCode}`
- **Descripción:** Obtiene el ensamble de equipo específico.
- **Parámetros:**
  - `equipmentCode` (path, requerido): Código del equipo.
  - `companyCode` (query, requerido): Código de la compañía.
- **Respuesta:**
```json
[
  {
    "code": "EQ-001",
    "description": "Componente Ensamble",
    ...
  }
]
```

---

## Notas
- Todos los endpoints requieren el parámetro `companyCode` para operar sobre la base de datos y la configuración de Business Central de la compañía correspondiente.
- En caso de error, la API retorna un mensaje descriptivo y el código HTTP correspondiente.
- El campo `salesPrice` es opcional y solo aplica para los endpoints de ensamble y ensamble-v2. 