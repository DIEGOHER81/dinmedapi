# FollowType API

API para la gestión de Tipos de Seguimiento (FollowType).

## Endpoints

### Obtener todos los tipos de seguimiento
- **GET** `/api/FollowType?companyCode={companyCode}`
- **Descripción:** Obtiene la lista de todos los tipos de seguimiento.
- **Parámetros:**
  - `companyCode` (query, requerido): Código de la compañía.
- **Respuesta:**
```json
[
  {
    "id": 1,
    "description": "Seguimiento telefónico",
    "isActive": true
  }
]
```

---

### Obtener un tipo de seguimiento por ID
- **GET** `/api/FollowType/{id}?companyCode={companyCode}`
- **Descripción:** Obtiene un tipo de seguimiento por su identificador.
- **Parámetros:**
  - `id` (path, requerido): ID del tipo de seguimiento.
  - `companyCode` (query, requerido): Código de la compañía.
- **Respuesta:**
```json
{
  "id": 1,
  "description": "Seguimiento telefónico",
  "isActive": true
}
```

---

### Crear un tipo de seguimiento
- **POST** `/api/FollowType?companyCode={companyCode}`
- **Descripción:** Crea un nuevo tipo de seguimiento.
- **Parámetros:**
  - `companyCode` (query, requerido): Código de la compañía.
- **Body:**
```json
{
  "description": "Seguimiento telefónico",
  "isActive": true
}
```
- **Respuesta:**
```json
{
  "id": 1,
  "description": "Seguimiento telefónico",
  "isActive": true
}
```

---

### Actualizar un tipo de seguimiento
- **PUT** `/api/FollowType/{id}?companyCode={companyCode}`
- **Descripción:** Actualiza un tipo de seguimiento existente.
- **Parámetros:**
  - `id` (path, requerido): ID del tipo de seguimiento.
  - `companyCode` (query, requerido): Código de la compañía.
- **Body:**
```json
{
  "description": "Seguimiento presencial",
  "isActive": false
}
```
- **Respuesta:**
  - `204 No Content` si la actualización fue exitosa.

---

### Eliminar un tipo de seguimiento
- **DELETE** `/api/FollowType/{id}?companyCode={companyCode}`
- **Descripción:** Elimina un tipo de seguimiento por su identificador.
- **Parámetros:**
  - `id` (path, requerido): ID del tipo de seguimiento.
  - `companyCode` (query, requerido): Código de la compañía.
- **Respuesta:**
  - `204 No Content` si la eliminación fue exitosa.

---

## Notas
- Todos los endpoints requieren el parámetro `companyCode` para operar sobre la base de datos correspondiente a la compañía.
- El campo `isActive` es opcional y puede ser `true`, `false` o `null`.
- En caso de error, la API retorna un mensaje descriptivo y el código HTTP correspondiente. 