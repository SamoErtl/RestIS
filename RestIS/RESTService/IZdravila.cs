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

        // izpis zdravil
        [OperationContract]
        [WebGet(UriTemplate = "Zdravila", ResponseFormat = WebMessageFormat.Json)]
        List<Zdravilo> VrniSeznamZdravil();

        [OperationContract]
        [WebGet(UriTemplate = "Zdravila/{id}", ResponseFormat = WebMessageFormat.Json)]
        Zdravilo VrniZdravila(string id);

        // Zdravilo

        [OperationContract]
        [WebInvoke(UriTemplate = "Zdravilo", ResponseFormat = WebMessageFormat.Json)]
        void DodajZdravilo(Zdravilo zdravilo);

        [OperationContract]
        [WebInvoke(UriTemplate = "Zdravilo/{Name}", ResponseFormat = WebMessageFormat.Json, Method = "DELETE")]
        void IzbrisiZdravilo(string Name);

        [OperationContract]
        [WebInvoke(UriTemplate = "Zdravilo/{id}", ResponseFormat = WebMessageFormat.Json, Method ="PUT")]
        void PosodobiZdravilo(Zdravilo zdravilo, string id);

        // Manufactured
        [OperationContract]
        [WebInvoke(UriTemplate = "Manufacturer", ResponseFormat = WebMessageFormat.Json)]
        void DodajManu(Manufacturer manu);

        [OperationContract]
        [WebInvoke(UriTemplate = "Manufacturer/{Name}", ResponseFormat = WebMessageFormat.Json, Method = "DELETE")]
        void IzbrisiManu(string Name);

        [OperationContract]
        [WebInvoke(UriTemplate = "Manufacturer/{id}", ResponseFormat = WebMessageFormat.Json, Method = "PUT")]
        void PosodobiManu(Manufacturer manu, string id);

        // Address
        [OperationContract]
        [WebInvoke(UriTemplate = "Address", ResponseFormat = WebMessageFormat.Json)]
        void DodajAddress(Address manu);

        [OperationContract]
        [WebInvoke(UriTemplate = "Address", ResponseFormat = WebMessageFormat.Json, Method = "DELETE")]
        void IzbrisiAddress(Address Name);

        [OperationContract]
        [WebInvoke(UriTemplate = "Address/{id}", ResponseFormat = WebMessageFormat.Json, Method = "PUT")]
        void PosodobiAddress(Address manu, string id);


        // User
        [OperationContract]
        [WebInvoke(UriTemplate = "NewUserdb", ResponseFormat = WebMessageFormat.Json)]
        void DodajUser(dbUser manu);

        [OperationContract]
        [WebInvoke(UriTemplate = "DelUserdb/{Name}", ResponseFormat = WebMessageFormat.Json, Method = "DELETE")]
        void IzbrisiUser(string Name);

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
    public int Id_med { get; set; }
    [DataMember]
    public int Id_manu { get; set; }

    }

    [DataContract]
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

    [DataContract]
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

    [DataContract]
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

    [DataContract]
    public class dbUser
    {
        [DataMember]
        public string Username { get; set; }
        [DataMember]
        public string Pass { get;  }
        [DataMember]
        public string Mail { get; set; }
        [DataMember]
        public int MOD { get; set; }
        [DataMember]
        public string Id_Addr { get; set; }

    }


}
