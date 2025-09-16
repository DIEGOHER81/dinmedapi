# EntryRequestComponents con Información de Equipo API

## Descripción
Este endpoint extiende la funcionalidad del endpoint `por-entryrequest` para incluir información adicional del equipo asociado al `assemblyNo` de cada componente.

## Endpoint

### GET /api/EntryRequestComponents/por-entryrequest-con-equipo

Obtiene los componentes de una solicitud de entrada (EntryRequest) incluyendo información del equipo asociado.

#### Parámetros de Query

| Parámetro | Tipo | Requerido | Descripción |
|-----------|------|-----------|-------------|
| `companyCode` | string | Sí | Código de la compañía |
| `idEntryReq` | int | Sí | ID de la solicitud de entrada |

#### Ejemplo de Request

```bash
curl -X 'GET' \
  'https://agendamientoapi.pruebas.sapienss.com.co/api/EntryRequestComponents/por-entryrequest-con-equipo?companyCode=6c265367-24c4-ed11-9a88-002248e00201&idEntryReq=10344' \
  -H 'accept: */*'
```

#### Ejemplo de Response

```json
[
  {
    "id": 286,
    "itemNo": "11201MR1000",
    "itemName": "REGULADOR DE NITROGENO IM",
    "warehouse": "101",
    "quantity": 1,
    "idEntryReq": 10344,
    "systemId": "",
    "quantityConsumed": 0,
    "branch": "101",
    "lot": "",
    "unitPrice": 0,
    "status": "NUEVO",
    "assemblyNo": "1182-1",
    "taxCode": "V_ARTVENTAGV",
    "shortDesc": "MR1000",
    "locationCodeStock": null,
    "name": null,
    "userIdTraceState": null,
    "invima": "NO REQUIERE",
    "expirationDate": "2999-12-31T00:00:00",
    "traceState": "",
    "rsFechaVencimiento": null,
    "rsClasifRegistro": "",
    "equipmentName": "EQUIPO DE NITRÓGENO PORTÁTIL",
    "equipmentDescription": "Equipo médico para administración de nitrógeno",
    "equipmentStatus": "ACTIVO"
  }
]
```

#### Campos Adicionales

| Campo | Tipo | Descripción |
|-------|------|-------------|
| `equipmentName` | string | Nombre del equipo asociado al `assemblyNo` |
| `equipmentDescription` | string | Descripción del equipo |
| `equipmentStatus` | string | Estado del equipo (ACTIVO, INACTIVO, etc.) |

#### Códigos de Respuesta

| Código | Descripción |
|--------|-------------|
| 200 | OK - Lista de componentes obtenida exitosamente |
| 400 | Bad Request - Parámetros inválidos o faltantes |
| 404 | Not Found - Compañía no encontrada |
| 500 | Internal Server Error - Error interno del servidor |

#### Notas

- Si el `assemblyNo` está vacío o no se encuentra un equipo asociado, los campos `equipmentName`, `equipmentDescription` y `equipmentStatus` serán `null`.
- El endpoint utiliza cache con una duración de 5 minutos para optimizar el rendimiento.
- La relación se establece entre el campo `assemblyNo` del componente y el campo `Code` de la tabla `Equipment`.

#### Diferencias con el Endpoint Original

El endpoint original `/api/EntryRequestComponents/por-entryrequest` devuelve la misma información pero sin los campos adicionales del equipo:

```json
[
  {
    "id": 286,
    "itemNo": "11201MR1000",
    "itemName": "REGULADOR DE NITROGENO IM",
    "warehouse": "101",
    "quantity": 1,
    "idEntryReq": 10344,
    "systemId": "",
    "quantityConsumed": 0,
    "branch": "101",
    "lot": "",
    "unitPrice": 0,
    "status": "NUEVO",
    "assemblyNo": "1182-1",
    "taxCode": "V_ARTVENTAGV",
    "shortDesc": "MR1000",
    "locationCodeStock": null,
    "name": null,
    "userIdTraceState": null,
    "invima": "NO REQUIERE",
    "expirationDate": "2999-12-31T00:00:00",
    "traceState": "",
    "rsFechaVencimiento": null,
    "rsClasifRegistro": ""
  }
]
```

El nuevo endpoint agrega los campos:
- `equipmentName`
- `equipmentDescription` 
- `equipmentStatus`
