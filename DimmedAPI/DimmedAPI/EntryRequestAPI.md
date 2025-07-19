# EntryRequest API Documentation

## Descripción
API para gestionar las solicitudes de entrada (EntryRequests) en el sistema multicompañía.

## Endpoints

### Obtener todas las solicitudes de entrada
- **GET** `/api/EntryRequest`
- **Query Parameters:**
  - `companyCode` (string, requerido): Código de la compañía

**Respuesta:**
```json
[
  {
    "id": 1,
    "date": "2024-01-15T10:30:00",
    "service": "Cirugía Ortopédica",
    "idOrderType": 1,
    "deliveryPriority": "Alta",
    "idCustomer": 1,
    "insurerType": 1,
    "insurer": 1,
    "idMedic": 1,
    "idPatient": 1,
    "applicant": "Dr. García",
    "idAtc": 1,
    "limbSide": "Derecha",
    "deliveryDate": "2024-01-16T08:00:00",
    "orderObs": "Observaciones del pedido",
    "surgeryTime": 120,
    "surgeryInit": "2024-01-16T07:00:00",
    "surgeryEnd": "2024-01-16T09:00:00",
    "status": "Pendiente",
    "idTraceStates": 1,
    "branchId": 1,
    "surgeryInitTime": "07:00:00",
    "surgeryEndTime": "09:00:00",
    "deliveryAddress": "Hospital Central",
    "purchaseOrder": "PO-001",
    "atcConsumed": false,
    "isSatisfied": null,
    "observations": "Observaciones generales",
    "obsMaint": "Observaciones de mantenimiento",
    "auxLog": 0,
    "idCancelReason": null,
    "idCancelDetail": null,
    "cancelReason": null,
    "cancelDetail": null,
    "notification": false,
    "isReplacement": false,
    "assemblyComponents": false,
    "priceGroup": "Grupo A"
  }
]
```

### Obtener solicitud de entrada por ID
- **GET** `/api/EntryRequest/{id}`
- **Path Parameters:**
  - `id` (int): ID de la solicitud de entrada
- **Query Parameters:**
  - `companyCode` (string, requerido): Código de la compañía

**Respuesta:**
```json
{
  "id": 1,
  "date": "2024-01-15T10:30:00",
  "service": "Cirugía Ortopédica",
  "idOrderType": 1,
  "deliveryPriority": "Alta",
  "idCustomer": 1,
  "insurerType": 1,
  "insurer": 1,
  "idMedic": 1,
  "idPatient": 1,
  "applicant": "Dr. García",
  "idAtc": 1,
  "limbSide": "Derecha",
  "deliveryDate": "2024-01-16T08:00:00",
  "orderObs": "Observaciones del pedido",
  "surgeryTime": 120,
  "surgeryInit": "2024-01-16T07:00:00",
  "surgeryEnd": "2024-01-16T09:00:00",
  "status": "Pendiente",
  "idTraceStates": 1,
  "branchId": 1,
  "surgeryInitTime": "07:00:00",
  "surgeryEndTime": "09:00:00",
  "deliveryAddress": "Hospital Central",
  "purchaseOrder": "PO-001",
  "atcConsumed": false,
  "isSatisfied": null,
  "observations": "Observaciones generales",
  "obsMaint": "Observaciones de mantenimiento",
  "auxLog": 0,
  "idCancelReason": null,
  "idCancelDetail": null,
  "cancelReason": null,
  "cancelDetail": null,
  "notification": false,
  "isReplacement": false,
  "assemblyComponents": false,
  "priceGroup": "Grupo A",
  "insurerNavigation": { ... },
  "insurerTypeNavigation": { ... },
  "branch": { ... },
  "idCustomerNavigation": { ... },
  "idMedicNavigation": { ... },
  "idPatientNavigation": { ... },
  "idTraceStatesNavigation": { ... },
  "idAtcNavigation": { ... },
  "entryRequestAssembly": [ ... ],
  "entryRequestComponents": [ ... ],
  "entryRequestDetails": [ ... ],
  "entryRequestHistory": [ ... ],
  "entryRequestTraceStates": [ ... ]
}
```

### Obtener solicitudes de entrada por cliente
- **GET** `/api/EntryRequest/by-customer/{customerId}`
- **Path Parameters:**
  - `customerId` (int): ID del cliente
- **Query Parameters:**
  - `companyCode` (string, requerido): Código de la compañía

### Obtener solicitudes de entrada por médico
- **GET** `/api/EntryRequest/by-medic/{medicId}`
- **Path Parameters:**
  - `medicId` (int): ID del médico
- **Query Parameters:**
  - `companyCode` (string, requerido): Código de la compañía

### Obtener solicitudes de entrada por paciente
- **GET** `/api/EntryRequest/by-patient/{patientId}`
- **Path Parameters:**
  - `patientId` (int): ID del paciente
- **Query Parameters:**
  - `companyCode` (string, requerido): Código de la compañía

### Obtener solicitudes de entrada por estado
- **GET** `/api/EntryRequest/by-status/{status}`
- **Path Parameters:**
  - `status` (string): Estado de la solicitud
- **Query Parameters:**
  - `companyCode` (string, requerido): Código de la compañía

### Crear nueva solicitud de entrada
- **POST** `/api/EntryRequest`
- **Query Parameters:**
  - `companyCode` (string, requerido): Código de la compañía
- **Body:**
```json
{
  "date": "2024-01-15T10:30:00",
  "service": "Cirugía Ortopédica",
  "idOrderType": 1,
  "deliveryPriority": "Alta",
  "idCustomer": 1,
  "insurerType": 1,
  "insurer": 1,
  "idMedic": 1,
  "idPatient": 1,
  "applicant": "Dr. García",
  "idAtc": 1,
  "limbSide": "Derecha",
  "deliveryDate": "2024-01-16T08:00:00",
  "orderObs": "Observaciones del pedido",
  "surgeryTime": 120,
  "surgeryInit": "2024-01-16T07:00:00",
  "surgeryEnd": "2024-01-16T09:00:00",
  "status": "Pendiente",
  "idTraceStates": 1,
  "branchId": 1,
  "surgeryInitTime": "07:00:00",
  "surgeryEndTime": "09:00:00",
  "deliveryAddress": "Hospital Central",
  "purchaseOrder": "PO-001",
  "atcConsumed": false,
  "isSatisfied": null,
  "observations": "Observaciones generales",
  "obsMaint": "Observaciones de mantenimiento",
  "auxLog": 0,
  "idCancelReason": null,
  "idCancelDetail": null,
  "cancelReason": null,
  "cancelDetail": null,
  "notification": false,
  "isReplacement": false,
  "assemblyComponents": false,
  "priceGroup": "Grupo A"
}
```

### Actualizar solicitud de entrada
- **PUT** `/api/EntryRequest/{id}`
- **Path Parameters:**
  - `id` (int): ID de la solicitud de entrada
- **Query Parameters:**
  - `companyCode` (string, requerido): Código de la compañía
- **Body:** Mismo formato que en la creación

### Eliminar solicitud de entrada
- **DELETE** `/api/EntryRequest/{id}`
- **Path Parameters:**
  - `id` (int): ID de la solicitud de entrada
- **Query Parameters:**
  - `companyCode` (string, requerido): Código de la compañía

### Verificar configuración de compañía
- **GET** `/api/EntryRequest/VerificarConfiguracionCompania`
- **Query Parameters:**
  - `companyCode` (string, requerido): Código de la compañía

**Respuesta:**
```json
{
  "company": {
    "id": 1,
    "businessName": "Empresa Ejemplo",
    "bCCodigoEmpresa": "EMP001",
    "sqlConnectionString": "Server=...;Database=...;"
  },
  "businessCentral": {
    "urlWS": "https://api.businesscentral.dynamics.com/...",
    "url": "https://businesscentral.dynamics.com/...",
    "company": "EMP001"
  }
}
```

## Códigos de Error

- **400 Bad Request**: Parámetros faltantes o inválidos
- **404 Not Found**: Recurso no encontrado
- **500 Internal Server Error**: Error interno del servidor

## Notas

- Todos los endpoints requieren el parámetro `companyCode` para identificar la base de datos específica de la compañía
- Las respuestas incluyen las entidades relacionadas cuando se solicitan con `Include`
- El sistema utiliza caché para mejorar el rendimiento de las consultas de lectura
- Las operaciones de escritura invalidan automáticamente el caché relacionado 