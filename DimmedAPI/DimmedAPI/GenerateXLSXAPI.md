# Generate XLSX API

## Descripción
Este endpoint permite generar un archivo Excel de remisión para una solicitud de entrada específica. El archivo contiene información detallada sobre el cliente, paciente, equipos, componentes y métodos de esterilización.

## Endpoint
```
GET /api/EntryRequest/generate-xlsx
```

## Parámetros de Consulta

| Parámetro | Tipo | Requerido | Descripción |
|-----------|------|-----------|-------------|
| IdEntryReq | int | Sí | ID de la solicitud de entrada |
| companyCode | string | Sí | Código de la compañía |

## Respuesta
- **Tipo**: `FileResult`
- **Content-Type**: `application/vnd.openxmlformats-officedocument.spreadsheetml.sheet`
- **Nombre del archivo**: `remision_{IdEntryReq}.xlsx`

## Estructura del Excel

### Encabezado
- Título: "REMISIÓN P-{IdEntryReq}"
- Información del cliente pagador (nombre, NIT, dirección, teléfono, ciudad)
- Información del paciente (nombre, cédula, dirección de entrega, solicitante, doctor, orden de compra, historia clínica)

### Asesor Técnico
- Nombre del asesor técnico
- Fecha de entrega
- Fecha de cirugía
- Hora de cirugía
- Fecha de recogida

### Componentes Adicionales (si existen)
- REF (referencia)
- Descripción
- INVIMA
- LOTE
- Cantidad LT
- Cantidad Total
- Precio Unitario
- IVA
- Gasto

### Equipos y Componentes
- Código y nombre del equipo
- Número de cajas
- Componentes del equipo con sus detalles

### Lista de Equipos
- ID Master
- Nombre del Master

### Áreas de Verificación
- Área de Distribución
- Área de Lavado
- Checklist de verificación

### Métodos de Esterilización
- Tabla con diferentes marcas y sus métodos de esterilización
- Temperaturas y tiempos de ciclo

### Cantidades en 0
- Lista de componentes con cantidad 0

## Ejemplo de Uso

### Request
```http
GET /api/EntryRequest/generate-xlsx?IdEntryReq=123&companyCode=COMP001
```

### Response
El endpoint devuelve un archivo Excel descargable con el nombre `remision_123.xlsx`.

## Códigos de Error

| Código | Descripción |
|--------|-------------|
| 400 | El código de compañía es requerido |
| 404 | No se encontró la solicitud de entrada con el ID especificado |
| 500 | Error interno del servidor |

## Notas
- El endpoint utiliza EPPlus para generar el archivo Excel
- Se configura para uso no comercial
- Incluye validaciones para datos nulos
- Maneja múltiples compañías a través del parámetro `companyCode`
- El archivo incluye información completa de la solicitud de entrada y sus relaciones
