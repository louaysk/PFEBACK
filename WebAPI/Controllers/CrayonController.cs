﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;
using WebAPI.Models;

namespace WebAPI.Controllers
{


    [Route("api/crayon")]
   // [ApiController]
    public class CrayonController : ControllerBase
    {
        private readonly ApplicationSettings _appSettings;

        public CrayonController(IOptions<ApplicationSettings> appSettings)

        {
            _appSettings = appSettings.Value;
        }

        [HttpGet("getTokenAsync")]
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
        [HttpGet("getOrganizations")]
        
        public async Task<IActionResult> getOrganizationsAsync(int page = 1, int pageSize = 10)
        {
            var accessToken = await this.getTokenAsync();
            var client = new RestClient("https://api.crayon.com/")
            {
                Authenticator = new JwtAuthenticator(accessToken)
            };

            var request = new RestRequest("/api/v1/organizations?page=" + page + "&pageSize=" + pageSize, Method.GET);
            var response = await client.ExecuteAsync(request);
            JObject jsonResponse = JObject.Parse(response.Content);
            return Ok(jsonResponse);
        }

        //Get : /api/crayon/getClients
        [HttpGet]
        [Route("getClients")]
        public async Task<IActionResult> getClientsAsync(int page = 1, int pageSize = 10, string search="")
        {
            var accessToken = await this.getTokenAsync();
            var client = new RestClient("https://api.crayon.com/")
            {
                Authenticator = new JwtAuthenticator(accessToken)
            };

            var request = new RestRequest("/api/v1/clients?search="+ search + "&page=" + page+"&pageSize="+ pageSize, Method.GET);
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

            var request = new RestRequest("/api/v1/clients/" + clientId, Method.GET);
            var response = await client.ExecuteAsync(request);
            JObject jsonResponse = JObject.Parse(response.Content);
            return Ok(jsonResponse);
        }


        //Get : /api/crayon/getUsers
        [HttpGet]
        [Route("getUsers")]
        public async Task<IActionResult> getUsersAsync(int page = 1, int pageSize = 10 )
        {
            var accessToken = await this.getTokenAsync();
            var client = new RestClient("https://api.crayon.com/")
            {
                Authenticator = new JwtAuthenticator(accessToken)
            };

            var request = new RestRequest("/api/v1/users?page=" + page + "&pageSize=" + pageSize, Method.GET);
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

            var request = new RestRequest("/api/v1/organizations/" + organizationId, Method.GET);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            var response = await client.ExecuteAsync(request);
            JObject jsonResponse = JObject.Parse(response.Content);
            return Ok(jsonResponse);
        }

        //Get : /api/crayon/getOrganizationAccessToGrant/5
        [HttpGet]
        [Route("getOrganizationAccessToGrant/{organizationId}/{userId}")]
        public async Task<IActionResult> getOrganizationAccessToGrantAsync(string organizationId, string userId)
        {
            var accessToken = await this.getTokenAsync();
            var client = new RestClient("https://api.crayon.com/")
            {
                Authenticator = new JwtAuthenticator(accessToken)
            };

            var request = new RestRequest("/api/v1/organizationaccess/grant/?userId=" + userId + "&organizatioId=" + organizationId, Method.GET);
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

            var request = new RestRequest("/api/v1/billingstatements/?organizationId=" + organizationId , Method.GET);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            var response = await client.ExecuteAsync(request);
            JObject jsonResponse = JObject.Parse(response.Content);
            return Ok(jsonResponse);
        }







      

        //Get : /api/crayon/Getinvoiceprofiles/5
        [HttpGet]
        [Route("Getinvoiceprofiles/{organizationId}")]
        public async Task<IActionResult> GetinvoiceprofilesAsync(string organizationId)
        {
            var accessToken = await this.getTokenAsync();
            var client = new RestClient("https://api.crayon.com/")
            {
                Authenticator = new JwtAuthenticator(accessToken)
            };

            var request = new RestRequest("/api/v1/invoiceprofiles/?organizationId=" + organizationId, Method.GET);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            var response = await client.ExecuteAsync(request);
            JObject jsonResponse = JObject.Parse(response.Content);
            return Ok(jsonResponse);
        }


        //Get : /api/crayon/Getcustomertenants/5
        [HttpGet]
        [Route("Getcustomertenants/{organizationId}")]
        public async Task<IActionResult> GetcustomertenantsAsync(string organizationId)
        {
            var accessToken = await this.getTokenAsync();
            var client = new RestClient("https://api.crayon.com/")
            {
                Authenticator = new JwtAuthenticator(accessToken)
            };

            var request = new RestRequest("/api/v1/customertenants/?organizationId=" + organizationId, Method.GET);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            var response = await client.ExecuteAsync(request);
            JObject jsonResponse = JObject.Parse(response.Content);
            return Ok(jsonResponse);
        }


        //Get : /api/crayon/Getagreementproducts/5
        [HttpGet]
        [Route("Getagreementproducts/{organizationId}")]
        public async Task<IActionResult> GetagreementproductsAsync(string organizationId)
        {
            var accessToken = await this.getTokenAsync();
            var client = new RestClient("https://api.crayon.com/")
            {
                Authenticator = new JwtAuthenticator(accessToken)
            };

            var request = new RestRequest("/api/v1/agreementproducts/?organizationId=" + organizationId, Method.GET);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            var response = await client.ExecuteAsync(request);
            JObject jsonResponse = JObject.Parse(response.Content);
            return Ok(jsonResponse);
        }





        //Get : /api/crayon/GetSubscriptions/5
        [HttpGet]
        [Route("GetSubscriptions/{organizationId}/{publisherCustomerId}")]
        public async Task<IActionResult> GetSubscriptionsAsync(string organizationId ,string publisherCustomerId)
        {
            var accessToken = await this.getTokenAsync();
            var client = new RestClient("https://api.crayon.com/")
            {
                Authenticator = new JwtAuthenticator(accessToken)
            };

            var request = new RestRequest("/api/v1/subscriptions/?organizationId=" + organizationId+ "&publisherCustomerId="+ publisherCustomerId, Method.GET);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            var response = await client.ExecuteAsync(request);
            JObject jsonResponse = JObject.Parse(response.Content);
            return Ok(jsonResponse);
        }




        //Get : /api/crayon/Getmanagementlinks/5
        [HttpGet]
        [Route("Getmanagementlinks/{resellerCustomerIds}/{subscriptionIds}")]
        public async Task<IActionResult> GetmanagementlinksAsync(string resellerCustomerIds, string subscriptionIds)
        {
            var accessToken = await this.getTokenAsync();
            var client = new RestClient("https://api.crayon.com/")
            {
                Authenticator = new JwtAuthenticator(accessToken)
            };

            var request = new RestRequest("/api/v1/managementlinks/?resellerCustomerIds=" + resellerCustomerIds + "&subscriptionIds=" + subscriptionIds, Method.GET);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            var response = await client.ExecuteAsync(request);
            JObject jsonResponse = JObject.Parse(response.Content);
            return Ok(jsonResponse);
        }




        //Get : /api/crayon/Getusagecostsbyorganization/5
        [HttpGet]
        [Route("Getusagecostsbyorganization")]
        public async Task<IActionResult> GetusagecostsbyorganizationAsync(string organizationId)
        {
            var accessToken = await this.getTokenAsync();
            var client = new RestClient("https://api.crayon.com/")
            {
                Authenticator = new JwtAuthenticator(accessToken)
            };

            var request = new RestRequest("/api/v1/UsageCost/organization/" + organizationId, Method.GET);
            //request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            var response = await client.ExecuteAsync(request);
            JObject jsonResponse = JObject.Parse(response.Content);
            return Ok(jsonResponse);
        }


        //Get : /api/crayon/Getusagecostsbysubscription/5
        [HttpGet]
        [Route("Getusagecostsbysubscription")]
        public async Task<IActionResult> Getusagecostsbysubscription(int resellerCustomerId, string subscriptionId, string currencyCode)
        {
            var accessToken = await this.getTokenAsync();
            var client = new RestClient("https://api.crayon.com/")
            {
                Authenticator = new JwtAuthenticator(accessToken)
            };

            var request = new RestRequest("/api/v1/usagecost/resellerCustomer/"+ resellerCustomerId + "/subscription/"+ subscriptionId + "/currency/"+currencyCode, Method.GET);
            var response = await client.ExecuteAsync(request);
            JObject jsonResponse = JObject.Parse(response.Content);
            return Ok(jsonResponse);

        }



        //Get : /api/crayon/Getusagecostsbysubcategory/5
        [HttpGet]
        [Route("Getusagecostsbysubcategory")]
        public async Task<IActionResult> GetusagecostsbysubcategoryAsync(int resellerCustomerId, string subscriptionId, string category,string subcategory, string currencyCode)
        {
            var accessToken = await this.getTokenAsync();
            var client = new RestClient("https://api.crayon.com/")
            {
                Authenticator = new JwtAuthenticator(accessToken)
            };

            var request = new RestRequest("/api/v1/usagecost/resellerCustomer/" + resellerCustomerId + "/subscription/" + subscriptionId + "/category/" + category + "/subcategory/"+ subcategory + "/currency/" + currencyCode , Method.GET);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            var response = await client.ExecuteAsync(request);
            JObject jsonResponse = JObject.Parse(response.Content);
            return Ok(jsonResponse);
        }



        //Get : /api/crayon/Getusagecostsbycategory/5
        [HttpGet]
        [Route("Getusagecostsbycategory")]
        public async Task<IActionResult> GetusagecostsbycategoryAsync(int resellerCustomerId, string subscriptionId, string category, string currencyCode)
        {
            var accessToken = await this.getTokenAsync();
            var client = new RestClient("https://api.crayon.com/")
            {
                Authenticator = new JwtAuthenticator(accessToken)
            };

            var request = new RestRequest("/api/v1/usagecost/resellerCustomer/" + resellerCustomerId + "/subscription/" + subscriptionId + "/category/" + category  + "/currency/" + currencyCode, Method.GET);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            var response = await client.ExecuteAsync(request);
            JObject jsonResponse = JObject.Parse(response.Content);
            return Ok(jsonResponse);
        }



        //louay sakhri

    }
}