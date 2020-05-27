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

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationUserController : ControllerBase
    {

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

        public async Task<String> getTokenAsync()
        {
            var client = new RestClient("https://api.crayon.com/")
            {
                Authenticator = new HttpBasicAuthenticator(_appSettings.Crayon_Client_Id, _appSettings.Crayon_Client_Secret)
            };

            var request = new RestRequest("/api/v1/connect/token", Method.POST);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("grant_type", "password");
            request.AddParameter("username", _appSettings.Crayon_Username);
            request.AddParameter("password", _appSettings.Crayon_Password);
            request.AddParameter("scope", "CustomerApi");
            var response = await client.ExecuteAsync(request);
            JObject jsonResponse = JObject.Parse(response.Content);
            string obj = jsonResponse["AccessToken"].ToObject<string>();
            string accessToken = obj.ToString();
            return accessToken;
        }

        //Get : /api/crayon/getOrganizations
        [HttpGet]
        [Route("getOrganizations")]
        public async Task<IActionResult> getOrganizationsAsync()
        {
            var accessToken = await this.getTokenAsync();
            var client = new RestClient("https://api.crayon.com/")
            {
                Authenticator = new JwtAuthenticator(accessToken)
            };

            var request = new RestRequest("/api/v1/organizations", Method.GET);
            var response = await client.ExecuteAsync(request);
            JObject jsonResponse = JObject.Parse(response.Content);
            return Ok(jsonResponse);
        }

        //Get : /api/crayon/getClients
        [HttpGet]
        [Route("getClients")]
        public async Task<IActionResult> getClientsAsync()
        {
            var accessToken = await this.getTokenAsync();
            var client = new RestClient("https://api.crayon.com/")
            {
                Authenticator = new JwtAuthenticator(accessToken)
            };

            var request = new RestRequest("/api/v1/clients", Method.GET);
            var response = await client.ExecuteAsync(request);
            JObject jsonResponse = JObject.Parse(response.Content);
            return Ok(jsonResponse);
        }

        //Get : /api/crayon/getClientById
        [HttpGet]
        [Route("getClientById/{clientId}")]
        public async Task<IActionResult> getClientByIdAsync(string clientId)
        {
            var accessToken = await this.getTokenAsync();
            var client = new RestClient("https://api.crayon.com/")
            {
                Authenticator = new JwtAuthenticator(accessToken)
            };

            var request = new RestRequest("/api/v1/clients/"+ clientId, Method.GET);
            var response = await client.ExecuteAsync(request);
            JObject jsonResponse = JObject.Parse(response.Content);
            return Ok(jsonResponse);
        }


        //Get : /api/crayon/getUsers
        [HttpGet]
        [Route("getUsers")]
        public async Task<IActionResult> getUsersAsync()
        {
            var accessToken = await this.getTokenAsync();
            var client = new RestClient("https://api.crayon.com/")
            {
                Authenticator = new JwtAuthenticator(accessToken)
            };

            var request = new RestRequest("/api/v1/users", Method.GET);
            var response = await client.ExecuteAsync(request);
            JObject jsonResponse = JObject.Parse(response.Content);
            return Ok(jsonResponse);
        }

        //Get : /api/crayon/getUserByUsername/5
        [HttpGet]
        [Route("getUserByUsername/{username}")]
        public async Task<IActionResult> getUserByUsernameAsync(string username)
        {
            var accessToken = await this.getTokenAsync();
            var client = new RestClient("https://api.crayon.com/")
            {
                Authenticator = new JwtAuthenticator(accessToken)
            };

            var request = new RestRequest("/api/v1/users/user/?userName=" + username, Method.GET);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            var response = await client.ExecuteAsync(request);
            JObject jsonResponse = JObject.Parse(response.Content);
            return Ok(jsonResponse);
        }

        //Get : /api/crayon/getUserByUserId/5
        [HttpGet]
        [Route("getUserByUserId/{userId}")]
        public async Task<IActionResult> getUserByUserIdAsync(string userId)
        {
            var accessToken = await this.getTokenAsync();
            var client = new RestClient("https://api.crayon.com/")
            {
                Authenticator = new JwtAuthenticator(accessToken)
            };

            var request = new RestRequest("/api/v1/users/user/?userId=" + userId, Method.GET);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            var response = await client.ExecuteAsync(request);
            JObject jsonResponse = JObject.Parse(response.Content);
            return Ok(jsonResponse);
        }


        //Get : /api/crayon/getOrganization/5
        [HttpGet]
        [Route("getOrganization/{organizationId}")]
        public async Task<IActionResult> getOrganizationByIdAsync(string organizationId)
        {
            var accessToken = await this.getTokenAsync();
            var client = new RestClient("https://api.crayon.com/")
            {
                Authenticator = new JwtAuthenticator(accessToken)
            };

            var request = new RestRequest("/api/v1/organizations/"+organizationId, Method.GET);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            var response = await client.ExecuteAsync(request);
            JObject jsonResponse = JObject.Parse(response.Content);
            return Ok(jsonResponse);
        }

        //Get : /api/crayon/getOrganizationAccessToGrant/5
        [HttpGet]
        [Route("getOrganizationAccessToGrant/{organizationId}/{userId}")]
        public async Task<IActionResult> getOrganizationAccessToGrantAsync(string organizationId,string userId)
        {
            var accessToken = await this.getTokenAsync();
            var client = new RestClient("https://api.crayon.com/")
            {
                Authenticator = new JwtAuthenticator(accessToken)
            };

            var request = new RestRequest("/api/v1/organizationaccess/grant/?userId="+userId+"&organizatioId=" + organizationId, Method.GET);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            var response = await client.ExecuteAsync(request);
            JObject jsonResponse = JObject.Parse(response.Content);
            return Ok(jsonResponse);
        }

        //Get : /api/crayon/billingstatements/5
        [HttpGet]
        [Route("getBillingstatements/{organizationId}")]
        public async Task<IActionResult> getBillingstatementsAsync(string organizationId)
        {
            var accessToken = await this.getTokenAsync();
            var client = new RestClient("https://api.crayon.com/")
            {
                Authenticator = new JwtAuthenticator(accessToken)
            };

            var request = new RestRequest("/api/v1/billingstatements/?organizationId=" + organizationId, Method.GET);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            var response = await client.ExecuteAsync(request);
            JObject jsonResponse = JObject.Parse(response.Content);
            return Ok(jsonResponse);
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




            // List<string> roles = new List<string>();

            //  string userId = User.Claims.First(c => c.Type == "UserID").Value;
            // var users = _userManager.GetUsersInRoleAsync("Admin").Result;          

            // var users = new List<UserAddDto>();
            // _userManager.Users
            //  .Where(x => x.Id != userId)
            // .ToList()
            // .ForEach( x => {
            //    var data= _userManager.GetRolesAsync(x);
            //  roles.Add(data.ToString());
            //var role = roles.Result.First();
            //  users.Add(new UserAddDto(x.Id, x.FirstName, x.lastName, x.Email, x.PhoneNumber, x.UserName, roles));
            //   });
            // return Ok(users);


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
                await _userManager.AddToRoleAsync(applicationUser, "Admin");

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
                }if (!string.IsNullOrEmpty(admin.lastName))
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

                return Ok(new { token,user.Id, user.UserName });
            }
            else
                return BadRequest(new { message = "Username or password is incorrect." });
        }
    }
}