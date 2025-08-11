using DimmedAPI.Entidades;
using System.Threading.Tasks;

namespace DimmedAPI.Interfaces
{
    public interface IEmployeeBO
    {
        Task<Employee?> GetByIdAsync(int id);
    }
} 