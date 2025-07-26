# Email API

## Descripción
API para el envío de correos electrónicos utilizando la configuración de correo de las compañías.

## Endpoints

### Enviar Correo Electrónico

**POST** `/api/email/send?companyCode={companyCode}`

Envía un correo electrónico utilizando la configuración de correo de la compañía especificada.

#### Parámetros de Consulta

| Parámetro | Tipo | Requerido | Descripción |
|-----------|------|-----------|-------------|
| companyCode | string | Sí | Código de la compañía para obtener la configuración de correo |

#### Parámetros de Entrada

```json
{
  "toEmail": "string",
  "subject": "string",
  "body": "string",
  "isHtml": false
}
```

| Campo | Tipo | Requerido | Descripción |
|-------|------|-----------|-------------|
| toEmail | string | Sí | Dirección de correo electrónico del destinatario |
| subject | string | Sí | Asunto del correo electrónico |
| body | string | Sí | Contenido del correo electrónico |
| isHtml | boolean | No | Indica si el contenido del correo es HTML (por defecto: false) |

#### Respuesta Exitosa

**Código:** 200 OK

```json
{
  "success": true,
  "message": "Correo enviado exitosamente",
  "errorDetails": null,
  "sentAt": "2024-01-15T10:30:00.000Z"
}
```

#### Respuestas de Error

**Código:** 400 Bad Request

```json
{
  "success": false,
  "message": "La compañía no tiene configurada la información de correo electrónico",
  "errorDetails": "Email configuration missing",
  "sentAt": "2024-01-15T10:30:00.000Z"
}
```

**Código:** 404 Not Found

```json
{
  "success": false,
  "message": "Compañía con código COMPANY001 no encontrada",
  "errorDetails": "Company not found",
  "sentAt": "2024-01-15T10:30:00.000Z"
}
```

**Código:** 500 Internal Server Error

```json
{
  "success": false,
  "message": "Error interno del servidor",
  "errorDetails": "Detalles del error específico",
  "sentAt": "2024-01-15T10:30:00.000Z"
}
```

## Configuración de Compañía

Para que el envío de correos funcione correctamente, la compañía debe tener configurados los siguientes campos en la entidad `Companies`:

- `correonotificacion`: Dirección de correo electrónico del remitente
- `nombrenotificacion`: Nombre del remitente (opcional, usa BusinessName si no está configurado)
- `pwdnotificacion`: Contraseña del correo electrónico
- `smtpserver`: Servidor SMTP (ej: smtp.office365.com para Microsoft)
- `puertosmtp`: Puerto SMTP (ej: 587 para Microsoft)

## Ejemplo de Uso

### Envío de Correo Simple

```bash
curl -X POST "https://api.example.com/api/email/send?companyCode=COMPANY001" \
  -H "Content-Type: application/json" \
  -d '{
    "toEmail": "destinatario@example.com",
    "subject": "Prueba de Correo",
    "body": "Este es un correo de prueba enviado desde la API.",
    "isHtml": false
  }'
```

### Envío de Correo HTML

```bash
curl -X POST "https://api.example.com/api/email/send?companyCode=COMPANY001" \
  -H "Content-Type: application/json" \
  -d '{
    "toEmail": "destinatario@example.com",
    "subject": "Correo HTML",
    "body": "<h1>Hola</h1><p>Este es un <strong>correo HTML</strong>.</p>",
    "isHtml": true
  }'
```

## Notas Importantes

1. **Seguridad**: Las credenciales de correo se almacenan en la base de datos. Asegúrese de que la base de datos esté protegida adecuadamente.

2. **Configuración SMTP**: Para Microsoft Office 365, use:
   - Servidor SMTP: `smtp.office365.com`
   - Puerto: `587`
   - SSL: Habilitado

3. **Validaciones**: El API valida que:
   - La compañía existe
   - La compañía tiene configurada la información de correo
   - Todos los campos requeridos están presentes

4. **Manejo de Errores**: El API maneja errores de conexión SMTP y proporciona mensajes descriptivos para facilitar la depuración. 