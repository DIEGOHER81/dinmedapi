# EntryRequest Filter Endpoint con Relaciones

## Endpoint
```
GET /api/EntryRequest/filter?companyCode=6c265367-24c4-ed11-9a88-002248e00201&Id=22
```

## Respuesta Actualizada

Con las modificaciones realizadas, el endpoint ahora incluye las relaciones con `Patient` y `CustomerAddress` además de la relación `Customer` que ya existía.

### Ejemplo de Respuesta JSON

```json
[
  {
    "id": 22,
    "date": "2023-10-20T08:14:00",
    "service": "ROTACION",
    "idOrderType": 1,
    "deliveryPriority": "NO APLICA",
    "idCustomer": 55,
    "insurerType": 4,
    "insurer": 168,
    "idMedic": 964,
    "idPatient": 4585,
    "applicant": "CO000198",
    "idATC": 116,
    "limbSide": "NO APLICA",
    "deliveryDate": "2023-10-20T11:00:00",
    "orderObs": "INFORMAR FALTANTES A SAC",
    "surgeryTime": 4200,
    "surgeryInit": "2023-10-23T07:00:00",
    "surgeryEnd": "2023-10-23T09:00:00",
    "status": "CONSUMO ATC",
    "idTraceStates": 56,
    "branchId": 1,
    "surgeryInitTime": "07:00:00",
    "surgeryEndTime": "09:00:00",
    "deliveryAddress": "PRINCIPAL",
    "purchaseOrder": "",
    "atcConsumed": true,
    "isSatisfied": true,
    "observations": "demo",
    "obsMaint": "demo",
    "auxLog": 4,
    "idCancelReason": null,
    "idCancelDetail": null,
    "cancelReason": null,
    "cancelDetail": null,
    "notification": true,
    "isReplacement": false,
    "assemblyComponents": false,
    "priceGroup": null,
    "customer": {
      "id": 55,
      "identification": "890902922",
      "idType": 31,
      "address": "CR 72 A 78 B 50",
      "city": "MEDELLIN",
      "phone": "5744455",
      "email": "auxsistemas@suplemedicos.com.co",
      "certMant": false,
      "remCustomer": true,
      "observations": "Imprimir 4 Copias Para Equipos De Consignacion.",
      "name": "CLINICA UNIVERSIDAD PONTIFICIA BOLIVARIANA",
      "systemIdBc": "ded4d96c-7ecf-ed11-a7c9-002248e0ec87",
      "salesZone": "1-NORT MED",
      "tradeRepres": "YULIANA  DUQUE GARCES",
      "noCopys": 4,
      "isActive": true,
      "segment": "TOP",
      "no": "890902922",
      "fullName": "CLINICA UNIVERSIDAD PONTIFICIA BOLIVARIANA",
      "priceGroup": "SPUPB23V1",
      "shortDesc": false,
      "exIva": true,
      "isSecondPriceList": true,
      "secondPriceGroup": "SPUPBS23V1",
      "insurerType": "SOAT",
      "isRemLot": false,
      "lyLOpeningHours1": null,
      "lyLOpeningHours2": null,
      "paymentMethodCode": null,
      "paymentTermsCode": null
    },
    "patient": {
      "id": 4585,
      "identification": "12345678",
      "name": "Juan Carlos",
      "idType": 1,
      "address": "Calle 123 # 45-67",
      "city": "MEDELLIN",
      "phone": "3001234567",
      "email": "juan.carlos@email.com",
      "isActive": true,
      "medicalRecord": "HC-2023-001",
      "gender": "M",
      "lastName": "Pérez González",
      "isPrecise": false
    },
    "customerAddresses": [
      {
        "id": 1,
        "code": "DIR001",
        "name": "Sede Principal",
        "address": "CR 72 A 78 B 50",
        "city": "MEDELLIN",
        "postCode": "050034",
        "locationCode": "LOC001",
        "systemIdBC": "bc-addr-001",
        "customerId": 55,
        "customerNo": "890902922",
        "phone": "5744455"
      },
      {
        "id": 2,
        "code": "DIR002",
        "name": "Sede Norte",
        "address": "CR 64 A 45 B 23",
        "city": "MEDELLIN",
        "postCode": "050034",
        "locationCode": "LOC002",
        "systemIdBC": "bc-addr-002",
        "customerId": 55,
        "customerNo": "890902922",
        "phone": "5744456"
      }
    ]
  }
]
```

## Cambios Realizados

### 1. Actualización del DTO
Se agregaron las siguientes propiedades al `EntryRequestFilteredResponseDTO`:

```csharp
// Relación con Patient
public Patient? Patient { get; set; }

// Relación con CustomerAddress (colección de direcciones del cliente)
public ICollection<CustomerAddress>? CustomerAddresses { get; set; }
```

### 2. Actualización del Controlador
Se modificó la consulta en el método `GetEntryRequestsFiltered` para incluir las nuevas relaciones:

```csharp
var query = companyContext.EntryRequests
    .Include(er => er.IdCustomerNavigation) // Incluir la relación con Customer
    .Include(er => er.IdPatientNavigation) // Incluir la relación con Patient
    .Include(er => er.IdCustomerNavigation.ShipAddress) // Incluir las direcciones del cliente
    .AsQueryable();
```

Y se actualizó el mapeo en el Select:

```csharp
Customer = er.IdCustomerNavigation,
Patient = er.IdPatientNavigation,
CustomerAddresses = er.IdCustomerNavigation?.ShipAddress
```

### 3. Actualización de la Entidad Customer
Se descomentó la relación `ShipAddress` en la entidad `Customer`:

```csharp
public virtual ICollection<CustomerAddress> ShipAddress { get; set; }
```

Y se agregó el constructor para inicializar la colección:

```csharp
public Customer()
{
    ShipAddress = new HashSet<CustomerAddress>();
}
```

## Beneficios

1. **Información Completa**: Ahora el endpoint devuelve toda la información relacionada en una sola consulta
2. **Mejor Rendimiento**: Se evitan múltiples llamadas al servidor para obtener información relacionada
3. **Facilidad de Uso**: Los clientes pueden obtener toda la información necesaria en una sola petición
4. **Consistencia**: Se mantiene la misma estructura de respuesta que ya existía para Customer

## Uso

El endpoint funciona exactamente igual que antes, pero ahora incluye automáticamente las relaciones con Patient y CustomerAddress cuando están disponibles en la base de datos. 