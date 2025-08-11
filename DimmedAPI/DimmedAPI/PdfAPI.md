# API de Generación de PDFs

Esta API permite generar PDFs de remisiones para solicitudes de entrada, manteniendo la funcionalidad multicompañía.

## Endpoints Disponibles

### 1. Generar PDF de Remisión (GET)

Genera un PDF de remisión para una solicitud de entrada específica.

**URL:** `GET /api/Pdf/remision/{id}`

**Parámetros de Ruta:**
- `id` (int): ID de la solicitud de entrada

**Parámetros de Consulta:**
- `companyCode` (string, requerido): Código de la compañía
- `lot` (int, opcional): Imprimir lote (1: sí, 0: no). Default: 1
- `price` (int, opcional): Imprimir precio (1: sí, 0: no). Default: 1
- `code` (int, opcional): Imprimir código corto (1: sí, 0: no). Default: 0
- `duedate` (int, opcional): Imprimir fecha de vencimiento (1: sí, 0: no). Default: 1
- `option` (int, opcional): Imprimir solo lo despachado (1: sí, 0: no). Default: 0
- `regSan` (int, opcional): Imprimir registro sanitario (1: sí, 0: no). Default: 1

**Respuesta:**
- **200 OK**: Archivo PDF generado exitosamente
- **400 Bad Request**: Código de compañía faltante
- **404 Not Found**: Solicitud de entrada no encontrada
- **500 Internal Server Error**: Error interno del servidor

**Ejemplo de Uso:**
```http
GET /api/Pdf/remision/123?companyCode=COMP001&lot=1&price=1&code=0&duedate=1&option=0&regSan=1
```

### 2. Generar PDF de Remisión (POST)

Genera un PDF de remisión para una solicitud de entrada específica usando POST.

**URL:** `POST /api/Pdf/remision`

**Cuerpo de la Solicitud:**
```json
{
  "entryRequestId": 123,
  "companyCode": "COMP001",
  "lot": 1,
  "price": 1,
  "code": 0,
  "dueDate": 1,
  "option": 0,
  "regSan": 1
}
```

**Parámetros del Cuerpo:**
- `entryRequestId` (int, requerido): ID de la solicitud de entrada
- `companyCode` (string, requerido): Código de la compañía
- `lot` (int, opcional): Imprimir lote (1: sí, 0: no). Default: 1
- `price` (int, opcional): Imprimir precio (1: sí, 0: no). Default: 1
- `code` (int, opcional): Imprimir código corto (1: sí, 0: no). Default: 0
- `dueDate` (int, opcional): Imprimir fecha de vencimiento (1: sí, 0: no). Default: 1
- `option` (int, opcional): Imprimir solo lo despachado (1: sí, 0: no). Default: 0
- `regSan` (int, opcional): Imprimir registro sanitario (1: sí, 0: no). Default: 1

**Respuesta:**
- **200 OK**: Archivo PDF generado exitosamente
- **400 Bad Request**: Código de compañía faltante
- **404 Not Found**: Solicitud de entrada no encontrada
- **500 Internal Server Error**: Error interno del servidor

**Ejemplo de Uso:**
```http
POST /api/Pdf/remision
Content-Type: application/json

{
  "entryRequestId": 123,
  "companyCode": "COMP001",
  "lot": 1,
  "price": 1,
  "code": 0,
  "dueDate": 1,
  "option": 0,
  "regSan": 1
}
```

## Descripción de Parámetros

### Parámetros de Impresión

- **lot**: Controla si se imprime la información de lote en el PDF
  - `1`: Imprimir información de lote
  - `0`: No imprimir información de lote

- **price**: Controla si se imprime la información de precios
  - `1`: Imprimir precios unitarios e IVA
  - `0`: No imprimir información de precios

- **code**: Controla qué código se muestra para los componentes
  - `0`: Mostrar código completo
  - `3`: Mostrar descripción corta

- **duedate**: Controla si se imprime la fecha de vencimiento
  - `1`: Imprimir fecha de vencimiento
  - `0`: No imprimir fecha de vencimiento

- **option**: Controla qué elementos se incluyen en el PDF
  - `0`: Incluir todos los elementos despachados
  - `1`: Incluir solo elementos despachados en los últimos 10 minutos

- **regSan**: Controla si se imprime información de registro sanitario
  - `1`: Imprimir fecha de vencimiento y clasificación de riesgo del registro sanitario
  - `0`: No imprimir información de registro sanitario

## Características del PDF Generado

El PDF generado incluye:

1. **Encabezado**: Logo de la empresa, título del documento y número de remisión
2. **Información del Cliente**: Nombre, NIT, dirección, teléfono, orden de compra, etc.
3. **Información del Paciente**: Nombre, identificación, historia clínica, médico, ATC, etc.
4. **Observaciones**: Observaciones generales de la solicitud
5. **Detalles de Componentes**: Lista de componentes con cantidades, lotes, fechas de vencimiento, etc.
6. **Detalles de Equipos**: Lista de equipos principales con información de cajas
7. **Componentes Sin Stock**: Componentes que no tienen stock disponible
8. **Pie de Página**: Fecha de impresión y numeración de páginas

## Archivos de Plantilla

El sistema utiliza plantillas HTML ubicadas en:
- **Plantilla Principal**: `/wwwroot/Format/htmlremision.html`
- **Logo de Empresa**: `/wwwroot/template/img/logo.png`

## Consideraciones Técnicas

- **Multicompañía**: El endpoint es completamente multicompañía, utilizando el `companyCode` para conectarse a la base de datos correspondiente
- **Caché**: No se aplica caché a la generación de PDFs para garantizar datos actualizados
- **Formato**: Los PDFs se generan en formato A4 con orientación vertical
- **Codificación**: Se utiliza UTF-8 para garantizar caracteres especiales correctos
- **Dependencias**: Utiliza la librería DinkToPdf para la conversión de HTML a PDF

## Manejo de Errores

El sistema maneja los siguientes tipos de errores:

- **Validación**: Verifica que el código de compañía esté presente
- **Existencia**: Verifica que la solicitud de entrada exista
- **Conexión**: Maneja errores de conexión a la base de datos
- **Generación**: Maneja errores durante la generación del PDF
- **Archivos**: Verifica la existencia de plantillas y recursos necesarios

## Ejemplos de Respuesta

### Respuesta Exitosa
```http
HTTP/1.1 200 OK
Content-Type: application/pdf
Content-Disposition: attachment; filename="Remision_P-123_20250809_101500.pdf"
Content-Length: 245760

[Contenido del PDF en bytes]
```

### Error de Validación
```http
HTTP/1.1 400 Bad Request
Content-Type: application/json

"El código de compañía es requerido"
```

### Recurso No Encontrado
```http
HTTP/1.1 404 Not Found
Content-Type: application/json

"No se encontró la solicitud de entrada con ID 999"
```

### Error Interno
```http
HTTP/1.1 500 Internal Server Error
Content-Type: application/json

"Error interno del servidor: Error al generar el PDF: No se encontró la plantilla HTML"
``` 