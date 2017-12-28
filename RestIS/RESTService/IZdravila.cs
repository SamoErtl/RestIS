using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace RESTService
{
    [ServiceContract]
    public interface IZdravila
    {
        [OperationContract]
        [WebGet(UriTemplate = "Zdravila", ResponseFormat = WebMessageFormat.Json)]
        List<Zdravilo> VrniSeznamZdravil();

        [OperationContract]
        [WebGet(UriTemplate = "Zdravila/{id}", ResponseFormat = WebMessageFormat.Json)]
        Zdravilo VrniZdravila(string id);

        [OperationContract]
        [WebInvoke(UriTemplate = "Zdravilo", ResponseFormat = WebMessageFormat.Json)]
        void DodajZdravilo(Zdravilo oseba);

        [OperationContract]
        [WebInvoke(UriTemplate = "Zdravilo/{id}", ResponseFormat = WebMessageFormat.Json, Method = "DELETE")]
        void IzbrisiZdravilo(string Name, string NameLat);

        [OperationContract]
        [WebInvoke(UriTemplate = "Zdravilo/{id}", ResponseFormat = WebMessageFormat.Json, Method ="PUT")]
        void PosodobiZdravilo(Zdravilo zdravilo, string id);
    }

    [DataContract]
    public class Zdravilo
    {
    [DataMember]
    public string Name { get; set; }
    [DataMember]
    public string NameLat { get; set; }
    [DataMember] 
    public string Descr { get; set; }
    [DataMember]
    public string Inst { get; set; }
    [DataMember]
    public int Id_med { get; }
    [DataMember]
    public int Id_manu { get; set; }

    }

    public class Manufacturer
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string NameTel { get; set; }
        [DataMember]
        public int Id_Manu { get; }
        [DataMember]
        public int Id_addr { get; set; }

    }

    public class Address
    {
        [DataMember]
        public string Country { get; set; }
        [DataMember]
        public int PostNum { get; set; }
        [DataMember]
        public string City { get; set; }
        [DataMember]
        public string Street { get; set; }
        [DataMember]
        public string HouseNum { get; set; }
        [DataMember]
        public int Id_Addres { get; }
    }

    public class Pharmacy
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Tel { get; set; }
        [DataMember]
        public int Id_Pha { get; }
        [DataMember]
        public int Id_Addr{ get; set; }

    }

    public class dbUser
    {
        [DataMember]
        public string Username { get; set; }
        [DataMember]
        public string pass { get;  }
        [DataMember]
        public int MOD { get; set; }
        [DataMember]
        public string Id_Addr { get; set; }

    }


}
