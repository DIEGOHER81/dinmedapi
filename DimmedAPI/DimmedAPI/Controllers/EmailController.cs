using Microsoft.AspNetCore.Mvc;
using DimmedAPI.DTOs;
using DimmedAPI.Interfaces;

namespace DimmedAPI.Controllers
{
    [Route("api/email")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        /// <summary>
        /// Envía un correo electrónico utilizando la configuración de la compañía especificada
        /// </summary>
        /// <param name="companyCode">Código de la compañía</param>
        /// <param name="request">Datos del correo a enviar</param>
        /// <returns>Resultado del envío del correo</returns>
        [HttpPost("send")]
        public async Task<ActionResult<EmailSendResponseDTO>> SendEmail([FromQuery] string companyCode, [FromBody] EmailSendRequestDTO request)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode))
                {
                    return BadRequest("El código de compañía es requerido");
                }

                if (request == null)
                {
                    return BadRequest("La solicitud no puede estar vacía");
                }

                if (string.IsNullOrEmpty(request.ToEmail))
                {
                    return BadRequest("El correo destinatario es requerido");
                }

                if (string.IsNullOrEmpty(request.Subject))
                {
                    return BadRequest("El asunto del correo es requerido");
                }

                if (string.IsNullOrEmpty(request.Body))
                {
                    return BadRequest("El contenido del correo es requerido");
                }

                var result = await _emailService.SendEmailAsync(companyCode, request);

                if (result.Success)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new EmailSendResponseDTO
                {
                    Success = false,
                    Message = "Error interno del servidor",
                    ErrorDetails = ex.Message
                });
            }
        }
    }
} 