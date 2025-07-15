# Company API Documentation

## Overview
La API de Company permite gestionar las compañías del sistema, incluyendo su información básica, configuración de Business Central y parámetros de autenticación.

## Base URL
```
https://localhost:7063/api/companies
```

## Endpoints

### 1. Obtener todas las compañías
**GET** `/api/companies/ObtenerCompanies`

Obtiene la lista completa de todas las compañías registradas en el sistema.

#### Response
- **200 OK**: Lista de compañías con información completa
- **500 Internal Server Error**: Error interno del servidor

#### Example Response
```json
[
  {
    "id": 1,
    "identificationTypeId": 2,
    "identificationNumber": "811041784",
    "businessName": "DINAMISMO MEDICO SAS",
    "tradeName": "DINMED",
    "mainAddress": "CALLE 66 A 43 02 BG 106 107",
    "department": "ANTIOQUIA",
    "city": "ITAGUI",
    "legalRepresentative": "",
    "contactEmail": "",
    "contactPhone": "3146617415",
    "sqlConnectionString": "Server=tcp:acuaman.database.windows.net,1433;Database=Dinmed;...",
    "bcurlWebService": "https://api.businesscentral.dynamics.com/v2.0/...",
    "bcurl": "https://api.businesscentral.dynamics.com/v2.0/...",
    "bcCodigoEmpresa": "093ca381-26c4-ed11-9a88-002248e00201",
    "createdBy": 2,
    "createdAt": "2025-05-20T10:34:11.0133333",
    "modifiedBy": null,
    "modifiedAt": null,
    "logoCompany": "",
    "instancia": null,
    "dominio": null,
    "clienteid": null,
    "tenantid": "a9643f6b-1667-4b68-a478-633546000bae",
    "clientsecret": null,
    "callbackpath": null,
    "correonotificacion": null,
    "nombrenotificacion": null,
    "pwdnotificacion": null,
    "smtpserver": null,
    "puertosmtp": "587",
    "identificationType": {
      "id": 2,
      "name": "NIT"
    }
  }
]
```

---

### 2. Obtener compañía por código
**GET** `/api/companies/ObtenerCompanyPorCodigo`

Obtiene una compañía específica por su código de empresa de Business Central.

#### Query Parameters
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| companyCode | string | Yes | Código de la empresa en Business Central |

#### Response
- **200 OK**: Información de la compañía
- **400 Bad Request**: Código de compañía requerido
- **404 Not Found**: Compañía no encontrada
- **500 Internal Server Error**: Error interno del servidor

#### Example Request
```bash
curl -X 'GET' \
  'https://localhost:7063/api/companies/ObtenerCompanyPorCodigo?companyCode=093ca381-26c4-ed11-9a88-002248e00201'
```

---

### 3. Verificar configuración de compañía
**GET** `/api/companies/VerificarConfiguracionCompania`

Verifica la configuración completa de una compañía, incluyendo información básica y configuración de Business Central.

#### Query Parameters
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| companyCode | string | Yes | Código de la empresa en Business Central |

#### Response
- **200 OK**: Configuración completa de la compañía
- **400 Bad Request**: Código de compañía requerido
- **404 Not Found**: Compañía no encontrada
- **500 Internal Server Error**: Error interno del servidor

#### Example Response
```json
{
  "company": {
    "id": 1,
    "businessName": "DINAMISMO MEDICO SAS",
    "bcCodigoEmpresa": "093ca381-26c4-ed11-9a88-002248e00201",
    "sqlConnectionString": "Server=tcp:acuaman.database.windows.net,1433;Database=Dinmed;..."
  },
  "businessCentral": {
    "urlWS": "https://api.businesscentral.dynamics.com/v2.0/...",
    "url": "https://api.businesscentral.dynamics.com/v2.0/...",
    "company": "093ca381-26c4-ed11-9a88-002248e00201"
  }
}
```

---

### 4. Obtener compañía por ID
**GET** `/api/companies/{id}`

Obtiene una compañía específica por su ID en la base de datos.

#### Path Parameters
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| id | integer | Yes | ID de la compañía |

#### Response
- **200 OK**: Información de la compañía
- **400 Bad Request**: ID inválido
- **404 Not Found**: Compañía no encontrada
- **500 Internal Server Error**: Error interno del servidor

#### Example Request
```bash
curl -X 'GET' \
  'https://localhost:7063/api/companies/1'
```

---

### 5. Crear nueva compañía
**POST** `/api/companies`

Crea una nueva compañía en el sistema.

#### Request Body
```json
{
  "identificationTypeId": 2,
  "identificationNumber": "900123456",
  "businessName": "NUEVA EMPRESA SAS",
  "tradeName": "NUEVA",
  "mainAddress": "CALLE 123 # 45-67",
  "department": "ANTIOQUIA",
  "city": "MEDELLIN",
  "legalRepresentative": "Juan Pérez",
  "contactEmail": "contacto@nuevaempresa.com",
  "contactPhone": "3001234567",
  "sqlConnectionString": "Server=mi-servidor;Database=mi-db;User ID=usuario;Password=clave;",
  "bcurlWebService": "https://api.businesscentral.dynamics.com/v2.0/tenant-id/environment/ODataV4/",
  "bcurl": "https://api.businesscentral.dynamics.com/v2.0/tenant-id/environment/api/company/v1.0/",
  "bcCodigoEmpresa": "company-guid",
  "createdBy": 1,
  "logoCompany": "",
  "instancia": null,
  "dominio": null,
  "clienteid": null,
  "tenantid": "tenant-guid",
  "clientsecret": null,
  "callbackpath": null,
  "correonotificacion": null,
  "nombrenotificacion": null,
  "pwdnotificacion": null,
  "smtpserver": null,
  "puertosmtp": "587"
}
```

#### Campos Requeridos
- `identificationTypeId`: ID del tipo de identificación
- `identificationNumber`: Número de identificación
- `businessName`: Nombre de la empresa
- `sqlConnectionString`: Cadena de conexión a la base de datos
- `bcurlWebService`: URL del servicio web de Business Central
- `bcurl`: URL de la API de Business Central
- `bcCodigoEmpresa`: Código de la empresa en Business Central

#### Response
- **201 Created**: Compañía creada exitosamente
- **400 Bad Request**: Error de validación o compañía duplicada
- **500 Internal Server Error**: Error interno del servidor

#### Example Request
```bash
curl -X 'POST' \
  'https://localhost:7063/api/companies' \
  -H 'accept: */*' \
  -H 'Content-Type: application/json' \
  -d '{
    "identificationTypeId": 2,
    "identificationNumber": "900123456",
    "businessName": "NUEVA EMPRESA SAS",
    "tradeName": "NUEVA",
    "mainAddress": "CALLE 123 # 45-67",
    "department": "ANTIOQUIA",
    "city": "MEDELLIN",
    "legalRepresentative": "Juan Pérez",
    "contactEmail": "contacto@nuevaempresa.com",
    "contactPhone": "3001234567",
    "sqlConnectionString": "Server=mi-servidor;Database=mi-db;User ID=usuario;Password=clave;",
    "bcurlWebService": "https://api.businesscentral.dynamics.com/v2.0/tenant-id/environment/ODataV4/",
    "bcurl": "https://api.businesscentral.dynamics.com/v2.0/tenant-id/environment/api/company/v1.0/",
    "bcCodigoEmpresa": "company-guid",
    "createdBy": 1,
    "logoCompany": "",
    "tenantid": "tenant-guid",
    "puertosmtp": "587"
  }'
```

---

### 6. Actualizar compañía
**PUT** `/api/companies/{id}`

Actualiza la información de una compañía existente.

#### Path Parameters
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| id | integer | Yes | ID de la compañía a actualizar |

#### Request Body
```json
{
  "identificationTypeId": 2,
  "identificationNumber": "811041784",
  "businessName": "DINAMISMO MEDICO SAS",
  "tradeName": "DINMED",
  "mainAddress": "CALLE 66 A 43 02 BG 106 107",
  "department": "ANTIOQUIA",
  "city": "ITAGUI",
  "legalRepresentative": "",
  "contactEmail": "",
  "contactPhone": "3146617415",
  "sqlConnectionString": "Server=tcp:acuaman.database.windows.net,1433;Database=Dinmed;Integrated Security=False;User ID=lylAdmin;Password=Abc-12345;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;",
  "bcurlWebService": "https://api.businesscentral.dynamics.com/v2.0/a9643f6b-1667-4b68-a478-633546000bae/Sandbox150425/ODataV4/",
  "bcurl": "https://api.businesscentral.dynamics.com/v2.0/a9643f6b-1667-4b68-a478-633546000bae/Sandbox150425/api/lylconsultores/sheldportal/v1.0/companies(093ca381-26c4-ed11-9a88-002248e00201)/",
  "bcCodigoEmpresa": "093ca381-26c4-ed11-9a88-002248e00201",
  "modifiedBy": null,
  "logoCompany": "",
  "instancia": null,
  "dominio": null,
  "clienteid": null,
  "tenantid": "a9643f6b-1667-4b68-a478-633546000bae",
  "clientsecret": null,
  "callbackpath": null,
  "correonotificacion": null,
  "nombrenotificacion": null,
  "pwdnotificacion": null,
  "smtpserver": null,
  "puertosmtp": "587"
}
```

#### Response
- **200 OK**: Compañía actualizada exitosamente
- **400 Bad Request**: Error de validación
- **404 Not Found**: Compañía no encontrada
- **500 Internal Server Error**: Error interno del servidor

#### Example Request
```bash
curl -X 'PUT' \
  'https://localhost:7063/api/companies/2' \
  -H 'accept: */*' \
  -H 'Content-Type: application/json' \
  -d '{
    "identificationTypeId": 2,
    "identificationNumber": "811041784",
    "businessName": "DINAMISMO MEDICO SAS",
    "tradeName": "DINMED",
    "mainAddress": "CALLE 66 A 43 02 BG 106 107",
    "department": "ANTIOQUIA",
    "city": "ITAGUI",
    "legalRepresentative": "",
    "contactEmail": "",
    "contactPhone": "3146617415",
    "sqlConnectionString": "Server=tcp:acuaman.database.windows.net,1433;Database=Dinmed;Integrated Security=False;User ID=lylAdmin;Password=Abc-12345;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;",
    "bcurlWebService": "https://api.businesscentral.dynamics.com/v2.0/a9643f6b-1667-4b68-a478-633546000bae/Sandbox150425/ODataV4/",
    "bcurl": "https://api.businesscentral.dynamics.com/v2.0/a9643f6b-1667-4b68-a478-633546000bae/Sandbox150425/api/lylconsultores/sheldportal/v1.0/companies(093ca381-26c4-ed11-9a88-002248e00201)/",
    "bcCodigoEmpresa": "093ca381-26c4-ed11-9a88-002248e00201",
    "modifiedBy": null,
    "logoCompany": "",
    "instancia": null,
    "dominio": null,
    "clienteid": null,
    "tenantid": "a9643f6b-1667-4b68-a478-633546000bae",
    "clientsecret": null,
    "callbackpath": null,
    "correonotificacion": null,
    "nombrenotificacion": null,
    "pwdnotificacion": null,
    "smtpserver": null,
    "puertosmtp": "587"
  }'
```

---

### 7. Subir logo de compañía
**POST** `/api/companies/upload-logo`

Sube el logo de una compañía específica.

#### Query Parameters
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| companyId | integer | Yes | ID de la compañía |

#### Form Data
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| file | file | Yes | Archivo de imagen del logo |

#### Tipos de archivo permitidos
- JPG, JPEG, PNG, GIF, BMP, SVG
- Tamaño máximo: 5MB

#### Response
- **200 OK**: Logo subido exitosamente
- **400 Bad Request**: Error de validación (archivo no proporcionado, tipo no permitido, tamaño excesivo)
- **404 Not Found**: Compañía no encontrada
- **500 Internal Server Error**: Error interno del servidor

#### Example Request
```bash
curl -X 'POST' \
  'https://localhost:7063/api/companies/upload-logo?companyId=1' \
  -H 'accept: */*' \
  -H 'Content-Type: multipart/form-data' \
  -F 'file=@/path/to/logo.png'
```

#### Example Response
```json
{
  "success": true,
  "message": "Logo subido exitosamente",
  "logoUrl": "/uploads/logos/logo_1_20250520123456.png",
  "fileName": "logo_1_20250520123456.png",
  "fileSize": 102400,
  "contentType": "image/png"
}
```

---

### 8. Eliminar compañía
**DELETE** `/api/companies/{id}`

Elimina una compañía del sistema.

#### Path Parameters
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| id | integer | Yes | ID de la compañía a eliminar |

#### Response
- **204 No Content**: Compañía eliminada exitosamente
- **404 Not Found**: Compañía no encontrada
- **500 Internal Server Error**: Error interno del servidor

#### Example Request
```bash
curl -X 'DELETE' \
  'https://localhost:7063/api/companies/1'
```

---

## Modelos de Datos

### Company Entity
```csharp
public class Companies
{
    public int Id { get; set; }
    public int IdentificationTypeId { get; set; }
    public required string IdentificationNumber { get; set; }
    public required string BusinessName { get; set; }
    public string? TradeName { get; set; }
    public string? MainAddress { get; set; }
    public string? Department { get; set; }
    public string? City { get; set; }
    public string? LegalRepresentative { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public required string SqlConnectionString { get; set; }
    public required string BCURLWebService { get; set; }
    public required string BCURL { get; set; }
    public required string BCCodigoEmpresa { get; set; }
    public int? CreatedBy { get; set; }
    public DateTime? CreatedAt { get; set; }
    public int? ModifiedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public string? logoCompany { get; set; }
    public string? instancia { get; set; }
    public string? dominio { get; set; }
    public string? clienteid { get; set; }
    public string? tenantid { get; set; }
    public string? clientsecret { get; set; }
    public string? callbackpath { get; set; }
    public string? correonotificacion { get; set; }
    public string? nombrenotificacion { get; set; }
    public string? pwdnotificacion { get; set; }
    public string? smtpserver { get; set; }
    public string? puertosmtp { get; set; }
    public IdentificationTypes? IdentificationType { get; set; }
    public AppUser? CreatedByUser { get; set; }
    public AppUser? ModifiedByUser { get; set; }
}
```

### CompanyCreateDTO
```csharp
public class CompanyCreateDTO
{
    public int IdentificationTypeId { get; set; }
    public required string IdentificationNumber { get; set; }
    public required string BusinessName { get; set; }
    public string? TradeName { get; set; }
    public string? MainAddress { get; set; }
    public string? Department { get; set; }
    public string? City { get; set; }
    public string? LegalRepresentative { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public required string SqlConnectionString { get; set; }
    public required string BCURLWebService { get; set; }
    public required string BCURL { get; set; }
    public required string BCCodigoEmpresa { get; set; }
    public string? logoCompany { get; set; }
    public string? instancia { get; set; }
    public string? dominio { get; set; }
    public string? clienteid { get; set; }
    public string? tenantid { get; set; }
    public string? clientsecret { get; set; }
    public string? callbackpath { get; set; }
    public string? correonotificacion { get; set; }
    public string? nombrenotificacion { get; set; }
    public string? pwdnotificacion { get; set; }
    public string? smtpserver { get; set; }
    public string? puertosmtp { get; set; }
    public int? CreatedBy { get; set; }
}
```

### CompanyUpdateDTO
```csharp
public class CompanyUpdateDTO
{
    public int IdentificationTypeId { get; set; }
    public required string IdentificationNumber { get; set; }
    public required string BusinessName { get; set; }
    public string? TradeName { get; set; }
    public string? MainAddress { get; set; }
    public string? Department { get; set; }
    public string? City { get; set; }
    public string? LegalRepresentative { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public required string SqlConnectionString { get; set; }
    public required string BCURLWebService { get; set; }
    public required string BCURL { get; set; }
    public required string BCCodigoEmpresa { get; set; }
    public string? logoCompany { get; set; }
    public string? instancia { get; set; }
    public string? dominio { get; set; }
    public string? clienteid { get; set; }
    public string? tenantid { get; set; }
    public string? clientsecret { get; set; }
    public string? callbackpath { get; set; }
    public string? correonotificacion { get; set; }
    public string? nombrenotificacion { get; set; }
    public string? pwdnotificacion { get; set; }
    public string? smtpserver { get; set; }
    public string? puertosmtp { get; set; }
    public int? ModifiedBy { get; set; }
}
```

### LogoUploadResponseDTO
```csharp
public class LogoUploadResponseDTO
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public string? LogoUrl { get; set; }
    public string? FileName { get; set; }
    public long FileSize { get; set; }
    public string? ContentType { get; set; }
}
```

---

## Códigos de Error

| Código | Descripción |
|--------|-------------|
| 200 | OK - Operación exitosa |
| 201 | Created - Recurso creado exitosamente |
| 204 | No Content - Recurso eliminado exitosamente |
| 400 | Bad Request - Error de validación o datos incorrectos |
| 404 | Not Found - Recurso no encontrado |
| 500 | Internal Server Error - Error interno del servidor |

---

## Notas Importantes

1. **Cache**: Los endpoints de lectura utilizan cache para mejorar el rendimiento. El cache se invalida automáticamente cuando se realizan operaciones de escritura.

2. **Validaciones**: 
   - Se valida que el tipo de identificación exista antes de crear/actualizar
   - Se verifica que no existan duplicados por número de identificación
   - Los campos requeridos son obligatorios

3. **Auditoría**: 
   - `CreatedAt` y `CreatedBy` se establecen automáticamente al crear
   - `ModifiedAt` se actualiza automáticamente al modificar
   - `ModifiedBy` debe ser proporcionado en las actualizaciones

4. **Business Central**: Los endpoints específicos de BC utilizan el servicio de conexión dinámica para obtener información de diferentes entornos.

5. **Seguridad**: Las cadenas de conexión y secretos se almacenan en la base de datos. Asegúrate de que estén encriptados en producción.

6. **Archivos**: Los logos se almacenan en la carpeta `wwwroot/uploads/logos/` y son accesibles públicamente. En producción, considera usar un servicio de almacenamiento en la nube como Azure Blob Storage. 