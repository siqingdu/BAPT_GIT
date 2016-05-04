using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack;

namespace BAPT.ServiceModel
{

    #region get BAPT quote response detail
    [Route("/api/BAPTQuote/{Id}", "GET", Summary = "GET Entelegent response detail", Notes = "Pass BAPT  Id")]
    public class BaptNumber : IReturn<BAPTResponseDetailResponse>
    {

        public Int32 Id { get; set; }

    }

    #endregion
    #region get Entelegent Quote detail

    [Route("/api/EntQuote/{Id}", "GET", Summary = "GET Entelegent Quote detail", Notes="Pass Entelegent Quote Id")]
    public class EntQuote : IReturn<BAPTResponseDetailResponse>
    {

        public Int32 Id { get; set; }

    }
    public class EntelegentQuoteResponse
    {

        public GetEntelegentQuoteResponseDTO  Detail { get; set; }
        public ResponseStatus ResponseStatus { get; set; }
    }
    public class BAPTResponseDetailResponse
    {

        public List<ResponseDetailDTO> Details { get; set; }
        public ResponseStatus ResponseStatus { get; set; }
    }
    public class ResponseDetailDTO
{
        public int Id { get; set; }
        public string VendorQuote { get; set; }
        //public int BAPTNumber { get; set; }
        public string BroadbandType { get; set; }
        public string PriceGroup { get; set; }
        public string Prequal { get; set; }
        public string LocationName { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string BTN { get; set; }
        public string Ilec { get; set; }
        public string CableProvider { get; set; }
        public string MultiIPATT { get; set; }
        public string MessageComcast { get; set; }
        public string RecommendationsCharter { get; set; }
        public string ReasonCharter { get; set; }
        public string ServiceabilityTWC { get; set; }
        public string MessageWOW { get; set; }
        public string NewWaveAvailability { get; set; }
        public string MegaPathAvailability { get; set; }
        public string LoopType { get; set; }
        public string iptype { get; set; }
        public string Download { get; set; }
        public string Upload { get; set; }
        //public string MRC { get; set; }
        //public string NRC { get; set; }
        //public string professionalinstall { get; set; }
        public string Term { get; set; }
        public string Customer_Reference { get; set; }
        public Nullable<System.DateTime> DateReceieved { get; set; }
        public double XOMRC { get; set; }
        public double XONRC { get; set; }
        public string Network { get; set; }
}
    public class EntelegentQuote
    {
        public string Quote_Id { get; set; }
        public string BaptId { get; set; }
        public string BroadbandType { get; set; }
        public string PriceGroup { get; set; }
        public string Prequal { get; set; }
        public string LocationName { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string BTN { get; set; }
        public string Ilec { get; set; }
        public string CableProvider { get; set; }
        public string MultiIP_ATT { get; set; }
        public string Message_Comcast { get; set; }
        public string Recommendations_Charter { get; set; }
        public string Reason_Charter { get; set; }
        public string Serviceability_TWC { get; set; }
        public string Message_WOW { get; set; }
        public string NewWave_Availability { get; set; }
        public string MegaPath_Availability { get; set; }
        public string looptype { get; set; }
        public string iptype { get; set; }
        public string download { get; set; }
        public string upload { get; set; }
        public string mrc { get; set; }
        public string nrc { get; set; }
        public string professionalinstall { get; set; }
        public string term { get; set; }
        public string Customer_Reference { get; set; }
        public string APIKey { get; set; }

    }
    public class GetEntelegentQuoteResponseDTO
    {
        public List<EntelegentQuote> quoted_broadband_pricing { get; set; }
      
    }
   
    
    #endregion
    
    #region BAPT Push

     [Route("/api/PushQuotes/{Ids}")]
    public class PushQuotes: IReturn<ReadyResponse>
    {

         public List<string> Quotes { get; set; }

    }

     public class ReadyResponse
    {
        public string Message { get; set; }
      
        public ResponseStatus ResponseStatus { get; set; }
    }
   
    
    #endregion
    #region BAPTRequest
    [Route("/api/BAPTRequests")]
    public class BAPTRequests : IReturn<BAPTResponse>
    {

        public List<BAPTRequest> Addresses { get; set; }
     
    }

 
    public class BAPTRequest
    {
        public string Location { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }
    }
    public class BAPTResponse
    {
        public int BAPTId { get; set; }
        public string EntelegentQuote { get; set; }
        public ResponseStatus ResponseStatus { get; set; }
    }
    public class CreateEntelegentQuoteRequest
    {
        public string QuoteName { get; set; }
        public string ServiceType { get; set; }
        public string LocationName { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Btn { get; set; }
        public string CustomerReference { get; set; }
        public string APIKey { get; set; }
    }
    public class EntelegentResponse
    {
        public string Quote_Id { get; set; }
        public int BAPTId { get; set; }
        public Exception Error { get; set; }
    }
    
    #endregion




   

  
}