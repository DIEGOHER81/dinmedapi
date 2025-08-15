# Diagrama de Infraestructura - DimmedAPI

## Arquitectura General del Sistema

```mermaid
graph TB
    subgraph "Cliente Frontend"
        A[Angular App - localhost:4200]
    end

    subgraph "API Gateway / Load Balancer"
        B[Azure Application Gateway]
    end

    subgraph "DimmedAPI - Backend"
        C[ASP.NET Core 9.0 Web API]
        D[Kestrel Server]
        E[Swagger/OpenAPI]
    end

    subgraph "Servicios de Negocio"
        F[Business Objects Layer]
        G[Services Layer]
    end

    subgraph "Persistencia de Datos"
        H[Entity Framework Core]
        I[SQL Server - Azure]
    end

    subgraph "Servicios Externos"
        J[Microsoft Business Central]
        K[Azure AD Authentication]
        L[CRM API - Sapiens]
        M[Email Service]
    end

    subgraph "Almacenamiento"
        N[Azure Blob Storage - Logos]
        O[Local File System - PDFs]
    end

    A --> B
    B --> C
    C --> D
    C --> E
    C --> F
    C --> G
    F --> H
    G --> H
    H --> I
    C --> J
    C --> K
    C --> L
    C --> M
    C --> N
    C --> O
```

## Arquitectura de Capas

```mermaid
graph TB
    subgraph "Presentation Layer"
        A1[Controllers]
        A2[DTOs]
        A3[Validaciones]
    end

    subgraph "Business Logic Layer"
        B1[Business Objects - BO]
        B2[Services]
        B3[Interfaces]
    end

    subgraph "Data Access Layer"
        C1[Entity Framework Core]
        C2[ApplicationDBContext]
        C3[Entidades/Models]
    end

    subgraph "Infrastructure Layer"
        D1[External APIs]
        D2[File Storage]
        D3[Email Service]
        D4[PDF Generation]
    end

    A1 --> A2
    A1 --> A3
    A1 --> B1
    A1 --> B2
    B1 --> B2
    B2 --> B3
    B1 --> C1
    B2 --> C1
    C1 --> C2
    C2 --> C3
    B2 --> D1
    B2 --> D2
    B2 --> D3
    B2 --> D4
```

## Detalle de Controllers y Endpoints

```mermaid
graph LR
    subgraph "EntryRequest Management"
        ER1[EntryRequestController]
        ER2[EntryRequestDetailsController]
        ER3[EntryRequestComponentsController]
        ER4[EntryRequestAssemblyController]
        ER5[EntryRequestTraceController]
    end

    subgraph "Customer Management"
        C1[CustomerController]
        C2[CustomerAddressController]
        C3[CustomerContactController]
        C4[CustomerTypeController]
    end

    subgraph "Equipment Management"
        E1[EquipmentController]
        E2[EquipmentAssemblyAPIController]
    end

    subgraph "Quotation Management"
        Q1[QuotationMasterController]
        Q2[QuotationDetailController]
        Q3[QuotationTypeController]
    end

    subgraph "Support Services"
        S1[EmailController]
        S2[PdfController]
        S3[UserController]
        S4[CompanyController]
    end

    ER1 --> ER2
    ER1 --> ER3
    ER1 --> ER4
    ER1 --> ER5
    C1 --> C2
    C1 --> C3
    C1 --> C4
    E1 --> E2
    Q1 --> Q2
    Q1 --> Q3
```

## Flujo de Datos Principal

```mermaid
sequenceDiagram
    participant Client as Cliente Angular
    participant API as DimmedAPI
    participant BO as Business Objects
    participant EF as Entity Framework
    participant DB as SQL Server
    participant BC as Business Central
    participant Email as Email Service
    participant PDF as PDF Service

    Client->>API: POST /api/EntryRequest
    API->>BO: Create EntryRequest
    BO->>EF: Save to Database
    EF->>DB: INSERT EntryRequests
    DB-->>EF: Success Response
    EF-->>BO: Entity Created
    BO->>BC: Sync with Business Central
    BC-->>BO: Sync Response
    BO->>Email: Send Notification
    Email-->>BO: Email Sent
    BO->>PDF: Generate PDF Report
    PDF-->>BO: PDF Generated
    BO-->>API: EntryRequest Created
    API-->>Client: 201 Created Response
```

## Configuración de Servicios

```mermaid
graph TB
    subgraph "Dependency Injection"
        DI1[AddDbContext - SQL Server]
        DI2[AddScoped - Business Objects]
        DI3[AddScoped - Services]
        DI4[AddSingleton - PDF Converter]
    end

    subgraph "Configuration"
        CF1[Connection Strings]
        CF2[Business Central URLs]
        CF3[Azure AD Settings]
        CF4[CORS Policy]
    end

    subgraph "Middleware Pipeline"
        MW1[CORS]
        MW2[Swagger]
        MW3[HTTPS Redirection]
        MW4[Static Files]
        MW5[Output Cache]
        MW6[Authorization]
        MW7[Controllers]
    end

    DI1 --> CF1
    DI2 --> CF2
    DI3 --> CF3
    DI4 --> CF4
    MW1 --> MW2
    MW2 --> MW3
    MW3 --> MW4
    MW4 --> MW5
    MW5 --> MW6
    MW6 --> MW7
```

## Modelo de Datos Principal

```mermaid
erDiagram
    EntryRequests ||--o{ EntryRequestDetails : contains
    EntryRequests ||--o{ EntryRequestComponents : has
    EntryRequests ||--o{ EntryRequestAssembly : includes
    EntryRequests ||--o{ EntryRequestTrace : tracks
    
    Customer ||--o{ CustomerAddress : has
    Customer ||--o{ CustomerContact : has
    Customer ||--o{ EntryRequests : creates
    
    Equipment ||--o{ EntryRequestAssembly : used_in
    Equipment ||--o{ EquipmentView : mapped_to
    
    QuotationMaster ||--o{ QuotationDetail : contains
    QuotationMaster ||--|| Customer : belongs_to
    
    Companies ||--o{ Branch : has
    Branch ||--o{ UserBranch : assigned_to
    Users ||--o{ UserBranch : belongs_to
    
    EntryRequests {
        int Id PK
        string RequestNumber
        datetime CreatedDate
        int CustomerId FK
        int StatusId FK
        string Description
    }
    
    Customer {
        int Id PK
        string Name
        string DocumentNumber
        string Email
        boolean IsActive
    }
    
    Equipment {
        int Id PK
        string Code
        string Name
        string Description
        decimal Price
    }
```

## Tecnologías y Versiones

| Componente | Tecnología | Versión |
|------------|------------|---------|
| **Backend Framework** | ASP.NET Core | 9.0 |
| **Database ORM** | Entity Framework Core | 9.0 |
| **Database** | SQL Server | Azure |
| **Authentication** | Azure AD | - |
| **PDF Generation** | DinkToPdf | - |
| **Containerization** | Docker | - |
| **API Documentation** | Swagger/OpenAPI | - |
| **External Integration** | Business Central API | v2.0 |
| **Email Service** | SMTP | - |
| **File Storage** | Azure Blob Storage | - |

## Configuraciones de Seguridad

- **CORS**: Configurado para permitir orígenes específicos
- **HTTPS**: Redirección forzada a HTTPS
- **Authentication**: Azure AD integration
- **Database**: Conexión encriptada a Azure SQL
- **API Keys**: Configuración segura para servicios externos

## Puntos de Integración

1. **Business Central**: Sincronización de datos comerciales
2. **CRM Sapiens**: Integración para pedidos
3. **Azure AD**: Autenticación y autorización
4. **Email Service**: Notificaciones automáticas
5. **PDF Service**: Generación de reportes
6. **File Storage**: Almacenamiento de logos y documentos
