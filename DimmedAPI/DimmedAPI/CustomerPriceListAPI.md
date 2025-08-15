# Customer Price List API

## Descripción
API para consultar las listas de precio asociadas a un cliente específico mediante el procedimiento almacenado `TI_GET_CUSTOMERPRICELIST`.

## Endpoints

### GET /api/CustomerPriceList/customer/{customerId}

Obtiene las listas de precio asociadas a un cliente específico.

#### Parámetros
- `customerId` (int, requerido): ID del cliente

#### Respuesta Exitosa (200 OK)
```json
[
  {
    "id": 1,
    "customerId": 123,
    "identification": "12345678",
    "name": "Cliente Ejemplo",
    "insurerType": "EPS",
    "priceGroup": "GRUPO_A"
  },
  {
    "id": 2,
    "customerId": 123,
    "identification": "87654321",
    "name": "Cliente Ejemplo 2",
    "insurerType": "ARS",
    "priceGroup": "GRUPO_B"
  }
]
```

#### Respuestas de Error

**400 Bad Request**
```json
"El ID del cliente debe ser mayor a 0"
```

**404 Not Found**
```json
"No se encontraron listas de precio para el cliente con ID 123"
```

**500 Internal Server Error**
```json
"Error interno del servidor: [mensaje de error]"
```

## Campos de Respuesta

| Campo | Tipo | Descripción | Nullable |
|-------|------|-------------|----------|
| id | int | ID único de la lista de precio | No |
| customerId | int | ID del cliente | No |
| identification | string | Identificación del cliente | Sí |
| name | string | Nombre del cliente | Sí |
| insurerType | string | Tipo de aseguradora | Sí |
| priceGroup | string | Grupo de precios | Sí |

## Procedimiento Almacenado

El servicio utiliza el procedimiento almacenado `TI_GET_CUSTOMERPRICELIST` con los siguientes parámetros:

- `@IdCustomer` (int): ID del cliente (puede ser NULL)

## Ejemplo de Uso

```bash
curl -X GET "https://api.example.com/api/CustomerPriceList/customer/123" \
  -H "accept: application/json"
```

## Notas Técnicas

- El servicio maneja valores nulos en los campos de respuesta
- Timeout del procedimiento almacenado: 120 segundos
- Logging detallado para auditoría y debugging
- Validación de parámetros de entrada
- Manejo de excepciones con mensajes descriptivos

## SUPLE TI

Este servicio fue implementado siguiendo las especificaciones de SUPLE TI para la consulta de listas de precio por ID de cliente.
