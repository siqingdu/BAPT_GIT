using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack;
using BAPT.ServiceModel;
using BAPT.Database;
using System.Configuration;
using ServiceStack.Text;
using System.Threading.Tasks;

namespace BAPT.ServiceInterface
{
    public class BAPTServices : Service
    {
       

        #region get Entelegent Quote detail
        public BAPTResponseDetailResponse Any(EntQuote quote)
        {
            //EntelegentQuoteResponse result = new EntelegentQuoteResponse();
            //GetEntelegentQuoteResponseDTO response = GetEntelegentQuote(quote.QuoteId);

            //return new BAPTResponseDetailResponse { Detail = response };
            return GetDetailResponse(quote.Id, "EntQuote");

        }
        public BAPTResponseDetailResponse Any(BaptNumber quote)
        {

            return GetDetailResponse(quote.Id, "BAPT");
          

        }
        private BAPTResponseDetailResponse GetDetailResponse(int id, string idtype)
        {
           

                using (var context = new BAPTEntities())
                {
                    string vendorquote = string.Empty;
                    if (idtype == "BAPT")
                    {
                        vendorquote = (from c in context.BAPTMasters
                                       where c.Id == id 
                                       select c.VendorQuote
                                           ).FirstOrDefault();
                    }
                    else
                        vendorquote = id.ToString();
                    if (vendorquote != null && vendorquote != string.Empty)
                    {
                        List<ResponseDetail> details = (from c in context.ResponseDetails

                                                       where c.VendorQuote == vendorquote &&
                                                       c.XOMRC != "" && c.XONRC != ""
                                                       orderby c.LocationName, c.LoopType,c.iptype,c.PriceGroup,c.Upload,c.Download,c.Term
                                                        select c
                                                           ).ToList<ResponseDetail>();
                        List<ResponseDetailDTO> responsedetails = details.ConvertAll(x => x.ConvertTo<ResponseDetailDTO>());

                       // List<ResponseDetailDTO> responsedetails = details.ConvertTo<ResponseDetailDTO>() as List<ResponseDetailDTO>;
                        foreach (ResponseDetailDTO dto in responsedetails)
                        {
                            dto.XOMRC = System.Convert.ToDouble(dto.XOMRC.ToString("F"));
                            dto.XONRC = System.Convert.ToDouble(dto.XONRC.ToString("F"));

                        }
                        return new BAPTResponseDetailResponse { Details = responsedetails };
                    }
                    else
                        return new BAPTResponseDetailResponse { ResponseStatus = new ResponseStatus { Message = "Bapt Quote not found or not ready" } };

                }
            
        }
        private  GetEntelegentQuoteResponseDTO GetEntelegentQuote(string entQuoteId)
        {
           
            string apiGetUrl = System.Configuration.ConfigurationSettings.AppSettings["GetQuoteURL"].ToString();
            apiGetUrl = apiGetUrl.Replace("##quoteid", entQuoteId);
            //apiGetUrl = "http://54.197.243.180/api.php/quoted_broadband_pricing?filter[]=APIKey,eq,13QfckyVbac4-5OZd8chG%2B%3DACv2iy*-Z_%3DwdAo9TC37RYV90K%3D~xt61j12fZ&amp;filter[]=Quote_Id,eq,17155";
            var client = new JsonServiceClient(apiGetUrl);
           

            GetEntelegentQuoteResponseDTO response = client.Get<GetEntelegentQuoteResponseDTO>("");

            return response;
        }
        #endregion

        #region BAPT push from Entelgent
        //public async Task<ReadyResponse> Any(PushQuotes Ids)
        //  {
        //      if (ValidateIds(Ids))
        //      {
        //          bool processquote = await ProcessQuote(Ids);
        //          return new ReadyResponse { Message = "Success" };

        //          //if (ProcessQuote(Ids))
        //          //    return new ReadyResponse { Message = "Success" };
        //          //else
        //          //    return new ReadyResponse { Message = "Failed" };
        //      }
        //      else
        //            return new ReadyResponse { Message = "Invalid Ids" };
              
        //  }
        public ReadyResponse Any(PushQuotes Ids)
        {
            if (ValidateIds(Ids))
            {
                //bool processquote = await ProcessQuote(Ids);
                //return new ReadyResponse { Message = "Success" };

                if (ProcessQuote(Ids))
                    return new ReadyResponse { Message = "Success" };
                else
                    return new ReadyResponse { Message = "Failed" };
            }
            else
                return new ReadyResponse { Message = "Invalid Ids" };

        }
       private bool ProcessQuote(PushQuotes Ids)
        {
          bool ret = true;
          using (var context = new BAPTEntities())
          {
              foreach (string id in Ids.Quotes)
              {
                  var idExist = (from _quote in context.BAPTMasters
                                 where _quote.VendorQuote == id 
                                 select _quote.Id

                      ).FirstNonDefault();
                  if (idExist != null && idExist.ToString().Length > 0)
                  {

                      //delete all the detail response for the given Entelequote
                      List<ResponseDetail> deleteDetail = (from d in context.ResponseDetails
                                                           where d.VendorQuote == id
                                                           select d
                                        ).ToList<ResponseDetail>();
                      if (deleteDetail.Count > 0)
                      {
                          context.ResponseDetails.RemoveRange(deleteDetail);
                          context.SaveChanges();
                      }
                      //get entQuote
                      GetEntelegentQuoteResponseDTO entquote = GetEntelegentQuote(id);
                      //Insert the entire response for logging/tracking
                      BAPTRequestReady pushquote = new BAPTRequestReady();

                      pushquote.VendorQuote = id;
                      pushquote.VendorQuoteResponse = entquote.SerializeToString();
                      pushquote.DateCreated = System.DateTime.Now;
                      context.BAPTRequestReadies.Add(pushquote);

                      //get the base price
                      List<Markup> baseprices = (from b in context.Markups select b).ToList<Markup>();

                      //insert into individual response detail

                      foreach (EntelegentQuote row in entquote.quoted_broadband_pricing)
                      {
                          ResponseDetail responsedetail = CreateResponseDetail(row);

                          int download = CovertSpeed(responsedetail.Download);
                          int upload = CovertSpeed(responsedetail.Upload);
                          int term = Convert.ToInt16(responsedetail.Term);

                          var price = (from p in context.BAPTXOPricings
                                       where p.PriceGroup == responsedetail.PriceGroup &&
                                             p.DownLoad == download &&
                                             p.Upload == upload &&
                                             p.Term == term &&
                                             p.IPType == responsedetail.iptype &&
                                             p.LoopType == responsedetail.LoopType
                                       select new
                                       {
                                           XOMRC = p.MRC,
                                           XONRC = p.NRC,
                                           XOInstall=p.Install,
                                           Network = p.Network
                                       }


                                    ).FirstOrDefault();
                          decimal xomrc=0.00M;
                          decimal xonrc=0.00M;
                          decimal xoinstall=0.00M;
                          if (price != null)
                          {
                              xomrc = MarkupPrice(System.Convert.ToDecimal(price.XOMRC), term, "MRC", "BMS", baseprices);
                              xonrc = MarkupPrice(System.Convert.ToDecimal(price.XONRC), term, "NRC", "BMS", baseprices);
                              xoinstall = MarkupPrice(System.Convert.ToDecimal(price.XOInstall), term, "Install", "BMS", baseprices);
                              xonrc = xonrc + xoinstall;

                          }

                          responsedetail.XOMRC = (price != null) ? xomrc.ToString() : "";
                          responsedetail.XONRC = (price != null) ? xonrc.ToString() : "";
                          responsedetail.Network = (price != null) ? price.Network.ToString() : "";
                          context.ResponseDetails.Add(responsedetail);

                      }

                      //need to update the master's status equal to Received. We are done
                      context.BAPTMasters.Find(idExist).Status = "Received";
                      context.SaveChanges();
                     // call to inform Pics
                      if (NotifyPics(idExist.ToString()))
                      {
                          context.BAPTMasters.Find(idExist).Status = "Notified to PICs";
                          context.SaveChanges();
                      }
                      else
                          ret = false;

                  }
                  else
                  {
                      // the entelegent quote either not exist or already received, skip
                  }

              }

          }
        
          return ret;
        }
       private decimal MarkupPrice(decimal price, int term, string priceType, string segement, List<Markup> baseprices)
       {
           decimal ret = price;
           if (priceType == "Install")
               term = 0;
           var markupNumber = (from b in baseprices 
                                   where b.MarkupTerm == term && b.MarkupType == priceType && b.Segement == segement select b.MarkupPercent).FirstOrDefault();
           if (markupNumber != null)
           {
               ret = System.Convert.ToDecimal(markupNumber) * price;
           }
          

           return ret;

       }
        private  bool NotifyPics(string baptId)
        {
            //PICS.p..batchCallback pics = new PICS.batchCallback();
            //pics.arg0 = 4;
            //pics.arg1 = baptId;
            
            //pics.batchCallback();
            //PICs.batchCallback pics = new PICs.PricingService();
            //PICs.batchCallback request=new PICs.batchCallback();
           
            //request.(4,baptId);
            //pics.batchCallback(request);
            //string picsURL = System.Configuration.ConfigurationSettings.AppSettings["PICSURL"] ?? "http://plajbsdev02:12580/pics/PricingService";

            //var client = new Soap12ServiceClient(picsURL);
            //PICs.batchCallbackResponse response = client.Send<PICs.batchCallbackResponse>(new PICs.batchCallback { arg0 = 4, arg1 = baptId });

            //string message = response.response;

          
            //PICs.PricingServiceClient client = new PICs.PricingServiceClient();

            PICs.pricingPortTypeClient client = new PICs.pricingPortTypeClient();

            string message=client.batchCallback(4, baptId);
            if (message == "Success")
                return true;
            else
                return false;
           
        }
        private int CovertSpeed(string speed)
        {
            try
            {
                if (speed.Contains("Mbps"))
                {
                    speed = speed.Replace("Mbps", "").Trim();
                    return Convert.ToInt32(Convert.ToDouble(speed) * 1000);
                }
                else if (speed.Contains("Kbps"))
                {
                    speed = speed.Replace("Kbps", "").Trim();
                    return Convert.ToInt32(Convert.ToDouble(speed));
                }
                else if (speed.Contains("N/A") || speed.Contains("NA"))
                    return 0;
                else if (speed.Contains("K"))
                {
                    speed = speed.Replace("K", "").Trim();
                    return Convert.ToInt32(Convert.ToDouble(speed));
                }
                else
                    return Convert.ToInt32(Convert.ToDouble(speed));
            }
            catch
            {
                return 0;
            }
                 
        }
        private ResponseDetail CreateResponseDetail(EntelegentQuote input)
        {
            ResponseDetail responsedetail = new ResponseDetail();
          
            responsedetail = input.ConvertTo<ResponseDetail>();
            responsedetail.VendorQuote = input.Quote_Id;
            responsedetail.Term = input.term;
            responsedetail.LoopType = input.looptype;
            //special conversion since the column name are different
            responsedetail.MegaPathAvailability = input.MegaPath_Availability;
            responsedetail.MessageComcast = input.Message_Comcast;
            responsedetail.MessageWOW = input.Message_WOW;
            responsedetail.RecommendationsCharter = input.Recommendations_Charter;
            responsedetail.NewWaveAvailability = input.NewWave_Availability;
            responsedetail.DateReceieved = System.DateTime.Now;
            responsedetail.MultiIPATT = input.MultiIP_ATT;
            responsedetail.ReasonCharter = input.Reason_Charter;
            responsedetail.ServiceabilityTWC = input.Serviceability_TWC;
            responsedetail.Upload = input.upload;
            responsedetail.Download = input.download;
            responsedetail.MRC = input.mrc;
            responsedetail.NRC = input.nrc;
            responsedetail.DateReceieved = System.DateTime.Now;
            return responsedetail;

        }
        private bool ProcessQuote(string quoteId)
        {
            bool ret = true;
            
            return ret;
        }
        private bool ValidateIds(PushQuotes Ids)
        {
            bool ret = true;
            try
            {

                foreach (string id in Ids.Quotes)
                {
                    if (id.ToString().IsInt() != true)
                        ret = false;
                }
            }
            catch
            {
                ret = false;
            }
            return ret;
        }

        #endregion
        #region BAPT requests Post
        public BAPTResponse Any(BAPTRequests requests)
        {
            if (requests !=null && requests.Addresses!=null)
            {
            if (ValidateRequest(requests))
            {
                EntelegentResponse response= CreateBAPTRequest (requests.Addresses);
                return new BAPTResponse { BAPTId = response.BAPTId, EntelegentQuote=response.Quote_Id};
            }
                
            else
              return new BAPTResponse { ResponseStatus = new ResponseStatus { Message = "Incomplete Request" } };
            }
            else
             return new BAPTResponse { ResponseStatus = new ResponseStatus { Message = "Missing Request" } };
         

        }
        private bool ValidateRequest(BAPTRequests requests)
        {
            bool result=true;
           foreach (BAPTRequest request in requests.Addresses)
           {
            if (request.Address == null || request.City == null || request.Zip == null || request.State == null)
                
                 result= false;
           }
            return result;
        }
        private BAPTRequest fromRequestToBAPTRequest(BAPTRequest request)
        {
            BAPTRequest ret = new BAPTRequest();
            return ret;
        }
        private string GetUserIP()
        {
           // return  System.Web.Current.Request.Params["HTTP_CLIENT_IP"] ?? HttpContext.Current.Request.UserHostAddress;
            return base.Request.RemoteIp;
            
        }
        private BAPTMaster CreateMaster(List<BAPTRequest> requests)
        {
           

                BAPTMaster master = new BAPTMaster();
                BAPTRequests input = new BAPTRequests();
                input.Addresses = requests;
                master.RequeslInput = input.SerializeToString();
                master.Status = "Submitted";
                master.RequestCount = requests.Count;
                try
                {
                    master.RequestFrom = base.Request.RemoteIp ?? string.Empty;
                }
            catch
                { master.RequestFrom = "N/A"; }
              
                
                return master;


           
         
        }
        private BAPRequestDetail CreateDetail(BAPTRequest request, BAPTMaster master)
        {

                BAPRequestDetail detail = new BAPRequestDetail();
                detail.Address = request.Address;
                detail.Address2 = request.Address2 ?? "";
                detail.City = request.City;
                detail.Zip = request.Zip;
                detail.State = request.State;
                detail.Phone = request.Phone??"";
                detail.BAPTMaster = master;

               return detail;
          
        }
        private EntelegentResponse PostToEntelegent(List<BAPTRequest> requests, int masterId)
        {
            EntelegentResponse result = new EntelegentResponse();
            string createquoteURL = System.Configuration.ConfigurationSettings.AppSettings["CreateQuoteURL"] ?? "http://54.197.243.180/api.php/quote_request";
            string apiKey = System.Configuration.ConfigurationSettings.AppSettings["ApiKey"] ?? "13QfckyVbac4-5OZd8chG+=ACv2iy*-Z_=wdAo9TC37RYV90K=~xt61j12fZ";
         
            var client = new JsonServiceClient(createquoteURL);
            List<CreateEntelegentQuoteRequest> quoterequests = new List<CreateEntelegentQuoteRequest>();

            foreach (BAPTRequest request in requests)
            {
                CreateEntelegentQuoteRequest quoterequest = new CreateEntelegentQuoteRequest();
                // quoterequest.QuoteName = "";
                // quoterequest.ServiceType = "BroadBand";
                quoterequest.Location = request.Location;
                quoterequest.Address = request.Address;
                quoterequest.Address2 = request.Address2;
                quoterequest.City = request.City;
                quoterequest.State = request.State;
                quoterequest.Zip = request.Zip;
                quoterequest.APIKey = apiKey;
                quoterequest.Phone = request.Phone;
                quoterequest.APIKey = apiKey;
                quoterequest.CustomerReference = masterId.ToString();
                quoterequests.Add(quoterequest);
                
            }
            try
            {
                var quote = quoterequests.SerializeToString();
               
                var response = client.Post<string>("", quoterequests);
                if (response.Length > 0)
                {
                    var obj = JsonObject.Parse(response);
                    result.Quote_Id = obj.Get<string>("Quote_Id");
                    result.BAPTId = masterId;
                  
                }
                else
                {
                    result.Quote_Id = string.Empty;
                    result.BAPTId = masterId;
                   
                }
                return result;
            }
            catch(Exception ex)
            {
                result.Quote_Id = "Error";
                result.BAPTId = masterId;
                result.Error = ex;
                return result;
            }
           
        }

        private EntelegentResponse CreateBAPTRequest(List<BAPTRequest> requests)
        {
            
            using (var context = new BAPTEntities())
            {

                BAPTMaster master = CreateMaster(requests);
                context.BAPTMasters.Add(master);

                foreach (BAPTRequest request in requests)
                {
                 context.BAPRequestDetails.Add(CreateDetail(request,master));
                }
                
                context.SaveChanges();
              
                EntelegentResponse entelegentResponse = PostToEntelegent(requests, master.Id);
                if (entelegentResponse.Error == null && entelegentResponse.Quote_Id!=string.Empty)
                {
                    
                    context.BAPTMasters.Find(master.Id).VendorQuote = entelegentResponse.Quote_Id;
                    context.BAPTMasters.Find(master.Id).Status = "Sent";
 
                }
                else
                {
                    context.BAPTMasters.Find(master.Id).Exception = entelegentResponse.Error.SerializeToString();

                }

                context.SaveChanges();
                return entelegentResponse;

            }




        }

        #endregion
    } 
 
}