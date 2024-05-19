using FrisörenSörenAPI.Interfaces;
using FrisörenSörenAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FrisörenSörenAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogInController : ControllerBase
    {
        private readonly ILogInService _logInService;

        public LogInController(ILogInService logInService)
        {
            _logInService = logInService;
        }

        // Metod för att logga in
        [HttpPost("CustomerLogin")]
        public async Task<IActionResult> CustomerLogin([FromBody] LogIn model)
        {
            try
            {
                //Sparar en ny customer med AuthenticCustomer-metoden från _autorizeRepot som tar mail och password som inparametrar.
                var customer = await _logInService.AuthenticateCustomer(model.Email, model.Password);
                if (customer == null)
                {
                    return Unauthorized($"Customer with Email {model.Email} and Password {model.Password} not found");
                }

                //Hämtar nuvarande Session och lagrar "UserRole" och "UserEmail" som nycklar, och Customer och customer.Email som value.
                HttpContext.Session.SetString("UserRole", "Customer");
                HttpContext.Session.SetString("UserEmail", customer.Email);


                //Retunerar Ok med kunden ID, namn och email.
                return Ok(new { customer.CustomerId, customer.CustomerName, customer.Email });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        [HttpPost("CompanyLogin")]
        public async Task<IActionResult> CompanyLogin([FromBody] LogIn model)
        {
            try
            {
                var company = await _logInService.AuthenticateCompany(model.Email, model.Password);
                if (company == null)
                {
                    return Unauthorized($"Company with Email {model.Email} and Password {model.Password} not found");
                }

                // Sätter sessionen för att lagra användarens roll och email
                HttpContext.Session.SetString("UserRole", "Company");
                HttpContext.Session.SetString("UserEmail", company.Email);
                return Ok(new { company.CompanyId, company.CompanyName, company.Email });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }



        // Metod för att logga ut en användare
        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            try
            {
                // Rensar sessionen för att logga ut användaren
                HttpContext.Session.Clear();
                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}

