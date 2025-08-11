using DimmedAPI.Entidades;
using DimmedAPI.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DimmedAPI.BO
{
    public class EmployeeBO : IEmployeeBO
    {
        private readonly ApplicationDBContext _context;

        public EmployeeBO(ApplicationDBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene un empleado por su ID
        /// </summary>
        /// <param name="id">ID del empleado</param>
        /// <returns>Empleado encontrado o null</returns>
        public async Task<Employee?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Employee
                    .FirstOrDefaultAsync(e => e.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener el empleado con ID {id}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene todos los empleados
        /// </summary>
        /// <returns>Lista de empleados</returns>
        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            try
            {
                return await _context.Employee.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener todos los empleados: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene empleados por cargo
        /// </summary>
        /// <param name="charge">Cargo del empleado</param>
        /// <returns>Lista de empleados del cargo especificado</returns>
        public async Task<IEnumerable<Employee>> GetByChargeAsync(string charge)
        {
            try
            {
                return await _context.Employee
                    .Where(e => e.Charge == charge)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener empleados por cargo {charge}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Crea un nuevo empleado
        /// </summary>
        /// <param name="employee">Empleado a crear</param>
        /// <returns>Empleado creado</returns>
        public async Task<Employee> CreateAsync(Employee employee)
        {
            try
            {
                _context.Employee.Add(employee);
                await _context.SaveChangesAsync();
                return employee;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al crear el empleado: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Actualiza un empleado existente
        /// </summary>
        /// <param name="employee">Empleado a actualizar</param>
        /// <returns>Empleado actualizado</returns>
        public async Task<Employee> UpdateAsync(Employee employee)
        {
            try
            {
                _context.Employee.Update(employee);
                await _context.SaveChangesAsync();
                return employee;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al actualizar el empleado: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Elimina un empleado por su ID
        /// </summary>
        /// <param name="id">ID del empleado a eliminar</param>
        /// <returns>True si se eliminó correctamente</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var employee = await _context.Employee.FindAsync(id);
                if (employee == null)
                    return false;

                _context.Employee.Remove(employee);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al eliminar el empleado con ID {id}: {ex.Message}", ex);
            }
        }
    }
} 