using System;
using NUnit.Framework;
using BAPT.ServiceInterface;
using BAPT.ServiceModel;
using ServiceStack.Testing;
using ServiceStack;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace BAPT.Tests
{
    [TestFixture]
    public class UnitTests
    {
        private readonly ServiceStackHost appHost;

        public UnitTests()
        {
            appHost = new BasicAppHost(typeof(BAPTServices).Assembly)
            {
                ConfigureContainer = container =>
                {
                    //Add your IoC dependencies here
                }
            }
            .Init();
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            appHost.Dispose();
        }

        [Test]
        public void CreateBAPTRequests()
        {

            var service = appHost.Container.Resolve<BAPTServices>();
            BAPTRequest request1 = new BAPTRequest { Location="2", Address = "4207 BURNET RD", Address2 = "", City = "AUSTIN", State = "TX", Zip = "78756", Phone = "5127061900" };
            BAPTRequest request2 = new BAPTRequest { Location = "7", Address = "5314 DASHWOOD DR", Address2 = "", City = "HOUSTON", State = "TX", Zip = "77081", Phone = "7132955300" };
            BAPTRequest request3 = new BAPTRequest { Location = "8", Address = "2001 HERMANN DR", Address2 = "", City = "HOUSTON", State = "TX", Zip = "77004", Phone = "713878" };
            BAPTRequest request4 = new BAPTRequest { Location = "12", Address = "2200 ROSS AVE", Address2 = "", City = "Dallas", State = "TX", Zip = "75201", Phone = "469828" };
          
          

            List<BAPTRequest>  addresses = new List<BAPTRequest>();

           addresses.Add(request1);
           addresses.Add(request2);
           addresses.Add(request3);
           addresses.Add(request4);
           BAPTRequests requests = new BAPTRequests();
           requests.Addresses = addresses;



            BAPTResponse response =(BAPTResponse) service.Any(requests);
           
            Assert.Greater(response.BAPTId, 0 );
            Assert.Greater(response.EntelegentQuote.Length, 0);
        }
          [Test]
        public void GetDetailByEntQuote()
        {
            var service = appHost.Container.Resolve<BAPTServices>();
            EntQuote quote = new EntQuote();
            quote.Id = 17155;
            BAPTResponseDetailResponse response = (BAPTResponseDetailResponse)service.Any(quote);
            Assert.Greater(response.Details.Count, 0);

        }
          [Test]
          public void GetDetailByBAPTQuote()
          {
              var service = appHost.Container.Resolve<BAPTServices>();
              BaptNumber quote = new BaptNumber();
              quote.Id = 10000026;
              BAPTResponseDetailResponse response = (BAPTResponseDetailResponse)service.Any(quote);
              Assert.Greater(response.Details.Count, 0);

          }
         [Test]
          //public async Task PushReadyQuoteFromEntelegentAsnc()
          //{
          //    var service = appHost.Container.Resolve<BAPTServices>();
          //    List<string> quotes = new List<string>() { "17163"};
          //    PushQuotes pushquotes = new PushQuotes();
          //    pushquotes.Quotes = quotes;

          //    //var response = await service.AsTaskResult Any(pushquotes);
          //    ReadyResponse response = await service.Any(pushquotes);

          //    Assert.AreEqual(response.Message,"Success");

          //}
         public void PushReadyQuoteFromEntelegent()
         {
             var service = appHost.Container.Resolve<BAPTServices>();
             List<string> quotes = new List<string>() { "17163" };
             PushQuotes pushquotes = new PushQuotes();
             pushquotes.Quotes = quotes;

             //var response = await service.AsTaskResult Any(pushquotes);
             ReadyResponse response = (ReadyResponse) service.Any(pushquotes);

             Assert.AreEqual(response.Message, "Success");

         }
    }
}
