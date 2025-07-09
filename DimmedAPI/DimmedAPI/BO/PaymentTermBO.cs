using DimmedAPI.DTOs;
using DimmedAPI.Entidades;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DimmedAPI.BO
{
    public class PaymentTermBO
    {
        private readonly ApplicationDBContext _context;
        private readonly IntBCConex _bcConn;

        public PaymentTermBO(ApplicationDBContext context, IntBCConex bcConn)
        {
            _context = context;
            _bcConn = bcConn;
        }

        // Sincroniza los términos de pago desde BC a la base local
        public async Task<List<PaymentTerm>> SincronizarDesdeBCAsync()
        {
            var termsBC = await _bcConn.GetPaymentTermsBCAsync();
            var termsLocal = await _context.Set<PaymentTerm>().ToListAsync();

            foreach (var termBC in termsBC)
            {
                var local = termsLocal.FirstOrDefault(t => t.Code == termBC.Code);
                if (local == null)
                {
                    // Nuevo término
                    _context.Set<PaymentTerm>().Add(new PaymentTerm
                    {
                        Code = termBC.Code,
                        Description = termBC.Description,
                        DueDateCalculation = termBC.DueDateCalculation
                    });
                }
                else
                {
                    // Actualizar existente
                    local.Description = termBC.Description;
                    local.DueDateCalculation = termBC.DueDateCalculation;
                }
            }
            await _context.SaveChangesAsync();
            return await _context.Set<PaymentTerm>().ToListAsync();
        }

        // Consulta local
        public async Task<List<PaymentTerm>> GetAllAsync()
        {
            return await _context.Set<PaymentTerm>().ToListAsync();
        }
    }
} 