using DimmedAPI.DTOs;
using DimmedAPI.Entidades;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DimmedAPI.BO
{
    /// <summary>
    /// Business Object para la validación de agendamiento de equipos
    /// </summary>
    public class EquipmentSchedulingBO
    {
        private readonly ApplicationDBContext _context;

        public EquipmentSchedulingBO(ApplicationDBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Valida si es permitido el agendamiento de un equipo en un rango de fechas específico
        /// </summary>
        /// <param name="request">Parámetros de validación</param>
        /// <returns>Resultado de la validación</returns>
        public async Task<EquipmentSchedulingValidationResponseDTO> ValidateEquipmentSchedulingAsync(EquipmentSchedulingValidationRequestDTO request)
        {
            try
            {
                // Validar que el equipo existe
                var equipment = await _context.Equipment
                    .FromSqlRaw(@"
                        SELECT 
                            Id,
                            ISNULL(Code, '') as Code,
                            ISNULL(Name, '') as Name,
                            ISNULL(ShortName, '') as ShortName,
                            ISNULL(Status, '') as Status,
                            ISNULL(ProductLine, '') as ProductLine,
                            ISNULL(Branch, '') as Branch,
                            ISNULL(EstimatedTime, '') as EstimatedTime,
                            ISNULL(Description, '') as Description,
                            IsActive,
                            ISNULL(TechSpec, '') as TechSpec,
                            ISNULL(DestinationBranch, '') as DestinationBranch,
                            LoanDate,
                            ReturnDate,
                            InitDate,
                            ISNULL(Vendor, '') as Vendor,
                            ISNULL(Brand, '') as Brand,
                            ISNULL(Model, '') as Model,
                            ISNULL(Abc, '') as Abc,
                            EndDate,
                            ISNULL(Type, '') as Type,
                            ISNULL(SystemIdBC, '') as SystemIdBC,
                            ISNULL(NoBoxes, 0) as NoBoxes,
                            ISNULL(LastPreventiveMaintenance, '1753-01-01') as LastPreventiveMaintenance,
                            ISNULL(LastMaintenance, '1753-01-01') as LastMaintenance,
                            ISNULL(Alert, '') as Alert,
                            ISNULL(LocationCode, '') as LocationCode,
                            ISNULL(TransferStatus, '') as TransferStatus
                        FROM Equipment 
                        WHERE Id = {0}", request.IdEquipment)
                    .FirstOrDefaultAsync();

                if (equipment == null)
                {
                    return new EquipmentSchedulingValidationResponseDTO
                    {
                        IsAllowed = false,
                        Message = $"El equipo con ID {request.IdEquipment} no existe",
                        IdEquipment = request.IdEquipment,
                        DateIni = request.DateIni,
                        DateEnd = request.DateEnd
                    };
                }

                // Obtener todos los EntryRequestDetails relacionados con el equipo en el rango de fechas
                // Excluir el pedido actual si se proporciona IdEntryReq
                var sqlQuery = @"
                    SELECT 
                        erd.Id,
                        erd.IdEntryReq,
                        erd.IdEquipment,
                        erd.CreateAt,
                        erd.DateIni,
                        erd.DateEnd,
                        ISNULL(erd.status, '') as status,
                        erd.DateLoadState,
                        ISNULL(erd.TraceState, '') as TraceState,
                        erd.IsComponent
                    FROM EntryRequestDetails erd
                    WHERE erd.IdEquipment = {0}
                      AND (
                           {1} <= erd.DateEnd
                       AND {2} >= erd.DateIni
                      )";

                // Si se proporciona IdEntryReq, excluir ese pedido de la consulta
                if (request.IdEntryReq.HasValue)
                {
                    sqlQuery += " AND erd.IdEntryReq != {3}";
                }

                var relatedSchedulings = await _context.EntryRequestDetails
                    .FromSqlRaw(sqlQuery, 
                        request.IdEquipment, 
                        request.DateIni, 
                        request.DateEnd,
                        request.IdEntryReq ?? (object)DBNull.Value)
                    .ToListAsync();

                // Verificar si existe algún conflicto con status 'NUEVO'
                var conflictingScheduling = relatedSchedulings.FirstOrDefault(rs => rs.status == "NUEVO");

                // Obtener información de los pedidos principales para los agendamientos relacionados
                var relatedOrders = new List<EquipmentSchedulingRelatedOrderDTO>();
                
                if (relatedSchedulings.Any())
                {
                    var entryRequestIds = relatedSchedulings.Select(rs => rs.IdEntryReq).Distinct().ToList();
                    var entryRequests = await _context.EntryRequests
                        .FromSqlRaw($@"
                            SELECT 
                                Id,
                                Date,
                                ISNULL(Service, '') as Service,
                                IdOrderType,
                                ISNULL(DeliveryPriority, '') as DeliveryPriority,
                                IdCustomer,
                                InsurerType,
                                Insurer,
                                IdMedic,
                                IdPatient,
                                ISNULL(Applicant, '') as Applicant,
                                IdATC,
                                ISNULL(LimbSide, '') as LimbSide,
                                DeliveryDate,
                                ISNULL(OrderObs, '') as OrderObs,
                                SurgeryTime,
                                SurgeryInit,
                                SurgeryEnd,
                                ISNULL(Status, '') as Status,
                                IdTraceStates,
                                BranchId,
                                SurgeryInitTime,
                                SurgeryEndTime,
                                ISNULL(DeliveryAddress, '') as DeliveryAddress,
                                ISNULL(PurchaseOrder, '') as PurchaseOrder,
                                AtcConsumed,
                                IsSatisfied,
                                ISNULL(Observations, '') as Observations,
                                ISNULL(obsMaint, '') as obsMaint,
                                AuxLog,
                                IdCancelReason,
                                IdCancelDetail,
                                ISNULL(CancelReason, '') as CancelReason,
                                ISNULL(CancelDetail, '') as CancelDetail,
                                Notification,
                                IsReplacement,
                                AssemblyComponents,
                                ISNULL(priceGroup, '') as priceGroup
                            FROM EntryRequests 
                            WHERE Id IN ({string.Join(",", entryRequestIds)})")
                        .ToListAsync();

                    foreach (var scheduling in relatedSchedulings)
                    {
                        var entryRequest = entryRequests.FirstOrDefault(er => er.Id == scheduling.IdEntryReq);
                        
                        relatedOrders.Add(new EquipmentSchedulingRelatedOrderDTO
                        {
                            Id = scheduling.Id,
                            IdEntryReq = scheduling.IdEntryReq,
                            IdEquipment = scheduling.IdEquipment,
                            CreateAt = scheduling.CreateAt,
                            DateIni = scheduling.DateIni,
                            DateEnd = scheduling.DateEnd,
                            Status = scheduling.status,
                            DateLoadState = scheduling.DateLoadState,
                            TraceState = scheduling.TraceState,
                            IsComponent = scheduling.IsComponent,
                            sInformation = scheduling.sInformation,
                            Name = scheduling.Name,
                            EquipmentName = equipment.Name,
                            EquipmentCode = equipment.Code,
                            OrderInfo = entryRequest != null ? new EquipmentSchedulingOrderInfoDTO
                            {
                                Id = entryRequest.Id,
                                Date = entryRequest.Date,
                                Service = entryRequest.Service,
                                IdOrderType = entryRequest.IdOrderType,
                                DeliveryPriority = entryRequest.DeliveryPriority,
                                IdCustomer = entryRequest.IdCustomer,
                                Applicant = entryRequest.Applicant,
                                DeliveryDate = entryRequest.DeliveryDate,
                                OrderObs = entryRequest.OrderObs,
                                Status = entryRequest.Status,
                                DeliveryAddress = entryRequest.DeliveryAddress,
                                PurchaseOrder = entryRequest.PurchaseOrder
                            } : null
                        });
                    }
                }

                if (conflictingScheduling != null)
                {
                    return new EquipmentSchedulingValidationResponseDTO
                    {
                        IsAllowed = false,
                        Message = $"El equipo {equipment.Code} - {equipment.Name} no está disponible en el rango de fechas especificado. Ya tiene un agendamiento activo que se superpone con las fechas solicitadas.",
                        IdEquipment = request.IdEquipment,
                        DateIni = request.DateIni,
                        DateEnd = request.DateEnd,
                        RelatedOrders = relatedOrders
                    };
                }

                return new EquipmentSchedulingValidationResponseDTO
                {
                    IsAllowed = true,
                    Message = $"El equipo {equipment.Code} - {equipment.Name} está disponible para el agendamiento en el rango de fechas especificado.",
                    IdEquipment = request.IdEquipment,
                    DateIni = request.DateIni,
                    DateEnd = request.DateEnd,
                    RelatedOrders = relatedOrders
                };
            }
            catch (Exception ex)
            {
                return new EquipmentSchedulingValidationResponseDTO
                {
                    IsAllowed = false,
                    Message = $"Error al validar el agendamiento: {ex.Message}",
                    IdEquipment = request.IdEquipment,
                    DateIni = request.DateIni,
                    DateEnd = request.DateEnd
                };
            }
        }
    }
}
