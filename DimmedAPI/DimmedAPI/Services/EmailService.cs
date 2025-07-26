using DimmedAPI.DTOs;
using DimmedAPI.Entidades;
using DimmedAPI.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
using System.Text;

namespace DimmedAPI.Services
{
    public class EmailService : IEmailService
    {
        private readonly ApplicationDBContext _context;

        public EmailService(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<EmailSendResponseDTO> SendEmailAsync(string companyCode, EmailSendRequestDTO request)
        {
            try
            {
                // Obtener la configuración de la compañía
                var company = await _context.Companies
                    .FirstOrDefaultAsync(c => c.BCCodigoEmpresa == companyCode);

                if (company == null)
                {
                    return new EmailSendResponseDTO
                    {
                        Success = false,
                        Message = $"Compañía con código {companyCode} no encontrada",
                        ErrorDetails = "Company not found"
                    };
                }

                // Validar que la compañía tenga la configuración de correo
                if (string.IsNullOrEmpty(company.correonotificacion) ||
                    string.IsNullOrEmpty(company.pwdnotificacion) ||
                    string.IsNullOrEmpty(company.smtpserver) ||
                    string.IsNullOrEmpty(company.puertosmtp))
                {
                    return new EmailSendResponseDTO
                    {
                        Success = false,
                        Message = "La compañía no tiene configurada la información de correo electrónico",
                        ErrorDetails = "Email configuration missing"
                    };
                }

                // Configurar el cliente SMTP
                using var smtpClient = new SmtpClient(company.smtpserver)
                {
                    Port = int.Parse(company.puertosmtp),
                    Credentials = new NetworkCredential(company.correonotificacion, company.pwdnotificacion),
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false
                };

                // Crear el mensaje
                using var mailMessage = new MailMessage
                {
                    From = new MailAddress(company.correonotificacion, company.nombrenotificacion ?? company.BusinessName),
                    Subject = request.Subject,
                    Body = request.Body,
                    IsBodyHtml = request.IsHtml,
                    BodyEncoding = Encoding.UTF8,
                    SubjectEncoding = Encoding.UTF8
                };

                mailMessage.To.Add(request.ToEmail);

                // Enviar el correo
                await smtpClient.SendMailAsync(mailMessage);

                return new EmailSendResponseDTO
                {
                    Success = true,
                    Message = "Correo enviado exitosamente"
                };
            }
            catch (Exception ex)
            {
                return new EmailSendResponseDTO
                {
                    Success = false,
                    Message = "Error al enviar el correo electrónico",
                    ErrorDetails = ex.Message
                };
            }
        }
    }
} 