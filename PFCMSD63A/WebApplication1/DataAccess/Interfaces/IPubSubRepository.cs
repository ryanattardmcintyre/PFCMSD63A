using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.DataAccess.Repositories;
using WebApplication1.Models.Domain;

namespace WebApplication1.DataAccess.Interfaces
{
   public  interface IPubSubRepository
    {
        void PublishMessage(Blog b, string email, string category);

        string PullMessage(Category cat);
    }
}
