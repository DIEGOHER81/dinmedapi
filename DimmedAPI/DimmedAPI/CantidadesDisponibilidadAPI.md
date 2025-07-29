# API de Consulta de Cantidades y Disponibilidad

Esta API proporciona endpoints específicos para consultar información detallada de cantidades y disponibilidad de componentes desde Business Central con soporte multicompañía.

## Configuración Requerida

Asegúrate de que la compañía esté configurada en la tabla `Companies` con:
- `BCCodigoEmpresa`: Código de la compañía
- `BCURL`: URL de Business Central
- `BCURLWebService`: URL del web service de Business Central

También se requieren las credenciales de Azure AD configuradas en `appsettings.json`:
- `AzureAd:ClientId`
- `AzureAd:ClientSecret`
- `AzureAd:TenantId`

## Endpoints

### 1. Consultar Cantidades y Disponibilidad

**GET** `/api/EntryRequestComponents/consultar-cantidades-disponibilidad`

Consulta información detallada de cantidades y disponibilidad de componentes desde Business Central.

**Parámetros de consulta:**
- `companyCode` (string, requerido): Código de la compañía
- `location` (string, opcional): Bodega/Ubicación
- `stock` (string, opcional): Bodega de stock
- `salesCode` (string, opcional): Código de venta
- `reference` (string, opcional): Código de referencia del componente

**Ejemplo de respuesta exitosa:**
```json
{
  "totalComponentes": 5,
  "componentesConStock": 4,
  "componentesSinStock": 1,
  "componentes": [
    {
      "id": 1,
      "itemNo": "ITEM001",
      "itemName": "Componente 1",
      "warehouse": "WH01",
      "quantity": 15.5,
      "unitPrice": 25.50,
      "systemId": "SALES001",
      "branch": "BR01",
      "lot": "LOT001",
      "status": "ACTIVE",
      "assemblyNo": "ASM001",
      "taxCode": "TAX01",
      "shortDesc": "Comp 1",
      "invima": "INV001",
      "expirationDate": "2024-12-31T00:00:00",
      "traceState": "TRACED",
      "rsFechaVencimiento": "2024-12-31T00:00:00",
      "rsClasifRegistro": "CLASS01",
      "disponibilidad": {
        "cantidadDisponible": 15.5,
        "cantidadReservada": 2.0,
        "cantidadNeta": 13.5,
        "tieneStock": true,
        "requiereReposicion": false,
        "estadoInventario": "STOCK_ALTO"
      }
    }
  ]
}
```

**Ejemplo de respuesta cuando no hay datos:**
```json
{
  "totalComponentes": 0,
  "componentesConStock": 0,
  "componentesSinStock": 0,
  "componentes": [],
  "mensaje": "No se pudieron obtener componentes desde Business Central"
}
```

### 2. Consultar Cantidades por Grupo de Precios

**GET** `/api/EntryRequestComponents/consultar-cantidades-por-grupo-precios`

Consulta información de cantidades por grupo de precios desde Business Central.

**Parámetros de consulta:**
- `companyCode` (string, requerido): Código de la compañía
- `location` (string, opcional): Bodega/Ubicación
- `stock` (string, opcional): Bodega de stock
- `priceGroup` (string, opcional): Grupo de precios
- `reference` (string, opcional): Código de referencia del componente

**Ejemplo de respuesta exitosa:**
```json
{
  "totalComponentes": 5,
  "grupoPrecios": "GRUPO_A",
  "componentesConStock": 4,
  "componentesSinStock": 1,
  "valorTotalInventario": 1250.75,
  "componentes": [
    {
      "id": 1,
      "itemNo": "ITEM001",
      "itemName": "Componente 1",
      "warehouse": "WH01",
      "quantity": 15.5,
      "unitPrice": 25.50,
      "systemId": "GRUPO_A",
      "branch": "BR01",
      "lot": "LOT001",
      "status": "ACTIVE",
      "assemblyNo": "ASM001",
      "taxCode": "TAX01",
      "shortDesc": "Comp 1",
      "invima": "INV001",
      "expirationDate": "2024-12-31T00:00:00",
      "traceState": "TRACED",
      "rsFechaVencimiento": "2024-12-31T00:00:00",
      "rsClasifRegistro": "CLASS01",
      "informacionPrecios": {
        "grupoPrecios": "GRUPO_A",
        "precioUnitario": 25.50,
        "cantidadDisponible": 15.5,
        "cantidadReservada": 2.0,
        "cantidadNeta": 13.5,
        "valorTotalDisponible": 395.25,
        "tieneStock": true,
        "requiereReposicion": false,
        "estadoInventario": "STOCK_ALTO"
      }
    }
  ]
}
```

**Ejemplo de respuesta cuando no hay datos:**
```json
{
  "totalComponentes": 0,
  "grupoPrecios": "GRUPO_A",
  "componentesConStock": 0,
  "componentesSinStock": 0,
  "valorTotalInventario": 0,
  "componentes": [],
  "mensaje": "No se pudieron obtener componentes desde Business Central"
}
```

## Estados de Inventario

Los endpoints devuelven información sobre el estado del inventario con las siguientes categorías:

- **SIN_STOCK**: Cantidad disponible = 0
- **STOCK_BAJO**: Cantidad disponible ≤ 5
- **STOCK_MEDIO**: Cantidad disponible entre 6 y 20
- **STOCK_ALTO**: Cantidad disponible > 20

## Códigos de Error

### 400 Bad Request
- Cuando no se proporciona el `companyCode`
- Cuando hay errores de conexión con Business Central

### 404 Not Found
- Cuando la compañía especificada no existe
- Cuando la configuración de la compañía no es válida

### 200 OK (con datos vacíos)
- Cuando no se pueden obtener datos de Business Central
- Cuando no hay componentes que cumplan con los criterios de búsqueda

## Ejemplos de Uso

### Consultar cantidades con filtros completos
```bash
curl -X GET "https://api.example.com/api/EntryRequestComponents/consultar-cantidades-disponibilidad?companyCode=COMP001&location=WH01&stock=STK01&salesCode=SALES001&reference=ITEM001"
```

### Consultar cantidades por grupo de precios
```bash
curl -X GET "https://api.example.com/api/EntryRequestComponents/consultar-cantidades-por-grupo-precios?companyCode=COMP001&location=WH01&stock=STK01&priceGroup=GRUPO_A&reference=ITEM001"
```

### Consultar cantidades sin filtros específicos
```bash
curl -X GET "https://api.example.com/api/EntryRequestComponents/consultar-cantidades-disponibilidad?companyCode=COMP001"
```

### Consultar cantidades por grupo de precios sin filtros específicos
```bash
curl -X GET "https://api.example.com/api/EntryRequestComponents/consultar-cantidades-por-grupo-precios?companyCode=COMP001&priceGroup=GRUPO_A"
```

## Características Especiales

1. **Soporte Multicompañía**: Todos los endpoints requieren el parámetro `companyCode` para identificar la compañía específica.

2. **Filtros Dinámicos**: Los parámetros `location`, `stock`, `salesCode`, `priceGroup` y `reference` son opcionales y se aplican dinámicamente.

3. **Información de Disponibilidad**: Cada componente incluye información detallada sobre:
   - Cantidad disponible
   - Cantidad reservada
   - Cantidad neta (disponible - reservada)
   - Estado del inventario
   - Indicadores de reposición

4. **Información de Precios**: El endpoint de grupo de precios incluye:
   - Precio unitario
   - Valor total del inventario
   - Información del grupo de precios

5. **Estadísticas Agregadas**: Ambos endpoints devuelven estadísticas como:
   - Total de componentes
   - Componentes con/sin stock
   - Valor total del inventario (solo en el endpoint de precios)

6. **Manejo Robusto de Errores**: Los endpoints manejan:
   - Conexiones fallidas con Business Central
   - Datos null o vacíos
   - Errores de configuración de compañía

## Notas Importantes

1. **Umbral de Reposición**: El sistema considera que un componente requiere reposición cuando tiene 5 unidades o menos (configurable).

2. **Cálculo de Cantidad Neta**: Se calcula como `Cantidad Disponible - Cantidad Reservada`.

3. **Filtrado por Referencia**: Si se proporciona el parámetro `reference`, se filtra por `ItemNo` exacto o por contenido en `ItemName`.

4. **Manejo de Errores**: Los endpoints manejan errores de configuración de compañía y errores de conexión con Business Central.

5. **Compatibilidad**: Estos endpoints son compatibles con el método `getComponentReferenceAsync` del sistema anterior, pero con funcionalidad mejorada y soporte multicompañía.

6. **Respuestas Consistentes**: Incluso cuando hay errores de conexión, los endpoints devuelven una respuesta estructurada con datos vacíos en lugar de errores críticos.

7. **Logging**: Los errores se registran en la consola para facilitar el debugging.