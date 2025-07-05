using DimmedAPI.DTOs;
using DimmedAPI.Entidades;
using DimmedAPI.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace DimmedAPI.BO
{
    public class CustomerBO : ICustomerBO
    {
        private readonly ApplicationDBContext _context;
        private readonly IntBCConex _bcConn;
        

        public CustomerBO(ApplicationDBContext context, IntBCConex bcConn)
        {
            _context = context;
            _bcConn = bcConn;
        }

        public async Task<List<Customer>> SincronizeBCAsync()
        {
            try
            {
                // Obtener todos los clientes de Business Central
                List<Customer> customersBC = await _bcConn.ConnBCAsync("lylcustomer");
                List<Customer> customersUpdated = new List<Customer>();

                foreach (var customerBC in customersBC)
                {
                    // Si Identification viene nulo o vacío, usar el valor de No
                    var identificationValue = string.IsNullOrWhiteSpace(customerBC.Identification) ? customerBC.No : customerBC.Identification;

                    // Buscar si el cliente ya existe
                    var existingCustomer = await _context.Customer
                        .FirstOrDefaultAsync(c => c.SystemIdBc == customerBC.SystemIdBc);

                    if (existingCustomer == null)
                    {
                        // Crear nuevo cliente
                        var newCustomer = new Customer
                        {
                            Identification = identificationValue,
                            IdType = customerBC.IdType,
                            Address = customerBC.Address,
                            City = customerBC.City,
                            Phone = customerBC.Phone,
                            Email = customerBC.Email,
                            CertMant = customerBC.CertMant,
                            RemCustomer = customerBC.RemCustomer,
                            Observations = customerBC.Observations,
                            Name = customerBC.Name,
                            SystemIdBc = customerBC.SystemIdBc,
                            SalesZone = customerBC.SalesZone,
                            TradeRepres = customerBC.TradeRepres,
                            NoCopys = customerBC.NoCopys,
                            IsActive = customerBC.IsActive,
                            Segment = customerBC.Segment,
                            No = customerBC.No,
                            FullName = customerBC.FullName,
                            PriceGroup = customerBC.PriceGroup,
                            ShortDesc = customerBC.ShortDesc,
                            ExIva = customerBC.ExIva,
                            IsSecondPriceList = customerBC.IsSecondPriceList,
                            SecondPriceGroup = customerBC.SecondPriceGroup,
                            InsurerType = customerBC.InsurerType,
                            IsRemLot = customerBC.IsRemLot,
                            LyLOpeningHours1 = customerBC.LyLOpeningHours1,
                            LyLOpeningHours2 = customerBC.LyLOpeningHours2
                        };

                        _context.Customer.Add(newCustomer);
                        customersUpdated.Add(newCustomer);
                    }
                    else
                    {
                        // Actualizar cliente existente
                        existingCustomer.Identification = identificationValue;
                        existingCustomer.IdType = customerBC.IdType;
                        existingCustomer.Address = customerBC.Address;
                        existingCustomer.City = customerBC.City;
                        existingCustomer.Phone = customerBC.Phone;
                        existingCustomer.Email = customerBC.Email;
                        existingCustomer.CertMant = customerBC.CertMant;
                        existingCustomer.RemCustomer = customerBC.RemCustomer;
                        existingCustomer.Observations = customerBC.Observations;
                        existingCustomer.Name = customerBC.Name;
                        existingCustomer.SalesZone = customerBC.SalesZone;
                        existingCustomer.TradeRepres = customerBC.TradeRepres;
                        existingCustomer.NoCopys = customerBC.NoCopys;
                        existingCustomer.IsActive = customerBC.IsActive;
                        existingCustomer.Segment = customerBC.Segment;
                        existingCustomer.No = customerBC.No;
                        existingCustomer.FullName = customerBC.FullName;
                        existingCustomer.PriceGroup = customerBC.PriceGroup;
                        existingCustomer.ShortDesc = customerBC.ShortDesc;
                        existingCustomer.ExIva = customerBC.ExIva;
                        existingCustomer.IsSecondPriceList = customerBC.IsSecondPriceList;
                        existingCustomer.SecondPriceGroup = customerBC.SecondPriceGroup;
                        existingCustomer.InsurerType = customerBC.InsurerType;
                        existingCustomer.IsRemLot = customerBC.IsRemLot;
                        existingCustomer.LyLOpeningHours1 = customerBC.LyLOpeningHours1;
                        existingCustomer.LyLOpeningHours2 = customerBC.LyLOpeningHours2;

                        customersUpdated.Add(existingCustomer);
                    }
                }

                await _context.SaveChangesAsync();
                return customersUpdated;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<object> SincronizarDesdeBC(CustomerBCDTO dto)
        {
            // Si Identification viene nulo o vacío, usar el valor de No
            var identificationValue = string.IsNullOrWhiteSpace(dto.Identification) ? dto.No : dto.Identification;

            // Buscar si el cliente ya existe
            var existingCustomer = await _context.Customer
                .FirstOrDefaultAsync(c => c.SystemIdBc == dto.SystemIdBc);

            if (existingCustomer == null)
            {
                // Crear nuevo
                var newCustomer = new Customer
                {
                    Identification = identificationValue,
                    IdType =  dto.IdType ?? 0,
                    Address = dto.Address,
                    City = dto.City,
                    Phone = dto.Phone,
                    Email = dto.Email,
                    CertMant = dto.CertMant,
                    RemCustomer = dto.RemCustomer,
                    Observations = dto.Observations,
                    Name = dto.Name,
                    SystemIdBc = dto.SystemIdBc,
                    SalesZone = dto.SalesZone,
                    TradeRepres = dto.TradeRepres,
                    NoCopys = dto.NoCopys,
                    IsActive = dto.IsActive,
                    Segment = dto.Segment,
                    No = dto.No,
                    FullName = dto.FullName,
                    PriceGroup = dto.PriceGroup,
                    ShortDesc = dto.ShortDesc,
                    ExIva = dto.ExIva,
                    IsSecondPriceList = dto.IsSecondPriceList,
                    SecondPriceGroup = dto.SecondPriceGroup,
                    InsurerType = dto.InsurerType,
                    IsRemLot = dto.IsRemLot,
                    LyLOpeningHours1 = dto.LyLOpeningHours1,
                    LyLOpeningHours2 = dto.LyLOpeningHours2
                };

                _context.Customer.Add(newCustomer);
                await _context.SaveChangesAsync();
                return newCustomer;
            }
            else
            {
                // Actualizar
                existingCustomer.Identification = identificationValue;
                existingCustomer.IdType = dto.IdType ?? existingCustomer.IdType;
                existingCustomer.Address = dto.Address;
                existingCustomer.City = dto.City;
                existingCustomer.Phone = dto.Phone;
                existingCustomer.Email = dto.Email;
                existingCustomer.CertMant = dto.CertMant;
                existingCustomer.RemCustomer = dto.RemCustomer;
                existingCustomer.Observations = dto.Observations;
                existingCustomer.Name = dto.Name;
                existingCustomer.SalesZone = dto.SalesZone;
                existingCustomer.TradeRepres = dto.TradeRepres;
                existingCustomer.NoCopys = dto.NoCopys;
                existingCustomer.IsActive = dto.IsActive;
                existingCustomer.Segment = dto.Segment;
                existingCustomer.No = dto.No;
                existingCustomer.FullName = dto.FullName;
                existingCustomer.PriceGroup = dto.PriceGroup;
                existingCustomer.ShortDesc = dto.ShortDesc;
                existingCustomer.ExIva = dto.ExIva;
                existingCustomer.IsSecondPriceList = dto.IsSecondPriceList;
                existingCustomer.SecondPriceGroup = dto.SecondPriceGroup;
                existingCustomer.InsurerType = dto.InsurerType;
                existingCustomer.IsRemLot = dto.IsRemLot;
                existingCustomer.LyLOpeningHours1 = dto.LyLOpeningHours1;
                existingCustomer.LyLOpeningHours2 = dto.LyLOpeningHours2;



                await _context.SaveChangesAsync();
                return existingCustomer;
            }
        }

        public async Task<List<Customer>> GetCustomersFromBCAsync(int? take = null, string systemIdBc = null)
        {
            var customers = new List<Customer>();
            if (!string.IsNullOrEmpty(systemIdBc))
            {
                var customer = await _bcConn.getCustomerBCAsync("lylcustomer", systemIdBc);
                if (customer != null)
                {
                    customers.Add(new Customer
                    {
                        SystemIdBc = customer.SystemIdBc,
                        No = customer.No,
                        Name = customer.Name,
                        FullName = customer.FullName,
                        Address = customer.Address,
                        City = customer.City,
                        Phone = customer.Phone,
                        Email = customer.Email,
                        Identification = customer.Identification,
                        IdType = customer.IdType ?? 0,
                        CertMant = customer.CertMant,
                        RemCustomer = customer.RemCustomer,
                        Observations = customer.Observations,
                        SalesZone = customer.SalesZone,
                        TradeRepres = customer.TradeRepres,
                        NoCopys = customer.NoCopys,
                        IsActive = customer.IsActive,
                        Segment = customer.Segment,
                        PriceGroup = customer.PriceGroup,
                        ShortDesc = customer.ShortDesc,
                        ExIva = customer.ExIva,
                        IsSecondPriceList = customer.IsSecondPriceList,
                        SecondPriceGroup = customer.SecondPriceGroup,
                        InsurerType = customer.InsurerType,
                        IsRemLot = customer.IsRemLot,
                        LyLOpeningHours1 = customer.LyLOpeningHours1,
                        LyLOpeningHours2 = customer.LyLOpeningHours2
        
                    });
                }
            }
            else
            {
                var allCustomers = await _bcConn.ConnBCAsync("lylcustomer");
                if (take.HasValue)
                    customers = allCustomers.Take(take.Value).ToList();
                else
                    customers = allCustomers;
            }
            return customers;
        }
    }
}
