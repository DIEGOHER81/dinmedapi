# API de Términos de Pago (PaymentTerm)

Esta API permite sincronizar y consultar los términos de pago desde Business Central y almacenarlos localmente.

## Sincronizar términos de pago desde Business Central

**POST** `/api/PaymentTerm/sincronizar-bc?companyCode={companyCode}`

Sincroniza los términos de pago desde Business Central a la base de datos local de la compañía indicada.

### Parámetros
- `companyCode` (query, requerido): Código de la compañía a sincronizar.

### Respuesta exitosa
```json
[
  {
    "id": 1,
    "code": "30D",
    "description": "30 DIAS",
    "dueDateCalculation": "30D"
  },
  ...
]
```

---

## Consultar términos de pago locales

**GET** `/api/PaymentTerm?companyCode={companyCode}`

Obtiene todos los términos de pago almacenados localmente para la compañía indicada.

### Parámetros
- `companyCode` (query, requerido): Código de la compañía a consultar.

### Respuesta exitosa
```json
[
  {
    "id": 1,
    "code": "30D",
    "description": "30 DIAS",
    "dueDateCalculation": "30D"
  },
  ...
]
```

---

## Notas
- Los endpoints requieren el parámetro `companyCode` para operar sobre la base de datos de la compañía correspondiente.
- El endpoint de sincronización obtiene los datos desde Business Central y los guarda/actualiza localmente.
- El endpoint de consulta solo lee los datos locales. 