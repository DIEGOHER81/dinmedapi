using DimmedAPI.Entidades;
using DimmedAPI.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace DimmedAPI.BO
{
    public class EntryRequestBO
    {
        private readonly ApplicationDBContext _context;
        private readonly IntBCConex _bcConn;

        public EntryRequestBO(ApplicationDBContext context, IntBCConex bcConn)
        {
            _context = context;
            _bcConn = bcConn;
        }

        /// <summary>
        /// Obtiene un EntryRequest por ID
        /// </summary>
        /// <param name="id">ID del EntryRequest</param>
        /// <returns>EntryRequest encontrado</returns>
        public async Task<EntryRequests?> GetByIdAsync(int id)
        {
            return await _context.EntryRequests
                .Include(er => er.EntryRequestComponents)
                .Include(er => er.Branch)
                .FirstOrDefaultAsync(er => er.Id == id);
        }

        /// <summary>
        /// Actualiza un EntryRequest
        /// </summary>
        /// <param name="entryRequest">EntryRequest a actualizar</param>
        /// <returns>Task</returns>
        public async Task UpdateAsync(EntryRequests entryRequest)
        {
            _context.EntryRequests.Update(entryRequest);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Enviar pedido de ensamblado a BC
        /// </summary>
        /// <param name="entry">Objeto EntryRequests con los datos del pedido</param>
        /// <returns>Texto, PEDIDO REGISTRADO|ERROR</returns>
        public async Task<string> sendAssemblyBC(EntryRequests entry)
        {
            try
            {
                AssemblyApiBC_Header entryRequestApiBC_Header = new AssemblyApiBC_Header();
                entryRequestApiBC_Header.documentNo = "P-" + entry.Id;
                entryRequestApiBC_Header.branch = entry.Branch?.Name ?? "";
                entryRequestApiBC_Header.itemNo = "9999";
                entryRequestApiBC_Header.lines = new List<AssemblyApiBC_Line>();

                bool consume = false;

                if (entry.EntryRequestComponents != null && entry.EntryRequestComponents.Count > 0)
                {
                    foreach (var component in entry.EntryRequestComponents)
                    {
                        decimal? totalQ = entry.EntryRequestComponents.ToList().FindAll(x => x.ItemNo == component.ItemNo).Sum(x => x.Quantity);
                        if (totalQ != null)
                        {
                            entryRequestApiBC_Header.lines.Add(new AssemblyApiBC_Line
                            {
                                documentNo = "P-" + entry.Id,
                                itemNo = component.ItemNo ?? "",
                                quantity = decimal.ToInt32((decimal)component.Quantity),
                                price = component.UnitPrice ?? 0,
                                locationCode = component.Warehouse ?? "",
                                lotNo = component.Lot ?? "",
                                quantityLine = decimal.ToInt32((decimal)totalQ),
                                idWeb = component.Id
                            });
                            consume = true;
                        }
                    }
                }

                if (!consume)
                {
                    return "ERROR: No hay componentes para procesar";
                }

                var jsonEntry = JsonSerializer.Serialize(entryRequestApiBC_Header);
                var response = await _bcConn.BCRQ_post("lylgenassembly?$expand=lines", "", jsonEntry);

                if (!response.IsSuccessful)
                {
                    return "ERROR: " + response.Content;
                }
                else
                {
                    // Crear equipo
                    EntryRequestDetails equipmntAd = new EntryRequestDetails
                    {
                        IdEntryReq = entry.Id,
                        IdEquipment = 1713, // TODO: buscar el id correcto
                        IsComponent = true,
                        DateIni = entry.SurgeryInit ?? DateTime.Now,
                        DateEnd = entry.SurgeryEnd ?? DateTime.Now,
                        CreateAt = DateTime.Now,
                        status = "NUEVO",
                        TraceState = ""
                    };

                    _context.EntryRequestDetails.Add(equipmntAd);
                    await _context.SaveChangesAsync();

                    foreach (var component in entry.EntryRequestComponents)
                    {
                        EntryRequestAssembly assembly = new EntryRequestAssembly
                        {
                            IsComponent = true,
                            AssemblyNo = "9910", // TODO: ajustar
                            Classification = "",
                            Code = component.ItemNo ?? "",
                            Description = component.ItemName ?? "",
                            EntryRequestId = entry.Id,
                            EntryRequestDetailId = equipmntAd.Id,
                            LocationCode = component.Warehouse ?? "",
                            Invima = component.Invima ?? "",
                            Location_Code_ile = component.Warehouse ?? "",
                            Quantity = (decimal)component.Quantity,
                            ShortDesc = component.shortDesc ?? "",
                            Lot = component.Lot ?? "",
                            UnitPrice = component.UnitPrice,
                            ReservedQuantity = (decimal)component.Quantity,
                            TaxCode = component.TaxCode ?? "",
                            QuantityConsumed = 0,
                            Quantity_ile = 0,
                            ExpirationDate = DateTime.Now.AddYears(20),
                            Status = "Vigente",
                            Position = component.Id,
                            LowTurnover = false,
                            LineNo = 0

                        };

                        _context.EntryRequestAssembly.Add(assembly);
                    }

                    await _context.SaveChangesAsync();
                }

                return "PEDIDO REGISTRADO";
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al enviar pedido de ensamble a BC: {ex.Message}", ex);
            }
        }
    }
}


