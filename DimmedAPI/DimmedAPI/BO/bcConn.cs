using DimmedAPI.Entidades;
using DimmedAPI.DTOs;
using RestSharp;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;
using System.Linq;

namespace DimmedAPI.BO
{
    public class bcConn : IntBCConex
    {
        private readonly string _baseUrl;
        private readonly string _companyId;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _tenantId;

        public bcConn(IConfiguration configuration)
        {
            _baseUrl = configuration["BusinessCentral:url"];
            _companyId = configuration["BusinessCentral:company"];
            _clientId = configuration["AzureAd:ClientId"];
            _clientSecret = configuration["AzureAd:ClientSecret"];
            _tenantId = configuration["AzureAd:TenantId"];
        }

        public async Task<string> BCRQ_GETTOKEN()
        {
            var tokenUrl = $"https://login.microsoftonline.com/{_tenantId}/oauth2/v2.0/token";
            var client = new RestClient(tokenUrl);
            var request = new RestRequest();
            request.Method = Method.Post;
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("grant_type", "client_credentials");
            request.AddParameter("client_id", _clientId);
            request.AddParameter("client_secret", _clientSecret);
            request.AddParameter("scope", "https://api.businesscentral.dynamics.com/.default");

            var response = await client.ExecuteAsync(request);
            if (response.IsSuccessful)
            {
                var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(response.Content);
                return tokenResponse.AccessToken;
            }
            throw new Exception($"Error obteniendo token: {response.ErrorMessage} - {response.Content}");
        }

        public async Task<string> BCRQ_GETTOKEN_BY_USER(string username, string password)
        {
            var tokenUrl = $"https://login.microsoftonline.com/{_tenantId}/oauth2/v2.0/token";
            var client = new RestClient(tokenUrl);
            var request = new RestRequest();
            request.Method = Method.Post;
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("grant_type", "password");
            request.AddParameter("username", username);
            request.AddParameter("password", password);
            request.AddParameter("client_id", _clientId);
            request.AddParameter("client_secret", _clientSecret);
            request.AddParameter("scope", "https://api.businesscentral.dynamics.com/.default");

            var response = await client.ExecuteAsync(request);
            if (response.IsSuccessful)
            {
                var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(response.Content);
                return tokenResponse.AccessToken;
            }
            throw new Exception($"Error obteniendo token: {response.ErrorMessage} - {response.Content}");
        }

        public async Task<RestResponse> BCRQ(string method, string filter)
        {
            try
            {
                var token = await BCRQ_GETTOKEN();
                string url;
                
                // Construir la URL específica para cada tipo de endpoint
                switch (method)
                {
                    case "lylequipment":
                        url = $"{_baseUrl}lylequipment";
                        break;
                    case "lylcustomer":
                        url = $"{_baseUrl}lylcustomer";
                        break;
                    case "lylassembly":
                    case "lylassemblyV2":
                    case "lylassemblyolines":
                    case "lylassemblyeq":
                        url = $"{_baseUrl}{method}";
                        break;
                    default:
                        url = $"{_baseUrl}{method}";
                        break;
                }

                var client = new RestClient(url);
                var request = new RestRequest();
                request.AddHeader("Authorization", $"Bearer {token}");
                request.AddHeader("Accept", "application/json");

                // Solo agregar el filtro si se proporciona
                if (!string.IsNullOrEmpty(filter))
                {
                    // Remover el prefijo ?$filter= si existe
                    if (filter.StartsWith("?$filter="))
                    {
                        filter = filter.Substring(9);
                    }
                    request.AddQueryParameter("$filter", filter);
                }

                var response = await client.ExecuteAsync(request);
                if (!response.IsSuccessful)
                {
                    throw new Exception($"Error en BCRQ: {response.StatusCode} - {response.Content}");
                }
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en BCRQ: {ex.Message}", ex);
            }
        }

        public async Task<RestResponse> BCRQ_post(string method, string filter, string body)
        {
            var token = await BCRQ_GETTOKEN();
            var client = new RestClient($"{_baseUrl}/{method}");
            var request = new RestRequest();
            request.Method = Method.Post;
            request.AddHeader("Authorization", $"Bearer {token}");
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(body);

            return await client.ExecuteAsync(request);
        }

        public async Task<RestResponse> BCRQ_postDeletePD(string method, string filter, string body)
        {
            var token = await BCRQ_GETTOKEN();
            var client = new RestClient($"{_baseUrl}/{method}");
            var request = new RestRequest();
            request.Method = Method.Post;
            request.AddHeader("Authorization", $"Bearer {token}");
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(body);

            return await client.ExecuteAsync(request);
        }

        public async Task<CustomerBCDTO> getCustomerBCAsync(string method, string systemID)
        {
            var response = await BCRQ(method, $"No eq '{systemID}'");
            if (response.IsSuccessful)
            {
                var result = JsonConvert.DeserializeObject<CustomerBCResponse>(response.Content);
                return result.Value.FirstOrDefault();
            }
            throw new Exception($"Error obteniendo cliente: {response.ErrorMessage}");
        }

        public async Task<List<Customer>> ConnBCAsync(string method)
        {
            try
            {
                var response = await BCRQ(method, null);
                if (response.IsSuccessful)
                {
                    var result = JsonConvert.DeserializeObject<CustomerBCResponse>(response.Content);
                    return result.Value.Select(c => new Customer
                    {
                        SystemIdBc = c.SystemIdBc,
                        No = c.No,
                        Name = c.Name,
                        FullName = c.FullName,
                        Address = c.Address,
                        City = c.City,
                        Phone = c.Phone,
                        Email = c.Email,
                        Identification = c.Identification,
                        IdType = c.IdType ?? 0,
                        CertMant = c.CertMant,
                        RemCustomer = c.RemCustomer,
                        Observations = c.Observations,
                        SalesZone = c.SalesZone,
                        TradeRepres = c.TradeRepres,
                        NoCopys = c.NoCopys,
                        IsActive = c.IsActive,
                        Segment = c.Segment,
                        PriceGroup = c.PriceGroup,
                        ShortDesc = c.ShortDesc,
                        ExIva = c.ExIva,
                        IsSecondPriceList = c.IsSecondPriceList,
                        SecondPriceGroup = c.SecondPriceGroup,
                        InsurerType = c.InsurerType,
                        IsRemLot = c.IsRemLot,
                        LyLOpeningHours1 = c.LyLOpeningHours1,
                        LyLOpeningHours2 = c.LyLOpeningHours2,
                        PaymentMethodCode = c.PaymentMethodCode,
                        PaymentTermsCode = c.PaymentTermsCode
                    }).ToList();
                }
                throw new Exception($"Error obteniendo clientes: {response.StatusCode} - {response.Content}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en ConnBCAsync: {ex.Message}", ex);
            }
        }

        public async Task<RestResponse> SPLE_API(string method, string filter, string type, string body)
        {
            var token = await BCRQ_GETTOKEN();
            var client = new RestClient($"{_baseUrl}/{method}");
            var request = new RestRequest();
            request.Method = Method.Post;
            request.AddHeader("Authorization", $"Bearer {token}");
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(body);

            return await client.ExecuteAsync(request);
        }

        public async Task<EquipmentBCDTO> getEquipmentBCAsync(string method, string systemID)
        {
            try
            {
                // Para buscar un equipo específico, usamos el systemId
                var response = await BCRQ(method, $"systemId eq '{systemID}'");
                if (response.IsSuccessful)
                {
                    var result = JsonConvert.DeserializeObject<EquipmentBCResponse>(response.Content);
                    return result?.Value?.FirstOrDefault();
                }
                throw new Exception($"Error obteniendo equipo: {response.ErrorMessage}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en getEquipmentBCAsync: {ex.Message}", ex);
            }
        }

        public async Task<List<Equipment>> getEquipmetListBCAsync(string method)
        {
            try
            {
                var response = await BCRQ(method, null);
                if (response.IsSuccessful)
                {
                    var result = JsonConvert.DeserializeObject<EquipmentBCResponse>(response.Content);
                    if (result?.Value == null)
                    {
                        return new List<Equipment>();
                    }
                    return result.Value.Select(e => new Equipment
                    {
                        Name = e.Name ?? "",
                        Abc = e.Abc ?? "",
                        Branch = e.Branch ?? "",
                        Brand = e.Brand ?? "",
                        Code = e.Code ?? "",
                        Description = e.Description ?? "",
                        DestinationBranch = e.DestinationBranch ?? "",
                        EndDate = e.EndDate,
                        EstimatedTime = e.EstimatedTime ?? "",
                        InitDate = e.InitDate,
                        SystemIdBC = e.SystemId ?? "",
                        IsActive = e.IsActive,
                        LoanDate = e.LoanDate,
                        Model = e.Model ?? "",
                        ProductLine = e.ProductLine ?? "",
                        ReturnDate = e.ReturnDate,
                        ShortName = e.ShortName ?? "",
                        Status = e.Status ?? "",
                        TechSpec = e.TechSpec ?? "",
                        Type = e.Type ?? "",
                        Vendor = e.Vendor ?? "",
                        NoBoxes = e.NoBoxes ?? 0,
                        LastPreventiveMaintenance = new DateTime(1753, 1, 1), // Valor por defecto para campos no nullable
                        LastMaintenance = new DateTime(1753, 1, 1), // Valor por defecto para campos no nullable
                        Alert = e.Alert ?? "",
                        LocationCode = e.LocationCode ?? "",
                        TransferStatus = e.TransferStatus ?? ""
                    }).ToList();
                }
                throw new Exception($"Error obteniendo equipos: {response.StatusCode} - {response.Content}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en getEquipmetListBCAsync: {ex.Message}", ex);
            }
        }

        public async Task<List<EntryRequestAssembly>> GetEntryReqAssembly(string method, string equipmentId, string salesPrice)
        {
            try
            {
                List<EntryRequestAssembly> lData = new List<EntryRequestAssembly>();

                string filter = $"equipment eq '{equipmentId}'";
                if (!string.IsNullOrEmpty(salesPrice) && (method == "lylassembly" || method == "lylassemblyV2"))
                    filter = $"equipment eq '{equipmentId}' and salesCode eq '{salesPrice}'";
                
                var response = await BCRQ(method, filter);
                if (!response.IsSuccessful)
                {
                    throw new Exception($"Error en la llamada a BC: {response.StatusCode} - {response.Content}");
                }

                var result = JsonConvert.DeserializeObject<EntryRequestAssemblyBCResponse>(response.Content);
                if (result?.value == null)
                {
                    return new List<EntryRequestAssembly>();
                }

                foreach(var row in result.value)
                {
                    if((method == "lylassemblyV2" && row.reservedQuantity == 0) || (method != "lylassemblyV2"))
                    {
                        var entryRequestAssembly = new EntryRequestAssembly
                        {
                            Code = row.no,
                            Description = row.description,
                            AssemblyNo = row.documentNo,
                            ReservedQuantity = row.reservedQuantity,
                            UnitPrice = 0,
                            LineNo = row.line_No_ ?? 0,
                            Position = row.position ?? 0,
                            TaxCode = row.taxCode ?? "",
                            RSClasifRegistro = row.rsClasifRegistro ?? "",
                            LowTurnover = row.llRot ?? false,
                            ShortDesc = row.description2,
                            Quantity = row.quantity,
                            LocationCode = row.locationCode
                        };

                        if (row.rsFechaVencimiento != null && row.rsFechaVencimiento != "0001-01-01")
                        {
                            if (DateTime.TryParse(row.rsFechaVencimiento, out DateTime fechaVencimiento))
                            {
                                entryRequestAssembly.RSFechaVencimiento = fechaVencimiento;
                            }
                        }

                        if (method == "lylassemblyeq")
                        {
                            entryRequestAssembly.Quantity = row.quantity;
                            entryRequestAssembly.Quantity_ile = row.remainingQuantity;
                            entryRequestAssembly.Location_Code_ile = row.locationCodeile;
                            entryRequestAssembly.LocationCode = row.locationCode;
                            if (row.reservedQuantityile.HasValue)
                            {
                                entryRequestAssembly.Quantity_ile = entryRequestAssembly.Quantity_ile - row.reservedQuantityile.Value;
                            }
                        }
                        else if (method != "lylassemblyolines")
                        {
                            entryRequestAssembly.Classification = row.llClasif;
                            entryRequestAssembly.Status = row.llStatus;
                            entryRequestAssembly.Quantity = (int)row.quantity;

                            if (!string.IsNullOrEmpty(row.lotNo))
                            {
                                entryRequestAssembly.ReservedQuantity = row.quantityToConsume;
                                entryRequestAssembly.Quantity = row.quantityToConsume;
                                entryRequestAssembly.Quantity_ile = row.quantityile;
                                entryRequestAssembly.Location_Code_ile = row.locationCodeile;
                                entryRequestAssembly.Lot = row.lotNo;
                                entryRequestAssembly.Invima = row.invima;
                                entryRequestAssembly.UnitPrice = row.unitPriceSales ?? row.unitCost ?? 0;
                                entryRequestAssembly.QuantityConsumed = 0;

                                if (!string.IsNullOrEmpty(row.expirationDate) && row.expirationDate != "0001-01-01")
                                {
                                    if (DateTime.TryParse(row.expirationDate, out DateTime expirationDate))
                                    {
                                        entryRequestAssembly.ExpirationDate = expirationDate;
                                    }
                                }
                                else
                                {
                                    entryRequestAssembly.ExpirationDate = new DateTime(2028, 1, 1);
                                }
                            }
                            else
                            {
                                entryRequestAssembly.Lot = "";
                                entryRequestAssembly.Invima = "";
                                entryRequestAssembly.Location_Code_ile = "";
                                entryRequestAssembly.QuantityConsumed = 0;
                                entryRequestAssembly.Quantity_ile = 0;
                            }
                        }

                        lData.Add(entryRequestAssembly);
                    }
                    else if(method == "lylassemblyV2" && (row.quantity != row.reservedQuantity))
                    {
                        var entryRequestAssembly = new EntryRequestAssembly
                        {
                            Code = row.no,
                            Description = row.description,
                            AssemblyNo = row.documentNo,
                            ReservedQuantity = 0,
                            LineNo = row.line_No_ ?? 0,
                            Position = row.position ?? 0,
                            TaxCode = row.taxCode ?? "",
                            RSClasifRegistro = row.rsClasifRegistro ?? "",
                            LowTurnover = row.llRot ?? false,
                            ShortDesc = row.description2,
                            Classification = row.llClasif,
                            Status = row.llStatus,
                            Quantity = (int)(row.quantity - row.reservedQuantity),
                            Lot = "",
                            Invima = "",
                            UnitPrice = row.unitPriceSales ?? 0,
                            Location_Code_ile = "",
                            QuantityConsumed = 0,
                            Quantity_ile = 0
                        };

                        if (row.rsFechaVencimiento != null && row.rsFechaVencimiento != "0001-01-01")
                        {
                            if (DateTime.TryParse(row.rsFechaVencimiento, out DateTime fechaVencimiento))
                            {
                                entryRequestAssembly.RSFechaVencimiento = fechaVencimiento;
                            }
                        }

                        lData.Add(entryRequestAssembly);
                    }
                }

                return lData;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener el ensamble del equipo: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Consulta de contacto de cliente a BC
        /// </summary>
        /// <param name="method">Metodo</param>
        /// <param name="systemID">SystemId del reguistro en BC</param>
        /// <returns>CustomerContact</returns>
        public async Task<CustomerContact> getCustContBCAsync(string method, string systemID)
        {
            try
            {
                var response = await BCRQ(method + "?$filter=systemId eq (" + systemID + ")", "");
                var resValues = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(response.Content);
                IEnumerable<object> data = JsonConvert.DeserializeObject(resValues["value"].ToString());
                CustomerContact customer = null;
                if (data != null)
                {
                    foreach (dynamic v in data)
                    {
                        customer = new CustomerContact();
                        customer.Code = v.no;
                        customer.Name = v.searchName;
                        customer.Identification = v.vatRegistrationNo;
                        customer.Phone = v.phoneNo;
                        customer.CustomerName = v.companyName;
                        customer.Email = v.eMail;
                        customer.systemIdBC = v.systemId;
                        break;
                    }
                }
                return customer;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Consulta de contactos de clientes a BC (todos)
        /// </summary>
        /// <param name="method">Método</param>
        /// <returns>Lista CustomerContact</returns>
        public async Task<List<CustomerContact>> GetCustContListAsync(string method)
        {
            try
            {
                List<CustomerContact> lCustomer = new List<CustomerContact>();
                var response = await BCRQ(method, "");
                var resValues = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(response.Content);
                IEnumerable<object> data = JsonConvert.DeserializeObject(resValues["value"].ToString());
                CustomerContact customer;
                if (data != null)
                {
                    foreach (dynamic v in data)
                    {
                        customer = new CustomerContact();
                        customer.Code = v.no;
                        customer.Name = v.searchName;
                        customer.Identification = v.vatRegistrationNo;
                        customer.Phone = v.phoneNo;
                        customer.CustomerName = v.companyName;
                        customer.Email = v.eMail;
                        customer.systemIdBC = v.systemId;
                        lCustomer.Add(customer);
                    }
                }
                return lCustomer;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Obtiene todos los artículos desde Business Central
        /// </summary>
        /// <param name="method">Método a llamar en BC</param>
        /// <returns>Lista de artículos</returns>
        public async Task<List<ItemsBC>> GetItems(string method)
        {
            try
            {
                List<ItemsBC> lData = new List<ItemsBC>();
                ItemsBC item = null;
                string dataNext = "";
                do
                {                 
                    RestResponse response;
                    
                    if (string.IsNullOrEmpty(dataNext))
                    {
                        // Primera llamada
                        response = await BCRQ(method, "");
                    }
                    else
                    {
                        // Llamadas de paginación - usar la URL completa del nextLink
                        var token = await BCRQ_GETTOKEN();
                        var client = new RestClient(dataNext);
                        var request = new RestRequest();
                        request.AddHeader("Authorization", $"Bearer {token}");
                        request.AddHeader("Accept", "application/json");
                        response = await client.ExecuteAsync(request);
                    }
                    
                    var resValues = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(response.Content);
                    IEnumerable<object> data = JsonConvert.DeserializeObject(resValues["value"].ToString());

                    try
                    {
                        if (resValues["@odata.nextLink"] != null)
                        {
                            dataNext = resValues["@odata.nextLink"].ToString();
                        }
                        else
                        {
                            dataNext = "";
                        }
                    }
                    catch (Exception)
                    {
                        dataNext = "";
                    }

                    if (data != null)
                    {
                        foreach (dynamic v in data)
                        {
                            item = new ItemsBC();
                            item.Code = v.no;
                            item.Name = v.description;
                            // item.PriceList = v.salesCode;
                            item.TaxCode = v.taxCode;
                            if (v.vendorItemNo != null)
                                item.ShortDesc = v.vendorItemNo;
                            if (v.invima != null)
                                item.Invima = v.invima;
                            lData.Add(item);
                        }
                    }
                } while (!string.IsNullOrEmpty(dataNext));

                return lData;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<ItemsBCWithPriceListDTO>> GetItemsWithPriceList(int? take = null)
        {
            try
            {
                List<ItemsBCWithPriceListDTO> lData = new List<ItemsBCWithPriceListDTO>();
                var response = await BCRQ("lylitemsum", "");
                string dataNext = "";
                bool reachedTake = false;
                do
                {
                    var resValues = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(response.Content);
                    IEnumerable<object> data = JsonConvert.DeserializeObject(resValues["value"].ToString());

                    if (data != null)
                    {
                        foreach (dynamic v in data)
                        {
                            var item = new ItemsBCWithPriceListDTO
                            {
                                No = v.no,
                                Description = v.description,
                                BaseUnitMeasure = v.baseUnitMeasure,
                                UnitCost = v.unitCost != null ? (decimal)v.unitCost : 0,
                                PriceIncludesVAT = v.priceIncludesVAT != null ? (bool)v.priceIncludesVAT : false,
                                SalesCode = v.salesCode,
                                UnitPrice = v.unitPrice != null ? (decimal)v.unitPrice : 0,
                                UnitMeasureCode = v.unitMeasureCode,
                                AuxiliaryIndex1 = v.auxiliaryIndex1,
                                AuxiliaryIndex2 = v.auxiliaryIndex2,
                                AuxiliaryIndex3 = v.auxiliaryIndex3
                            };
                            lData.Add(item);
                            if (take.HasValue && lData.Count >= take.Value)
                            {
                                reachedTake = true;
                                break;
                            }
                        }
                    }

                    if (reachedTake)
                        break;

                    try
                    {
                        if (resValues["@odata.nextLink"] != null)
                        {
                            dataNext = resValues["@odata.nextLink"].ToString();
                            if (!string.IsNullOrEmpty(dataNext))
                            {
                                var client = new RestClient(dataNext);
                                var request = new RestRequest();
                                request.AddHeader("Accept", "application/json");
                                var token = await BCRQ_GETTOKEN();
                                request.AddHeader("Authorization", $"Bearer {token}");
                                response = await client.ExecuteAsync(request);
                            }
                        }
                        else
                        {
                            dataNext = "";
                        }
                    }
                    catch (Exception)
                    {
                        dataNext = "";
                    }
                } while (!string.IsNullOrEmpty(dataNext));

                return lData;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Obtiene componentes de EntryRequest desde Business Central
        /// </summary>
        /// <param name="method">Método a llamar en BC</param>
        /// <param name="location">Ubicación (opcional)</param>
        /// <param name="stock">Stock (opcional)</param>
        /// <param name="salesCode">Código de venta (opcional)</param>
        /// <returns>Lista de componentes</returns>
        public async Task<List<EntryRequestComponents>> GetComponents(string method, string location, string stock, string salesCode)
        {
            try
            {
                List<EntryRequestComponents> lData = new List<EntryRequestComponents>();
                EntryRequestComponents reqcomponents;
                string dataNext = "";
                int maxRetries = 3;
                int currentRetry = 0;

                do
                {
                    try
                    {
                        RestResponse response;
                        
                        if (string.IsNullOrEmpty(dataNext))
                        {
                            // Primera llamada - construir filtro
                            var filterConditions = new List<string>();
                            
                            // Siempre filtrar por cantidad mayor a 0
                            filterConditions.Add("quantity ne 0");
                            
                            // Agregar filtro de salesCode solo si se proporciona
                            if (!string.IsNullOrEmpty(salesCode))
                            {
                                filterConditions.Add("salesCode eq '" + salesCode + "'");
                            }
                            
                            // Agregar filtro de location/stock si se proporcionan
                            if (!string.IsNullOrEmpty(location) || !string.IsNullOrEmpty(stock))
                            {
                                var locationConditions = new List<string>();
                                
                                if (!string.IsNullOrEmpty(location))
                                {
                                    locationConditions.Add("location eq '" + location + "'");
                                }
                                
                                if (!string.IsNullOrEmpty(stock))
                                {
                                    locationConditions.Add("location eq '" + stock + "'");
                                }
                                
                                if (locationConditions.Count > 0)
                                {
                                    filterConditions.Add("(" + string.Join(" or ", locationConditions) + ")");
                                }
                            }
                            
                            var filter = string.Join(" and ", filterConditions);
                            response = await BCRQ(method, filter);
                        }
                        else
                        {
                            // Llamadas de paginación - usar la URL completa del nextLink
                            var token = await BCRQ_GETTOKEN();
                            var client = new RestClient(dataNext);
                            var request = new RestRequest();
                            request.AddHeader("Authorization", $"Bearer {token}");
                            request.AddHeader("Accept", "application/json");
                            response = await client.ExecuteAsync(request);
                        }
                        
                        if (!response.IsSuccessful)
                        {
                            throw new Exception($"Error en la respuesta de BC: {response.StatusCode} - {response.Content}");
                        }
                        
                        var resValues = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(response.Content);
                        
                        if (resValues == null || !resValues.ContainsKey("value"))
                        {
                            throw new Exception("Respuesta inválida de Business Central");
                        }
                        
                        IEnumerable<object> data = JsonConvert.DeserializeObject(resValues["value"].ToString());

                        try
                        {
                            if (resValues["@odata.nextLink"] != null)
                            {
                                dataNext = resValues["@odata.nextLink"].ToString();
                            }
                            else
                            {
                                dataNext = "";
                            }
                        }
                        catch (Exception)
                        {
                            dataNext = "";
                        }
                        
                        if (data != null)
                        {
                            foreach (dynamic v in data)
                            {
                                reqcomponents = new EntryRequestComponents();
                                reqcomponents.ItemNo = v.no;
                                reqcomponents.ItemName = v.description;
                                reqcomponents.Warehouse = v.location;
                                reqcomponents.UnitPrice = v.unitPrice;

                                // Calcular cantidad disponible (quantity - reservedQuantity)
                                if (v.reservedQuantity != null)
                                    reqcomponents.Quantity = v.quantity - v.reservedQuantity;
                                else
                                    reqcomponents.Quantity = v.quantity;

                                if (v.salesCode != null)
                                {
                                    reqcomponents.SystemId = v.salesCode;
                                }

                                lData.Add(reqcomponents);
                            }
                        }
                        
                        // Reset retry counter on successful request
                        currentRetry = 0;
                    }
                    catch (Exception ex)
                    {
                        currentRetry++;
                        Console.WriteLine($"Error en GetComponents (intento {currentRetry}/{maxRetries}): {ex.Message}");
                        
                        if (currentRetry >= maxRetries)
                        {
                            throw new Exception($"Error después de {maxRetries} intentos: {ex.Message}", ex);
                        }
                        
                        // Esperar antes de reintentar (backoff exponencial)
                        await Task.Delay(1000 * currentRetry);
                    }
                } while (!string.IsNullOrEmpty(dataNext) && currentRetry < maxRetries);         
              
                return lData;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error crítico en GetComponents: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Obtiene líneas de ensamble desde Business Central
        /// </summary>
        /// <param name="method">Método a llamar en BC</param>
        /// <param name="documentNo">Número de documento (opcional)</param>
        /// <returns>Lista de componentes de líneas de ensamble</returns>
        public async Task<List<EntryRequestComponents>> GetAssemblyLines(string method, string documentNo = null)
        {
            try
            {
                List<EntryRequestComponents> lData = new List<EntryRequestComponents>();
                EntryRequestComponents reqcomponents;
                string dataNext = "";
                do
                {
                    RestResponse response;
                    
                    if (string.IsNullOrEmpty(dataNext))
                    {
                        // Primera llamada - construir filtro
                        var filterConditions = new List<string>();
                        
                        // Agregar filtro de documento solo si se proporciona
                        if (!string.IsNullOrEmpty(documentNo))
                        {
                            filterConditions.Add("documentNo eq '" + documentNo + "'");
                        }
                        
                        var filter = filterConditions.Count > 0 ? string.Join(" and ", filterConditions) : "";
                        response = await BCRQ(method, filter);
                    }
                    else
                    {
                        // Llamadas de paginación - usar la URL completa del nextLink
                        var token = await BCRQ_GETTOKEN();
                        var client = new RestClient(dataNext);
                        var request = new RestRequest();
                        request.AddHeader("Authorization", $"Bearer {token}");
                        request.AddHeader("Accept", "application/json");
                        response = await client.ExecuteAsync(request);
                    }
                    var resValues = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(response.Content);
                    IEnumerable<object> data = JsonConvert.DeserializeObject(resValues["value"].ToString());

                    try
                    {
                        if (resValues["@odata.nextLink"] != null)
                        {
                            dataNext = resValues["@odata.nextLink"].ToString();
                        }
                        else
                        {
                            dataNext = "";
                        }
                    }
                    catch (Exception)
                    {
                        dataNext = "";
                    }
                    
                    if (data != null)
                    {
                        foreach (dynamic v in data)
                        {
                            reqcomponents = new EntryRequestComponents();
                            reqcomponents.ItemNo = v.no;
                            reqcomponents.ItemName = v.description;
                            reqcomponents.Quantity = v.quantityToConsume;
                            reqcomponents.Warehouse = v.Location_Code;
                            reqcomponents.AssemblyNo = v.documentNo;
                            reqcomponents.status = v.documentType;
                            
                            // Mapear campos adicionales si están disponibles
                            if (v.lineNo != null)
                            {
                                reqcomponents.IdEntryReq = (int)v.lineNo;
                            }
                            
                            if (v.Unit_of_Measure_Code != null)
                            {
                                reqcomponents.shortDesc = v.Unit_of_Measure_Code;
                            }

                            lData.Add(reqcomponents);
                        }
                    }
                } while (!string.IsNullOrEmpty(dataNext));         
              
                return lData;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<PaymentTermBCDTO>> GetPaymentTermsBCAsync()
        {
            var response = await BCRQ("lylpaymentterms", null);
            if (response.IsSuccessful)
            {
                var result = JsonConvert.DeserializeObject<PaymentTermBCResponse>(response.Content);
                return result.Value;
            }
            throw new Exception($"Error obteniendo términos de pago: {response.ErrorMessage}");
        }

        /// <summary>
        /// Consultar lotes disponibles de referencia
        /// </summary>
        /// <param name="ItemCode">Referencia</param>
        /// <param name="locationCode">Bodega</param>
        /// <returns>Lista de EntryRequestAssembly con información de lotes</returns>
        public async Task<List<EntryRequestAssembly>> GetLotsAdditionals(string method, string equipmentId, string locationCode)
        {
            try
            {
                List<EntryRequestAssembly> lData = new List<EntryRequestAssembly>();
                var response = await BCRQ(method, "?$filter=(itemNo eq '" + equipmentId + "') and (locationCodeile eq '" + locationCode + "')");
                var resValues = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(response.Content);
                IEnumerable<object> data = JsonConvert.DeserializeObject(resValues["value"].ToString());

                EntryRequestAssembly entryRequestAssembly = new EntryRequestAssembly();
                if (data != null)
                {
                    foreach (dynamic row in data)
                    {
                        if (row.lotNo != "")
                        {
                            entryRequestAssembly = new EntryRequestAssembly();
                            entryRequestAssembly.Code = row.itemNo;
                            entryRequestAssembly.Lot = row.lotNo;
                            entryRequestAssembly.ReservedQuantity = row.reservedQuantityile;
                            entryRequestAssembly.Location_Code_ile = row.locationCodeile;
                            entryRequestAssembly.Quantity_ile = row.quantityile;

                            try
                            {
                                if (row.rsClasifRegistro != null)
                                    entryRequestAssembly.RSClasifRegistro = row.rsClasifRegistro;
                                else
                                    entryRequestAssembly.RSClasifRegistro = "";
                            }
                            catch (Exception)
                            {
                                entryRequestAssembly.RSClasifRegistro = "";
                            }

                            try
                            {
                                if (row.rsFechaVencimiento != null && row.rsFechaVencimiento != "0001-01-01")
                                {
                                    string dateString = row.rsFechaVencimiento.ToString();
                                    string[] dataDatestring = dateString.Split("-");
                                    DateTime dateTime = new DateTime(int.Parse(dataDatestring[0]), int.Parse(dataDatestring[1]), int.Parse(dataDatestring[2]));
                                    entryRequestAssembly.RSFechaVencimiento = dateTime;
                                }
                            }
                            catch (Exception)
                            {
                            }

                            if (row.expirationDate != "" && row.expirationDate != "0001-01-01")
                            {
                                string dateString = row.expirationDate.ToString();
                                string[] dataDatestring = dateString.Split("-");
                                DateTime dateTime = new DateTime(int.Parse(dataDatestring[0]), int.Parse(dataDatestring[1]), int.Parse(dataDatestring[2]));
                                entryRequestAssembly.ExpirationDate = dateTime;
                            }
                            else
                            {
                                DateTime dateTime;
                                DateTime.TryParse("2999-01-01", out dateTime);
                                entryRequestAssembly.ExpirationDate = dateTime;
                            }

                            lData.Add(entryRequestAssembly);
                        }
                    }
                }
                return lData;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }

    public class TokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }
    }

    public class CustomerBCResponse
    {
        [JsonProperty("value")]
        public List<CustomerBCDTO> Value { get; set; }
    }

    public class EquipmentBCResponse
    {
        [JsonProperty("value")]
        public List<EquipmentBCDTO> Value { get; set; }
    }

    public class PaymentTermBCResponse
    {
        [JsonProperty("value")]
        public List<PaymentTermBCDTO> Value { get; set; }
    }
} 