# Employee API Documentation

## Overview
The Employee API provides comprehensive CRUD operations for managing employee data in the system. It supports multi-tenant architecture through company-specific database connections.

## Base URL
```
/api/Employee
```

## Authentication
All endpoints require a `companyCode` query parameter to identify the target company database.

## Endpoints

### 1. Get All Employees
**GET** `/api/Employee`

Retrieves all employees from the specified company database.

**Query Parameters:**
- `companyCode` (required): Company identifier

**Response:**
```json
[
  {
    "id": 1,
    "code": "EMP001",
    "name": "Juan Pérez",
    "charge": "Vendedor",
    "systemIdBC": "BC001",
    "atc": true,
    "mResponsible": false,
    "phone": "+1234567890",
    "email": "juan.perez@company.com",
    "branch": "Sucursal Centro"
  }
]
```

**Error Codes:**
- `400`: Company code is required
- `404`: Company not found
- `500`: Internal server error

---

### 2. Get Employees with Quotations Count
**GET** `/api/Employee/with-quotations-count`

Retrieves all employees with their associated quotations count.

**Query Parameters:**
- `companyCode` (required): Company identifier

**Response:**
```json
[
  {
    "id": 1,
    "code": "EMP001",
    "name": "Juan Pérez",
    "charge": "Vendedor",
    "systemIdBC": "BC001",
    "atc": true,
    "mResponsible": false,
    "phone": "+1234567890",
    "email": "juan.perez@company.com",
    "branch": "Sucursal Centro",
    "quotationsCount": 15
  }
]
```

---

### 3. Get Employees Statistics
**GET** `/api/Employee/statistics`

Retrieves detailed statistics for all employees including quotation metrics.

**Query Parameters:**
- `companyCode` (required): Company identifier

**Response:**
```json
[
  {
    "id": 1,
    "name": "Juan Pérez",
    "charge": "Vendedor",
    "branch": "Sucursal Centro",
    "totalQuotations": 15,
    "activeQuotations": 8,
    "inactiveQuotations": 7,
    "totalQuotationValue": 150000.00,
    "activeQuotationValue": 120000.00,
    "averageQuotationValue": 10000.00,
    "lastQuotationDate": "2024-01-15T10:30:00Z",
    "lastActiveQuotationDate": "2024-01-10T10:30:00Z"
  }
]
```

---

### 4. Get Employee by ID
**GET** `/api/Employee/{id}`

Retrieves a specific employee by their ID.

**Path Parameters:**
- `id` (required): Employee ID

**Query Parameters:**
- `companyCode` (required): Company identifier

**Response:**
```json
{
  "id": 1,
  "code": "EMP001",
  "name": "Juan Pérez",
  "charge": "Vendedor",
  "systemIdBC": "BC001",
  "atc": true,
  "mResponsible": false,
  "phone": "+1234567890",
  "email": "juan.perez@company.com",
  "branch": "Sucursal Centro"
}
```

**Error Codes:**
- `404`: Employee not found

---

### 5. Get Employees by Name
**GET** `/api/Employee/by-name/{name}`

Searches for employees by name (partial match).

**Path Parameters:**
- `name` (required): Employee name to search for

**Query Parameters:**
- `companyCode` (required): Company identifier

**Response:**
```json
[
  {
    "id": 1,
    "code": "EMP001",
    "name": "Juan Pérez",
    "charge": "Vendedor",
    "systemIdBC": "BC001",
    "atc": true,
    "mResponsible": false,
    "phone": "+1234567890",
    "email": "juan.perez@company.com",
    "branch": "Sucursal Centro"
  }
]
```

---

### 6. Get Employees by Branch
**GET** `/api/Employee/by-branch/{branch}`

Retrieves all employees from a specific branch.

**Path Parameters:**
- `branch` (required): Branch name to filter by

**Query Parameters:**
- `companyCode` (required): Company identifier

**Response:**
```json
[
  {
    "id": 1,
    "code": "EMP001",
    "name": "Juan Pérez",
    "charge": "Vendedor",
    "systemIdBC": "BC001",
    "atc": true,
    "mResponsible": false,
    "phone": "+1234567890",
    "email": "juan.perez@company.com",
    "branch": "Sucursal Centro"
  }
]
```

---

### 7. Get ATC Employees
**GET** `/api/Employee/atc-employees`

Retrieves all employees with ATC (Authorized Technical Contact) status.

**Query Parameters:**
- `companyCode` (required): Company identifier

**Response:**
```json
[
  {
    "id": 1,
    "code": "EMP001",
    "name": "Juan Pérez",
    "charge": "Vendedor",
    "systemIdBC": "BC001",
    "atc": true,
    "mResponsible": false,
    "phone": "+1234567890",
    "email": "juan.perez@company.com",
    "branch": "Sucursal Centro"
  }
]
```

---

### 8. Get M-Responsible Employees
**GET** `/api/Employee/m-responsible-employees`

Retrieves all employees with M-Responsible (Medical Responsible) status.

**Query Parameters:**
- `companyCode` (required): Company identifier

**Response:**
```json
[
  {
    "id": 2,
    "code": "EMP002",
    "name": "María García",
    "charge": "Médico",
    "systemIdBC": "BC002",
    "atc": false,
    "mResponsible": true,
    "phone": "+1234567891",
    "email": "maria.garcia@company.com",
    "branch": "Sucursal Norte"
  }
]
```

---

### 9. Create Employee
**POST** `/api/Employee`

Creates a new employee.

**Query Parameters:**
- `companyCode` (required): Company identifier

**Request Body:**
```json
{
  "code": "EMP001",
  "name": "Juan Pérez",
  "charge": "Vendedor",
  "systemIdBC": "BC001",
  "atc": true,
  "mResponsible": false,
  "phone": "+1234567890",
  "email": "juan.perez@company.com",
  "branch": "Sucursal Centro"
}
```

**Validation Rules:**
- `name`: Required, max 200 characters
- `code`: Optional, max 50 characters, must be unique
- `charge`: Optional, max 100 characters
- `systemIdBC`: Optional, max 50 characters, must be unique
- `phone`: Optional, max 20 characters, must be valid phone format
- `email`: Optional, max 100 characters, must be valid email format
- `branch`: Optional, max 100 characters

**Response:**
```json
{
  "id": 1,
  "code": "EMP001",
  "name": "Juan Pérez",
  "charge": "Vendedor",
  "systemIdBC": "BC001",
  "atc": true,
  "mResponsible": false,
  "phone": "+1234567890",
  "email": "juan.perez@company.com",
  "branch": "Sucursal Centro"
}
```

**Error Codes:**
- `400`: Validation error or duplicate code/SystemIdBC
- `201`: Employee created successfully

---

### 10. Update Employee
**PUT** `/api/Employee/{id}`

Updates an existing employee.

**Path Parameters:**
- `id` (required): Employee ID

**Query Parameters:**
- `companyCode` (required): Company identifier

**Request Body:**
```json
{
  "code": "EMP001-UPDATED",
  "name": "Juan Pérez",
  "charge": "Vendedor Senior",
  "systemIdBC": "BC001",
  "atc": true,
  "mResponsible": false,
  "phone": "+1234567890",
  "email": "juan.perez@company.com",
  "branch": "Sucursal Centro"
}
```

**Response:**
- `204`: Employee updated successfully

**Error Codes:**
- `400`: Validation error or duplicate code/SystemIdBC
- `404`: Employee not found

---

### 11. Delete Employee
**DELETE** `/api/Employee/{id}`

Deletes an employee (only if they have no associated quotations).

**Path Parameters:**
- `id` (required): Employee ID

**Query Parameters:**
- `companyCode` (required): Company identifier

**Response:**
- `204`: Employee deleted successfully

**Error Codes:**
- `400`: Employee has associated quotations
- `404`: Employee not found

---

### 12. Get Employee Quotations
**GET** `/api/Employee/{id}/quotations`

Retrieves all quotations associated with a specific employee.

**Path Parameters:**
- `id` (required): Employee ID

**Query Parameters:**
- `companyCode` (required): Company identifier

**Response:**
```json
[
  {
    "id": 1,
    "idCustomer": 1,
    "creationDateTime": "2024-01-15T10:30:00Z",
    "dueDate": "2024-01-30T10:30:00Z",
    "total": 15000.00,
    "isActive": true,
    "totalizingQuotation": false,
    "equipmentRemains": true
  }
]
```

---

### 12.1. Get Employee Active Quotations
**GET** `/api/Employee/{id}/active-quotations`

Retrieves only active quotations associated with a specific employee.

**Path Parameters:**
- `id` (required): Employee ID

**Query Parameters:**
- `companyCode` (required): Company identifier

**Response:**
```json
[
  {
    "id": 1,
    "idCustomer": 1,
    "creationDateTime": "2024-01-15T10:30:00Z",
    "dueDate": "2024-01-30T10:30:00Z",
    "total": 15000.00,
    "isActive": true,
    "totalizingQuotation": false,
    "equipmentRemains": true
  }
]
```

---

### 14. Get Employee with Quotations Details
**GET** `/api/Employee/{id}/quotations-with-details`

Retrieves an employee with their complete quotation details.

**Path Parameters:**
- `id` (required): Employee ID

**Query Parameters:**
- `companyCode` (required): Company identifier

**Response:**
```json
{
  "id": 1,
  "code": "EMP001",
  "name": "Juan Pérez",
  "charge": "Vendedor",
  "systemIdBC": "BC001",
  "atc": true,
  "mResponsible": false,
  "phone": "+1234567890",
  "email": "juan.perez@company.com",
  "branch": "Sucursal Centro",
  "quotations": [
    {
      "id": 1,
      "idCustomer": 1,
      "creationDateTime": "2024-01-15T10:30:00Z",
      "dueDate": "2024-01-30T10:30:00Z",
      "fk_idEmployee": 1,
      "totalizingQuotation": false,
      "equipmentRemains": true
    }
  ]
}
```

---

### 15. Verify Company Configuration
**GET** `/api/Employee/VerificarConfiguracionCompania`

Provides configuration status and employee statistics for the company.

**Query Parameters:**
- `companyCode` (required): Company identifier

**Response:**
```json
{
  "totalEmployees": 25,
  "atcEmployees": 5,
  "mResponsibleEmployees": 3,
  "configurationStatus": "Configurado"
}
```

---

## Special Features

### Caching
All GET endpoints implement output caching with the tag "employee" for improved performance.

### Multi-Tenant Support
All endpoints support multi-tenant architecture through dynamic database connections based on the company code.

### Validation
- Email format validation for email fields
- Phone format validation for phone fields
- Unique constraint validation for code and SystemIdBC fields
- Required field validation for name
- String length validation for all text fields

### Error Handling
Comprehensive error handling with appropriate HTTP status codes and descriptive error messages.

### Business Rules
- Employees cannot be deleted if they have associated quotations
- Code and SystemIdBC must be unique within the company
- ATC and MResponsible are boolean flags for special employee types
- Quotations now include an IsActive flag for status management
- Statistics distinguish between active and inactive quotations

## Data Types

### Employee Entity
```csharp
public class Employee
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string Name { get; set; }
    public string? Charge { get; set; }
    public string? SystemIdBC { get; set; }
    public bool? ATC { get; set; }
    public bool? MResponsible { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Branch { get; set; }
}
```

### QuotationMaster Entity (Updated)
```csharp
public class QuotationMaster
{
    public int Id { get; set; }
    public int FK_idBranch { get; set; }
    public char? CustomerOrigin { get; set; }
    public int FK_idCustomerType { get; set; }
    public int IdCustomer { get; set; }
    public DateTime? CreationDateTime { get; set; }
    public DateTime? DueDate { get; set; }
    public int FK_idEmployee { get; set; }
    public int? FK_QuotationTypeId { get; set; }
    public int? PaymentTerm { get; set; }
    public int? FK_CommercialConditionId { get; set; }
    public bool? TotalizingQuotation { get; set; }
    public double? Total { get; set; }
    public bool? EquipmentRemains { get; set; }
    public double? MonthlyConsumption { get; set; }
    public bool IsActive { get; set; } = true;
}
```

### DTOs
- `EmployeeCreateDTO`: For creating new employees
- `EmployeeUpdateDTO`: For updating existing employees
- `EmployeeResponseDTO`: For responses with quotation count
- `EmployeeWithQuotationsDTO`: For responses with detailed quotations
- `EmployeeStatisticsDTO`: For statistical information 