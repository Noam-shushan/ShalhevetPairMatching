using Microsoft.AspNetCore.Mvc;
using PairMatching.DomainModel.Services;
using PairMatching.Models;


namespace ShalhevetAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmailsController : Controller
    {
        private readonly IEmailService _emailService;

        public EmailsController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpGet(Name = "GetEmails")]
        public async Task<IActionResult> Get()
        {
            var emails = await _emailService.GetEmails();
            return Ok(emails);
        }

        [HttpPost(Name = "SendEmail")]
        public async Task<IActionResult> Post(EmailModel emailModel)
        {
            var newEmail = await _emailService.SendEmail(emailModel);
            return Ok(newEmail);
        }
    }
}
