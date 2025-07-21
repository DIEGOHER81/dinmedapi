using DimmedAPI.Entidades;
using DimmedAPI.Interfaces;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DimmedAPI.BO
{
    public class CustomerAddressBO : ICustomerAddressBO
    {
        private readonly ApplicationDBContext _context;
        private readonly IntBCConex _bcConn;

        public CustomerAddressBO(ApplicationDBContext context, IntBCConex bcConn)
        {
            _context = context;
            _bcConn = bcConn;
        }

        /// <summary>
        /// Sincronizar datos de dirección de cliente desde BC, crea o actualiza
        /// </summary>
        public async Task<CustomerAddress> UpdateAddFromBC(string systemID, int option)
        {
            try
            {
                CustomerAddress customerAddress = await _context.CustomerAddress.FirstOrDefaultAsync(x => x.systemIdBC == systemID);
                CustomerAddress customerBC = await getCustAddrBCAsync("shiptotddress", systemID);
                if (option == 1 && customerAddress != null) // actualizar
                {
                    customerAddress.Name = customerBC.Name;
                    customerAddress.Address = customerBC.Address;
                    customerAddress.PostCode = customerBC.PostCode;
                    customerAddress.LocationCode = customerBC.LocationCode;
                    customerAddress.City = customerBC.City;
                    customerAddress.Code = customerBC.Code;
                    customerAddress.Phone = customerBC.Phone;
                    _context.CustomerAddress.Update(customerAddress);
                    await _context.SaveChangesAsync();
                    return customerAddress;
                }
                else if (option == 1 || (option == 2 && customerAddress == null))
                {
                    var customers = await _context.Customer.FirstOrDefaultAsync(x => x.No == customerBC.CustomerNo);
                    if (customers != null)
                    {
                        customerBC.CustomerId = customers.Id;
                    }
                    _context.CustomerAddress.Add(customerBC);
                    await _context.SaveChangesAsync();
                    return customerBC;
                }
                return customerAddress;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Consulta de direccion de cliente a BC
        /// </summary>
        public async Task<CustomerAddress> getCustAddrBCAsync(string method, string systemID)
        {
            try
            {
                var response = await _bcConn.BCRQ(method + "?$filter=systemId eq (" + systemID + ")", "");
                var resValues = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(response.Content);
                IEnumerable<object> data = JsonConvert.DeserializeObject(resValues["value"].ToString());
                CustomerAddress customer = null;
                if (data != null)
                {
                    foreach (dynamic v in data)
                    {
                        customer = new CustomerAddress();
                        customer.Code = v.code;
                        customer.Name = v.name;
                        customer.Address = v.address;
                        customer.City = v.city;
                        customer.PostCode = v.postCode;
                        customer.LocationCode = v.locationCode;
                        customer.systemIdBC = v.systemId;
                        customer.CustomerNo = v.customerNo;
                        customer.Phone = v.phone;
                        break;
                    }
                }
                return customer;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Consulta local de todas las direcciones de clientes
        /// </summary>
        public async Task<List<CustomerAddress>> GetAllAsync()
        {
            return await _context.CustomerAddress.ToListAsync();
        }

        /// <summary>
        /// Consulta local de una dirección de cliente por Id
        /// </summary>
        public async Task<CustomerAddress> GetByIdAsync(int id)
        {
            return await _context.CustomerAddress.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<CustomerAddress>> GetCustAddrListAsync(string method)
        {
            try
            {
                List<CustomerAddress> lCustomer = new List<CustomerAddress>();
                var response = await _bcConn.BCRQ(method, "");
                var resValues = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(response.Content);
                IEnumerable<object> data = JsonConvert.DeserializeObject(resValues["value"].ToString());
                CustomerAddress customer;
                if (data != null)
                {
                    foreach (dynamic v in data)
                    {
                        customer = new CustomerAddress();
                        customer.Code = v.code;
                        customer.Name = v.name;
                        customer.Address = v.address;
                        customer.City = v.city;
                        customer.PostCode = v.postCode;
                        customer.LocationCode = v.locationCode;
                        customer.systemIdBC = v.systemId;
                        customer.CustomerNo = v.customerNo;
                        customer.Phone = v.phone;
                        lCustomer.Add(customer);
                    }
                }
                return lCustomer;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Consulta de direcciones desde BC y mapea CustomerId local
        /// </summary>
        public async Task<List<CustomerAddress>> SinCustAddressBCAsync()
        {
            try
            {
                List<CustomerAddress> customerAddress = new List<CustomerAddress>();
                var customers = _context.Customer.ToList();
                if (customers.Any())
                {
                    customerAddress = await GetCustAddrListAsync("shiptotddress");
                    if (customerAddress != null && customerAddress.Count > 0)
                    {
                        foreach (var CA in customerAddress)
                        {
                            var customer = customers.Find(x => x.No == CA.CustomerNo);
                            if (customer != null)
                                CA.CustomerId = customer.Id;
                        }
                    }
                }
                return customerAddress;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Sincroniza todas las direcciones desde BC y las almacena en la base local (crea o actualiza)
        /// </summary>
        public async Task<List<CustomerAddress>> SyncAllFromBCAsync()
        {
            var bcAddresses = await SinCustAddressBCAsync();
            if (bcAddresses == null || bcAddresses.Count == 0)
                return new List<CustomerAddress>();

            foreach (var bcAddr in bcAddresses)
            {
                var local = await _context.CustomerAddress.FirstOrDefaultAsync(x => x.systemIdBC == bcAddr.systemIdBC);
                if (local != null)
                {
                    // Actualizar
                    local.Code = bcAddr.Code;
                    local.Name = bcAddr.Name;
                    local.Address = bcAddr.Address;
                    local.City = bcAddr.City;
                    local.PostCode = bcAddr.PostCode;
                    local.LocationCode = bcAddr.LocationCode;
                    local.CustomerId = bcAddr.CustomerId;
                    local.CustomerNo = bcAddr.CustomerNo;
                    local.Phone = bcAddr.Phone;
                }
                else
                {
                    // Crear
                    _context.CustomerAddress.Add(bcAddr);
                }
            }
            await _context.SaveChangesAsync();
            return await _context.CustomerAddress.ToListAsync();
        }
    }
} 