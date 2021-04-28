using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.DataAccess.Interfaces;
using WebApplication1.Models.Domain;
using Google.Cloud.PubSub.V1;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Google.Protobuf;
using Grpc.Core;

namespace WebApplication1.DataAccess.Repositories
{
   public enum Category { economy, business, luxury}


    public class PubSubRepository : IPubSubRepository
    {
        string projectId;

        public PubSubRepository(IConfiguration config)
        {
            projectId = config.GetSection("AppSettings").GetSection("ProjectId").Value;
        }


        public void PublishMessage(Blog b, string email, string category)
        {
            TopicName topicName = TopicName.FromProjectTopic(projectId, "pfc2021topicra"); //topic = queue

            Task<PublisherClient> t = PublisherClient.CreateAsync(topicName); 
            t.Wait();
            PublisherClient publisher = t.Result  ; //after making an asynchronous call to get all the info i need to be able to make a 
                                                    //a request to use the queue/topic

            var myOnTheFlyObject = new { Email = email, Blog = b }; //anonymous object
            string myOnTheflyObject_serialized = JsonConvert.SerializeObject(myOnTheFlyObject);

            var pubsubMessage = new PubsubMessage
            {
                // The data is any arbitrary ByteString. Here, we're using text.
                Data = ByteString.CopyFromUtf8(myOnTheflyObject_serialized),                //Ctrl + .
                // The attributes provide metadata in a string-to-string dictionary.
                Attributes =
                        {
                            { "category", category }
                        }
            };

            Task<string> t2 = publisher.PublishAsync(pubsubMessage); //initiating an asynchronous call to store msg in topic
            t2.Wait();
            string message = t2.Result ; //reference no/ id //log id date and time it was published
            
        }

        public string PullMessage(Category cat)
        {
             string subscriptionid = "pfc2021subscriptionra3";
            switch (cat)
            {
                case Category.business:
                    subscriptionid = "pfc2021subscriptionra3";
                    break;

                case Category.economy:
                    subscriptionid = "pfc2021subscriptionra4";
                    break;
            }


            SubscriptionName subscriptionName = SubscriptionName.FromProjectSubscription(projectId, subscriptionid);
            SubscriberServiceApiClient subscriberClient = SubscriberServiceApiClient.Create();
            int messageCount = 0;string text ="";
            try
            {
                // Pull messages from server,
                // allowing an immediate response if there are no messages.
                PullResponse response = subscriberClient.Pull(subscriptionName, returnImmediately: true, maxMessages: 1);
                // Print out each received message.
                if (response.ReceivedMessages.Count > 0)
                {
                    var msg = response.ReceivedMessages.FirstOrDefault();
                    if (msg != null)
                    { 
                        text = msg.Message.Data.ToStringUtf8();
                    }

                    //  subscriberClient.Acknowledge(subscriptionName, response.ReceivedMessages.Select(msg => msg.AckId)); //this acknowledges more than 1 message 
                    subscriberClient.Acknowledge(subscriptionName, new List<string>() { msg.AckId });
                }
            }
            catch (RpcException ex) when (ex.Status.StatusCode == StatusCode.Unavailable)
            {
                // UNAVAILABLE due to too many concurrent pull requests pending for the given subscription.

                //log when there is an error
            }
            return text;

        }
    }
}
