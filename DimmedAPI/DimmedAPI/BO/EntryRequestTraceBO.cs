using DimmedAPI.DTOs;
using DimmedAPI.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace DimmedAPI.BO
{
    public class EntryRequestTraceBO
    {
        private readonly IDynamicConnectionService _dynamicConnectionService;

        public EntryRequestTraceBO(IDynamicConnectionService dynamicConnectionService)
        {
            _dynamicConnectionService = dynamicConnectionService;
        }

        /// <summary>
        /// Obtiene el trace de un EntryRequest usando el procedimiento almacenado GET_TRACE_RQ_2
        /// </summary>
        /// <param name="companyCode">Código de la compañía</param>
        /// <param name="rqId">ID del EntryRequest (opcional)</param>
        /// <param name="branchId">ID de la sucursal (opcional)</param>
        /// <param name="dateIni">Fecha inicial (opcional)</param>
        /// <param name="dateEnd">Fecha final (opcional)</param>
        /// <returns>Lista de traces del EntryRequest</returns>
        public async Task<List<EntryRequestTraceDTO>> GetEntryRequestTraceAsync(
            string companyCode,
            int? rqId = null,
            int? branchId = null,
            DateTime? dateIni = null,
            DateTime? dateEnd = null)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    throw new ArgumentException("El código de compañía es requerido");
                }

                if (rqId.HasValue && rqId.Value <= 0)
                {
                    throw new ArgumentException("El RQID debe ser mayor a 0");
                }

                using var companyContext = await _dynamicConnectionService.GetCompanyDbContextAsync(companyCode);
                var traces = new List<EntryRequestTraceDTO>();

                using (var command = companyContext.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "EXEC [dbo].[GET_TRACE_RQ_2] @RQID, @BRANCHID, @DATEINI, @DATEEND";
                    command.CommandType = CommandType.Text;

                    // Agregar parámetros
                    var rqIdParam = command.CreateParameter();
                    rqIdParam.ParameterName = "@RQID";
                    rqIdParam.Value = rqId ?? (object)DBNull.Value;
                    command.Parameters.Add(rqIdParam);

                    var branchIdParam = command.CreateParameter();
                    branchIdParam.ParameterName = "@BRANCHID";
                    branchIdParam.Value = branchId ?? (object)DBNull.Value;
                    command.Parameters.Add(branchIdParam);

                    var dateIniParam = command.CreateParameter();
                    dateIniParam.ParameterName = "@DATEINI";
                    dateIniParam.Value = dateIni ?? (object)DBNull.Value;
                    command.Parameters.Add(dateIniParam);

                    var dateEndParam = command.CreateParameter();
                    dateEndParam.ParameterName = "@DATEEND";
                    dateEndParam.Value = dateEnd ?? (object)DBNull.Value;
                    command.Parameters.Add(dateEndParam);

                    if (command.Connection.State != ConnectionState.Open)
                        await command.Connection.OpenAsync();

                    using (var result = await command.ExecuteReaderAsync())
                    {
                        while (await result.ReadAsync())
                        {
                            traces.Add(new EntryRequestTraceDTO
                            {
                                Id = GetSafeInt32(result, "Id"),
                                LoadDate = GetSafeDateTime(result, "LoadDate"),
                                TraceState = GetSafeString(result, "traceState"),
                                CustomerName = GetSafeString(result, "CustomerName"),
                                Branch = GetSafeString(result, "Branch"),
                                SurgeryInit = GetSafeDateTime(result, "SurgeryInit"),
                                Status = GetSafeString(result, "Status"),
                                UserName = GetSafeString(result, "UserName"),
                                EntryRequestTraceState = GetSafeString(result, "EntryRequestTraceState"),
                                EName = GetSafeString(result, "eName"),
                                ECode = GetSafeString(result, "eCode"),
                                Equipos = GetSafeString(result, "equipos"),
                                Componentes = GetSafeString(result, "Componentes")
                            });
                        }
                    }
                }

                return traces;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener el trace del EntryRequest: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene el trace de un EntryRequest usando el DTO de filtro
        /// </summary>
        /// <param name="companyCode">Código de la compañía</param>
        /// <param name="filter">Filtro con los parámetros</param>
        /// <returns>Lista de traces del EntryRequest</returns>
        public async Task<List<EntryRequestTraceDTO>> GetEntryRequestTraceWithFilterAsync(
            string companyCode,
            EntryRequestTraceFilterDTO filter)
        {
            return await GetEntryRequestTraceAsync(
                companyCode,
                filter.RQID,
                filter.BranchId,
                filter.DateIni,
                filter.DateEnd);
        }

        #region Helper Methods

        private static int? GetSafeInt32(DbDataReader reader, string columnName)
        {
            try
            {
                var ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? null : reader.GetInt32(ordinal);
            }
            catch
            {
                return null;
            }
        }

        private static DateTime? GetSafeDateTime(DbDataReader reader, string columnName)
        {
            try
            {
                var ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? null : reader.GetDateTime(ordinal);
            }
            catch
            {
                return null;
            }
        }

        private static string? GetSafeString(DbDataReader reader, string columnName)
        {
            try
            {
                var ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
} 