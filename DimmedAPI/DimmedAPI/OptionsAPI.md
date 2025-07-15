# API de Options

Este documento describe los endpoints disponibles para la entidad **Options** en el sistema, incluyendo ejemplos de uso. Todos los endpoints requieren el parámetro `companyCode` para operar en el contexto de la compañía correspondiente.

---

## Endpoints

### 1. Obtener todas las opciones

- **GET** `/api/Options?companyCode={companyCode}`

**Descripción:**
Obtiene todas las opciones registradas para la compañía indicada.

**Ejemplo de uso:**
```http
GET /api/Options?companyCode=COMP01
```

---

### 2. Obtener una opción por ID

- **GET** `/api/Options/{id}?companyCode={companyCode}`

**Descripción:**
Obtiene una opción específica por su ID.

**Ejemplo de uso:**
```http
GET /api/Options/5?companyCode=COMP01
```

---

### 3. Crear una nueva opción

- **POST** `/api/Options?companyCode={companyCode}`

**Descripción:**
Crea una nueva opción en la compañía indicada.

**Body de ejemplo:**
```json
{
  "text": "Nueva opción",
  "isActive": true,
  "path": "/ruta",
  "iOrder": 1,
  "parent": null,
  "icon": "icono",
  "isParent": false
}
```

**Ejemplo de uso:**
```http
POST /api/Options?companyCode=COMP01
Content-Type: application/json

{
  "text": "Nueva opción",
  "isActive": true,
  "path": "/ruta",
  "iOrder": 1,
  "parent": null,
  "icon": "icono",
  "isParent": false
}
```

---

### 4. Actualizar una opción existente

- **PUT** `/api/Options/{id}?companyCode={companyCode}`

**Descripción:**
Actualiza todos los campos de una opción existente.

**Body de ejemplo:**
```json
{
  "id": 5,
  "text": "Opción actualizada",
  "isActive": true,
  "path": "/nueva-ruta",
  "iOrder": 2,
  "parent": null,
  "icon": "nuevo-icono",
  "isParent": false
}
```

**Ejemplo de uso:**
```http
PUT /api/Options/5?companyCode=COMP01
Content-Type: application/json

{
  "id": 5,
  "text": "Opción actualizada",
  "isActive": true,
  "path": "/nueva-ruta",
  "iOrder": 2,
  "parent": null,
  "icon": "nuevo-icono",
  "isParent": false
}
```

---

### 5. Eliminar una opción

- **DELETE** `/api/Options/{id}?companyCode={companyCode}`

**Descripción:**
Elimina una opción por su ID.

**Ejemplo de uso:**
```http
DELETE /api/Options/5?companyCode=COMP01
```

---

### 6. Actualizar solo el estado (IsActive) de una opción

- **PATCH** `/api/Options/{id}/estado?companyCode={companyCode}`

**Descripción:**
Actualiza únicamente el campo `IsActive` de una opción específica.

**Body de ejemplo:**
```json
true
```

**Ejemplo de uso:**
```http
PATCH /api/Options/5/estado?companyCode=COMP01
Content-Type: application/json

true
```

---

## Notas
- Todos los endpoints requieren el parámetro `companyCode` como query string.
- El campo `id` es autogenerado al crear una nueva opción.
- El endpoint PATCH espera un valor booleano en el body para el estado `IsActive`. 