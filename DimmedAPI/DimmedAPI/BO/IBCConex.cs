using DimmedAPI.Entidades;
using DimmedAPI.DTOs;
using RestSharp;

namespace DimmedAPI.BO
{
    public interface IntBCConex
    {
        Task<string> BCRQ_GETTOKEN();
        Task<string> BCRQ_GETTOKEN_BY_USER(string username, string password);
        Task<RestResponse> BCRQ(string method, string filter);
        Task<RestResponse> BCRQ_post(string method, string filter, string body);
        Task<RestResponse> BCRQ_postDeletePD(string method, string filter, string body);
        Task<CustomerBCDTO> getCustomerBCAsync(string method, string systemID);
        Task<EquipmentBCDTO> getEquipmentBCAsync(string method, string systemID);
        Task<List<Customer>> ConnBCAsync(string method);
        Task<List<Equipment>> getEquipmetListBCAsync(string method);
        Task<List<EntryRequestAssembly>> GetEntryReqAssembly(string method, string equipmentId, string salesPrice);
        Task<RestResponse> SPLE_API(string method, string filter, string type, string body);
        Task<CustomerContact> getCustContBCAsync(string method, string systemID);
        Task<List<ItemsBC>> GetItems(string method);
    }
}
