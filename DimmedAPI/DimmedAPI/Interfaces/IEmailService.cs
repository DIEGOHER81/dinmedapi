using DimmedAPI.DTOs;

namespace DimmedAPI.Interfaces
{
    public interface IEmailService
    {
        Task<EmailSendResponseDTO> SendEmailAsync(string companyCode, EmailSendRequestDTO request);
    }
} 