using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Microsoft.AspNetCore.Http.Extensions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WebAPI.Models;
using RestSharp;
using RestSharp.Authenticators;
using Newtonsoft.Json.Linq;
using Microsoft.AspNet.OData;
using System.Net.Http;
using System.Web;
using System.Net.Http.Headers;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationUserController : ControllerBase
    {
        private string azurefunctionlink = "https://prod-03.francecentral.logic.azure.com:443/workflows/b927bc534bb8420f982e2d1044fbae16/triggers/manual/paths/invoke?api-version=2016-10-01&sp=%2Ftriggers%2Fmanual%2Frun&sv=1.0&sig=VYjSnFIYlE9Vg6__26AeGVcahaGCEavmopSlfQduYbw";
        private readonly CrayonContext _context;
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _singInManager;
        private readonly ApplicationSettings _appSettings;



        public ApplicationUserController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IOptions<ApplicationSettings> appSettings, CrayonContext context)
        {
            _userManager = userManager;
            _singInManager = signInManager;
            _appSettings = appSettings.Value;
            _context = context;
        }






        public class Users_in_Role_ViewModel
        {
            public string UserId { get; set; }
            public string Username { get; set; }
            public string Email { get; set; }
            public string Role { get; set; }
            public string PhoneNumber { get; set; }
            public string FirstName { get; set; }
            public string lastName { get; set; }

        }


        [Authorize(Roles = "GlobalAdmin,Admin")]
        [HttpGet("GetAdmins")]
        //Get : /api/ApplicationUser/GetAdmins
        public IEnumerable<Users_in_Role_ViewModel> GetUserwithRole()
        {


            var usersWithRoles = (from user in _context.Users
                                  select new
                                  {
                                      UserId = user.Id,
                                      Username = user.UserName,
                                      FirstName = user.FirstName,
                                      lastName = user.lastName,
                                      PhoneNumber = user.PhoneNumber,

                                      Email = user.Email,
                                      RoleNames = (from userRole in user.Roles
                                                   join role in _context.Roles on userRole.RoleId
                                                   equals role.Id
                                                   select role.Name).ToList()
                                  }).ToList().Select(p => new Users_in_Role_ViewModel()

                                  {
                                      UserId = p.UserId,
                                      Username = p.Username,
                                      FirstName = p.FirstName,
                                      lastName = p.lastName,
                                      PhoneNumber = p.PhoneNumber,
                                      Email = p.Email,
                                      Role = string.Join(",", p.RoleNames)
                                  });



            return usersWithRoles;


        }











        [HttpPost]
        [Route("Register/Admin")]
        //POST : /api/ApplicationUser/Register/Admin
        public async Task<Object> PostApplicationUser(RegisterUserModel model)
        {
            // List<string> error = new List<string>();
            // if (!ModelState.IsValid)
            // {
            //    ModelState.Root.Children.ToList().ForEach(x =>
            //  {
            //  x.Errors.ToList().ForEach(y =>
            //  error.Add(y.ErrorMessage)
            // );
            // });
            // return BadRequest(error);


            // }
            var applicationUser = new ApplicationUser() {
                UserName = model.UserName,
                Email = model.Email,
                FirstName = model.FirstName,
                lastName = model.lastName,
                PhoneNumber = model.PhoneNumber,

                IsActive = true
            };

            try
            {

                var result = await _userManager.CreateAsync(applicationUser, model.Password);
                await _userManager.AddToRoleAsync(applicationUser, "GlobalAdmin");
              await  SendUserEmailVerificationAsync(applicationUser);
                return Ok(result);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        //Get : /api/ApplicationUser/getAdminById/id
        [Authorize(Roles = "GlobalAdmin,Admin")]
        [HttpGet("getAdminById/{id}")]
        public async Task<IActionResult> getAdminById(string id)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(c => c.Id == id);
            var roles = await _userManager.GetRolesAsync(user);

            if (user == null)
            {
                return BadRequest();
            }
            return Ok(new { User = user, Roles = roles });
        }

        //Put : /api/ApplicationUser/ChangeToGlobal/{username}
        //[Authorize(Roles = "GlobalAdmin,Admin")]
        [HttpPut("ChangeToGlobal/{username}")]
        public async Task<IActionResult> ChangeToGlobal(string username)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(c => c.UserName == username);
            if (user == null)
            {
                return BadRequest();
            }
            try
            {

                var roles = await _userManager.GetRolesAsync(user);

                var removeresult = await _userManager.RemoveFromRolesAsync(user, roles.ToList());
                var addResult = await _userManager.AddToRoleAsync(user, "GlobalAdmin");
                //user.Role = "GlobalAdmin";
                var updateResult = await _userManager.UpdateAsync(user);

                var adminrole = await _userManager.GetRolesAsync(user);
                return Ok(adminrole);
            }
            catch
            {
                return NoContent();
            }
        }



        //Put : /api/ApplicationUser/ChangeToAdmin/{username}
        //[Authorize(Roles = "GlobalAdmin,Admin")]
        [HttpPut("ChangeToAdmin/{username}")]
        public async Task<IActionResult> ChangeToAdmin(string username)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(c => c.UserName == username);
            if (user == null)
            {
                return BadRequest();
            }
            try
            {
                var roles = await _userManager.GetRolesAsync(user);

                await _userManager.RemoveFromRolesAsync(user, roles);
                await _userManager.AddToRoleAsync(user, "Admin");
                //user.Role = "GlobalAdmin";
                await _userManager.UpdateAsync(user);

                var adminrole = await _userManager.GetRolesAsync(user);
                return Ok(adminrole);
            }
            catch
            {
                return NoContent();
            }
        }



        //Put : /api/ApplicationUser/Edit
        [Authorize(Roles = "GlobalAdmin")]
        [HttpPut("Edit/{id}")]
        public async Task<IActionResult> Edit(string id, ApplicationUser admin)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(c => c.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            try
            {
                if (!string.IsNullOrEmpty(admin.UserName))
                {
                    user.UserName = admin.UserName;
                }
                if (!string.IsNullOrEmpty(admin.Email))
                {
                    user.Email = admin.Email;
                }
                if (!string.IsNullOrEmpty(admin.FirstName))
                {
                    user.FirstName = admin.FirstName;
                } if (!string.IsNullOrEmpty(admin.lastName))
                {
                    user.lastName = admin.lastName;
                }
                if (!string.IsNullOrEmpty(admin.PhoneNumber))
                {
                    user.PhoneNumber = admin.PhoneNumber;
                }
                if (!string.IsNullOrEmpty(admin.IsActive.ToString()))
                {
                    user.IsActive = admin.IsActive;
                }

                await _userManager.UpdateAsync(user);
                return Ok(user);

            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        //Delete : /api/ApplicationUser/Delete/{username}
        [HttpDelete("delete/{username}")]
        [Authorize(Roles = "GlobalAdmin,Admin")]
        public async Task<IActionResult> Delete(string username)
        {

            var user = await _userManager.Users.SingleOrDefaultAsync(c => c.UserName == username);
            if (user == null)
            {
                return NotFound();
            }
            try
            {
                await _userManager.DeleteAsync(user);
                return NoContent();
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        [HttpPost]
        [Route("Login")]
        //POST : /api/ApplicationUser/Login
        public async Task<IActionResult> Login(LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                //Get role assigned to the user
                var role = await _userManager.GetRolesAsync(user);
                IdentityOptions _options = new IdentityOptions();

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim("UserID",user.Id.ToString()),
                        new Claim("UserName",user.UserName),
                        new Claim("FirstName",user.FirstName),
                        new Claim("LastName",user.lastName),
                        new Claim(_options.ClaimsIdentity.RoleClaimType,role.FirstOrDefault())
                    }),
                    Expires = DateTime.UtcNow.AddDays(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.JWT_Secret)), SecurityAlgorithms.HmacSha256Signature)
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                var token = tokenHandler.WriteToken(securityToken);

                return Ok(new { token, user.Id, user.UserName });
            }
            else
                return BadRequest(new { message = "Username or password is incorrect." });
        }






        //class

        public class UpdateUserPasswordApiModel
        {
            #region Public Properties
            /// <summary>
            /// The users current password
            /// </summary>
            public string CurrentPassword { get; set; }
            /// <summary>
            /// The users new password
            /// </summary>
            public string NewPassword { get; set; }
            #endregion
        }

        //update user pass using UserEmail
        [HttpPut]
        [Route("updateauserpassword/{UserEmail}")]
        public async Task<IActionResult> UpdateaUserPasswordAsync([FromRoute] string UserEmail,
            UpdateUserPasswordApiModel model)
        {







            try
            {
                #region Get User



                // var user = await _userManager.GetUserAsync();
                var user = await _userManager.FindByEmailAsync(UserEmail);
                // If we have no user...
                if (user == null)
                {





                    return BadRequest(new { message = " user dont exist" });
                }



                #region Update Password



                var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);



                //return BuildJsonResponse(200, "SUCCESS", result, null);

                return Ok(result);
                ;
                #endregion



            }



            #endregion



            catch (Exception ex)
            {
                throw
                     ;



            }




        }






        /// <summary>
        /// Sends the given user a new verify email link
        /// </summary>
        /// <param name="user">The user to send the link to</param>
        /// <returns></returns>
        //[HttpPost]
        //[Route("SendMail")]
        private async Task<IActionResult> SendUserEmailVerificationAsync(ApplicationUser user)
        {



            //ErrorObject _emailTosenderror = new ErrorObject();



            List<string> error = new List<string>();




            try
            {
                var httpClient = new HttpClient();
                emailTosend _emailTosend = new emailTosend();
                //passing the template to use
                //string template = "";
                //var t = _context.Templates.Find(1);




                //if (t != null)
                //{
                //    template = t.Code;
                //}




                // Get the user details
                var userIdentity = await _userManager.FindByNameAsync(user.UserName);



                // Generate an email verification code
                user.TwoFactorEnabled = true;
                var emailVerificationCode = await _userManager.GenerateEmailConfirmationTokenAsync(user);



                // TODO: Replace with APIRoutes that will contain the static routes to use
                //fix sending problem code
                var confirmationUrl = $"https://{Request.Host.Value}/VerifyEmail?userId={HttpUtility.UrlEncode(userIdentity.Id)}&emailToken={HttpUtility.UrlEncode(emailVerificationCode)}";



                _emailTosend.confirmationUrl = confirmationUrl;
                _emailTosend.Email = userIdentity.Email;
                _emailTosend.FullName = user.FirstName;
                Console.WriteLine(_emailTosend);



                var json = Newtonsoft.Json.JsonConvert.SerializeObject(_emailTosend);



                StringContent content = new StringContent(json);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");



                // need to log this
                var result = await httpClient.PostAsync(azurefunctionlink, content);



                return Ok(200);



                // Email the user the verification code
                //await TaskIdoEmailSender.SendUserVerificationEmailAsync(user.FullName, userIdentity.Email, confirmationUrl, template);



                // here to use the logic app to send the email 



            }
            catch (Exception ex)
            {
                error.Add(ex.Message);

                return Ok(500);
            }
        }





        public class emailTosend
        {
            public string FullName { get; set; }
            public string Email { get; set; }
            public string confirmationUrl { get; set; }


        }






        [ApiExplorerSettings(IgnoreApi = true)]
        [AllowAnonymous]
        [Route("/VerifyEmail")]
        [HttpGet]
        public async Task<IActionResult> VerifyEmailAsync(string userId, string emailToken)
        {

            List<string> error = new List<string>();

            try
            {
                var user = await _userManager.FindByIdAsync(userId);



                // If the user is null
                if (user == null)
                // TODO: Nice UI
                {
                    return Ok(404);


                }

                else
                if (user.EmailConfirmed == true)
                {
                    return Ok(200);



                }




                else
                {
                    // If we have the user...



                    // Verify the email token
                    var result = await _userManager.ConfirmEmailAsync(user, emailToken);



                    // If succeeded...
                    if (result.Succeeded)
                    // TODO: Nice UI



                    {


                        return  Ok(200);



                        // return Content("Email Verified :)");

                    }



                    else
                    {
                        return Ok(500);
                    }
                }
            }



            catch (Exception ex)
            {
                error.Add(ex.Message);


                return Ok(500);
            }





            // TODO: Nice UI
            // return Content("Invalid Email Verification Token :(");
        }







    }









}