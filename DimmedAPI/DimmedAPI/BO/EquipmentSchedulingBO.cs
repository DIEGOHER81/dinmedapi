using DimmedAPI.DTOs;
using DimmedAPI.Entidades;
using Microsoft.EntityFrameworkCore;
using System;
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

                // Verificar si existe algún EntryRequestDetail que se superponga con el rango de fechas
                var conflictingScheduling = await _context.EntryRequestDetails
                    .FromSqlRaw(@"
                        SELECT 
                            Id,
                            IdEntryReq,
                            IdEquipment,
                            CreateAt,
                            DateIni,
                            DateEnd,
                            ISNULL(status, '') as status,
                            DateLoadState,
                            ISNULL(TraceState, '') as TraceState,
                            IsComponent
                        FROM EntryRequestDetails
                        WHERE IdEquipment = {0}
                          AND status = 'NUEVO'
                          AND (
                               {1} <= DateEnd
                           AND {2} >= DateIni
                          )", request.IdEquipment, request.DateIni, request.DateEnd)
                    .FirstOrDefaultAsync();

                if (conflictingScheduling != null)
                {
                    return new EquipmentSchedulingValidationResponseDTO
                    {
                        IsAllowed = false,
                        Message = $"El equipo {equipment.Code} - {equipment.Name} no está disponible en el rango de fechas especificado. Ya tiene un agendamiento activo que se superpone con las fechas solicitadas.",
                        IdEquipment = request.IdEquipment,
                        DateIni = request.DateIni,
                        DateEnd = request.DateEnd
                    };
                }

                return new EquipmentSchedulingValidationResponseDTO
                {
                    IsAllowed = true,
                    Message = $"El equipo {equipment.Code} - {equipment.Name} está disponible para el agendamiento en el rango de fechas especificado.",
                    IdEquipment = request.IdEquipment,
                    DateIni = request.DateIni,
                    DateEnd = request.DateEnd
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
