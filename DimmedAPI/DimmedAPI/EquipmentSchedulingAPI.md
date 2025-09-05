# Equipment Scheduling Validation API

## Descripción
API para validar si es permitido el agendamiento de un equipo en un rango de fechas específico. Esta validación verifica si existe algún `EntryRequestDetail` con estado 'NUEVO' que se superponga con el rango de fechas solicitado.

## Endpoints

### 1. Validar Agendamiento (POST)
**Endpoint:** `POST /api/EquipmentScheduling/validate`

**Descripción:** Valida si es permitido el agendamiento de un equipo usando parámetros en el body de la petición.

**Parámetros de Query:**
- `companyCode` (string, requerido): Código de la compañía

**Body (JSON):**
```json
{
  "idEquipment": 2,
  "dateIni": "2025-08-23T00:00:00",
  "dateEnd": "2025-08-25T23:59:59"
}
```

**Respuesta Exitosa (200 OK):**
```json
{
  "isAllowed": false,
  "message": "El equipo EQ001 - Equipo de Cirugía no está disponible en el rango de fechas especificado. Ya tiene un agendamiento activo que se superpone con las fechas solicitadas.",
  "idEquipment": 2,
  "dateIni": "2025-08-23T00:00:00",
  "dateEnd": "2025-08-25T23:59:59",
  "validatedAt": "2025-01-27T10:30:00.000Z"
}
```

**Respuesta de Equipo Disponible:**
```json
{
  "isAllowed": true,
  "message": "El equipo EQ001 - Equipo de Cirugía está disponible para el agendamiento en el rango de fechas especificado.",
  "idEquipment": 2,
  "dateIni": "2025-08-23T00:00:00",
  "dateEnd": "2025-08-25T23:59:59",
  "validatedAt": "2025-01-27T10:30:00.000Z"
}
```

### 2. Validar Agendamiento (GET)
**Endpoint:** `GET /api/EquipmentScheduling/validate`

**Descripción:** Valida si es permitido el agendamiento de un equipo usando parámetros de query string.

**Parámetros de Query:**
- `companyCode` (string, requerido): Código de la compañía
- `idEquipment` (int, requerido): ID del equipo
- `dateIni` (string, requerido): Fecha inicial en formato yyyy-MM-dd
- `dateEnd` (string, requerido): Fecha final en formato yyyy-MM-dd

**Ejemplo de URL:**
```
GET /api/EquipmentScheduling/validate?companyCode=COMP001&idEquipment=2&dateIni=2025-08-23&dateEnd=2025-08-25
```

**Respuesta:** Misma estructura que el endpoint POST.

## Códigos de Error

### 400 Bad Request
- Código de compañía faltante
- Parámetros de validación faltantes
- Fechas con formato inválido
- Fecha inicial mayor o igual a fecha final
- ID de equipo inválido

**Ejemplo:**
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "IdEquipment": ["El ID del equipo es requerido"]
  }
}
```

### 500 Internal Server Error
- Error interno del servidor
- Error de conexión a la base de datos

**Ejemplo:**
```json
{
  "isAllowed": false,
  "message": "Error interno del servidor: Connection timeout",
  "idEquipment": 2,
  "dateIni": "2025-08-23T00:00:00",
  "dateEnd": "2025-08-25T23:59:59",
  "validatedAt": "2025-01-27T10:30:00.000Z"
}
```

## Lógica de Validación

La validación se basa en la siguiente consulta SQL:

```sql
IF EXISTS (
    SELECT 1
    FROM EntryRequestDetails
    WHERE idEquipment = @idEquipment
      AND Status = 'NUEVO'
      AND (
           @dateIni <= DateEnd
       AND @dateEnd >= DateIni
      )
)
BEGIN
    -- No permitido: existe conflicto
END
ELSE
BEGIN
    -- Permitido: no hay conflictos
END
```

### Criterios de Validación:
1. **Equipo Existente:** Verifica que el equipo con el ID especificado existe en la base de datos
2. **Estado NUEVO:** Solo considera `EntryRequestDetails` con estado 'NUEVO'
3. **Superposición de Fechas:** Detecta si el rango solicitado se superpone con algún agendamiento existente
4. **Multicompañía:** Utiliza el contexto de base de datos específico de la compañía

## Ejemplos de Uso

### Ejemplo 1: Validación Exitosa
```bash
curl -X POST "https://api.dimmed.com/api/EquipmentScheduling/validate?companyCode=COMP001" \
  -H "Content-Type: application/json" \
  -d '{
    "idEquipment": 2,
    "dateIni": "2025-08-23T00:00:00",
    "dateEnd": "2025-08-25T23:59:59"
  }'
```

### Ejemplo 2: Validación con GET
```bash
curl -X GET "https://api.dimmed.com/api/EquipmentScheduling/validate?companyCode=COMP001&idEquipment=2&dateIni=2025-08-23&dateEnd=2025-08-25"
```

### Ejemplo 3: Equipo No Disponible
```bash
curl -X POST "https://api.dimmed.com/api/EquipmentScheduling/validate?companyCode=COMP001" \
  -H "Content-Type: application/json" \
  -d '{
    "idEquipment": 2,
    "dateIni": "2025-08-23T00:00:00",
    "dateEnd": "2025-08-25T23:59:59"
  }'
```

**Respuesta:**
```json
{
  "isAllowed": false,
  "message": "El equipo EQ001 - Equipo de Cirugía no está disponible en el rango de fechas especificado. Ya tiene un agendamiento activo que se superpone con las fechas solicitadas.",
  "idEquipment": 2,
  "dateIni": "2025-08-23T00:00:00",
  "dateEnd": "2025-08-25T23:59:59",
  "validatedAt": "2025-01-27T10:30:00.000Z"
}
```

## Notas Técnicas

- **Multicompañía:** El endpoint utiliza el servicio `IDynamicConnectionService` para obtener el contexto de base de datos específico de cada compañía
- **Validación de Fechas:** Se valida que la fecha inicial sea anterior a la fecha final
- **Manejo de Errores:** Todos los errores se capturan y se devuelven en un formato consistente
- **Performance:** Utiliza consultas SQL optimizadas para verificar conflictos de agendamiento
- **Logging:** Se recomienda implementar logging para auditoría de validaciones

## Dependencias

- `ApplicationDBContext`: Contexto de base de datos
- `IDynamicConnectionService`: Servicio para conexiones multicompañía
- `EquipmentSchedulingBO`: Lógica de negocio para validación
- `EquipmentSchedulingValidationRequestDTO`: DTO de entrada
- `EquipmentSchedulingValidationResponseDTO`: DTO de respuesta
