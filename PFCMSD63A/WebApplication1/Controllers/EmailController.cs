using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using WebApplication1.DataAccess.Interfaces;

namespace WebApplication1.Controllers
{
    public class EmailController : Controller
    {
        private readonly IBlogsRepository _blogsRepo;
        private readonly IConfiguration _config;
        private readonly IPubSubRepository _pubsubRepo;
        public EmailController( IConfiguration config, IPubSubRepository pubsubRepo)
        {
            _config = config;
            _pubsubRepo = pubsubRepo;
        }


        public IActionResult Pull()
        {

         string msgSerialized =  _pubsubRepo.PullMessage(DataAccess.Repositories.Category.luxury);
            if (msgSerialized == string.Empty) return Content("No message read"); 

            dynamic myDeserializedData = JsonConvert.DeserializeObject(msgSerialized);
            string email = myDeserializedData.Email;
            string blogTitle = myDeserializedData.Blog.Title;
            string blogUrl = myDeserializedData.Blog.Url;

            //forming your email

            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v3"); 
            client.Authenticator =   new HttpBasicAuthenticator("api", "YOUR-API-KEY");
            RestRequest request = new RestRequest();
            request.AddParameter("domain", "sandbox71aaf3a4083d41fbaf81858054139ab9.mailgun.org", ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", "ryanattard83@gmail.com");
            request.AddParameter("to", email);
            request.AddParameter("subject", "Receipt for Inputting bLog");
            request.AddParameter("text", "blog " +blogTitle+ " registered with url" + blogUrl);
            request.Method = Method.POST;
            var response= client.Execute(request);
            return Content(response.StatusCode.ToString()); 
        }
    }
}
