# CustomerContact API

Este controlador maneja la sincronización y gestión de contactos de clientes con Business Central en un sistema multitenant.

## Endpoints Disponibles

### 1. Sincronizar Contacto de Cliente desde Business Central
**GET** `/api/CustomerContact/sync/{companyCode}`

Sincroniza un contacto de cliente desde Business Central a la base de datos local.

**Parámetros:**
- `companyCode` (path): Código de la compañía
- `method` (query): Método de Business Central (ej: "lylcustomercontact")
- `systemID` (query): SystemId del registro en Business Central

**Ejemplo:**
```
GET /api/CustomerContact/sync/COMP001?method=lylcustomercontact&systemID=12345
```

**Respuesta:**
```json
{
  "message": "Contacto del cliente sincronizado exitosamente",
  "contact": {
    "id": 1,
    "code": "CUST001",
    "name": "Juan Pérez",
    "email": "juan.perez@email.com",
    "systemIdBC": "12345",
    "phone": "3001234567",
    "customerName": "Empresa ABC",
    "identification": "12345678"
  },
  "action": "created"
}
```

### 2. Obtener Todos los Contactos de una Compañía
**GET** `/api/CustomerContact/list/{companyCode}`

Obtiene todos los contactos de clientes almacenados localmente para una compañía específica.

**Parámetros:**
- `companyCode` (path): Código de la compañía

**Ejemplo:**
```
GET /api/CustomerContact/list/COMP001
```

### 3. Obtener Contacto Específico por ID
**GET** `/api/CustomerContact/detail/{companyCode}/{id}`

Obtiene un contacto específico por su ID.

**Parámetros:**
- `companyCode` (path): Código de la compañía
- `id` (path): ID del contacto

**Ejemplo:**
```
GET /api/CustomerContact/detail/COMP001/1
```

### 4. Buscar Contactos
**GET** `/api/CustomerContact/search/{companyCode}?searchTerm={term}`

Busca contactos por nombre o código.

**Parámetros:**
- `companyCode` (path): Código de la compañía
- `searchTerm` (query): Término de búsqueda

**Ejemplo:**
```
GET /api/CustomerContact/search/COMP001?searchTerm=Juan
```

### 5. Buscar Contactos por Nombre del Cliente
**GET** `/api/CustomerContact/searchByCustomerName/{companyCode}?customerName={name}`

Busca contactos por el nombre del cliente (customerName).

**Parámetros:**
- `companyCode` (path): Código de la compañía
- `customerName` (query): Nombre del cliente a buscar

**Ejemplo:**
```
GET /api/CustomerContact/searchByCustomerName/COMP001?customerName=CLINICA LA SABANA
```

**Respuesta:**
```json
[
  {
    "id": 14,
    "code": "CO000082",
    "name": "CLINICA LA SABANA S.A",
    "email": "feservicios@clinicalasabana.com",
    "systemIdBC": "2952ab56-7dcf-ed11-a7c9-002248e0ec87",
    "phone": "5716221",
    "customerName": "CLINICA LA SABANA S.A",
    "identification": "800017308"
  }
]
```

### 6. Eliminar Contacto
**DELETE** `/api/CustomerContact/delete/{companyCode}/{id}`

Elimina un contacto de cliente.

**Parámetros:**
- `companyCode` (path): Código de la compañía
- `id` (path): ID del contacto a eliminar

**Ejemplo:**
```
DELETE /api/CustomerContact/delete/COMP001/1
```

## Estructura de Datos

### CustomerContact
```json
{
  "id": 1,
  "code": "CUST001",
  "name": "Juan Pérez",
  "email": "juan.perez@email.com",
  "systemIdBC": "12345",
  "phone": "3001234567",
  "customerName": "Empresa ABC",
  "identification": "12345678"
}
```

## Características del Sistema

1. **Multitenant**: Cada endpoint requiere un código de compañía para acceder a la base de datos correcta
2. **Sincronización con Business Central**: El endpoint de sincronización conecta con Business Central usando las credenciales específicas de cada compañía
3. **Gestión Local**: Los contactos se almacenan localmente después de la sincronización para acceso rápido
4. **Actualización Automática**: Si un contacto ya existe, se actualiza con la información más reciente de Business Central

## Configuración Requerida

Asegúrate de que la compañía esté configurada en la tabla `Companies` con:
- `BCCodigoEmpresa`: Código de la compañía
- `BCURL`: URL de Business Central
- `BCURLWebService`: URL del web service de Business Central

También se requieren las credenciales de Azure AD configuradas en `appsettings.json`:
- `AzureAd:ClientId`
- `AzureAd:ClientSecret`
- `AzureAd:TenantId` 