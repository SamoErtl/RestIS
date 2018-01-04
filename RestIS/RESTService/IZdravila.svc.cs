using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Data.SqlClient;

namespace RESTService
{
    public class Service1 : IZdravila
    {
        // SQL Server in Azure
        //string cs = ConfigurationManager.ConnectionStrings["dbosebeCS"].ConnectionString;

       
        string cs = ConfigurationManager.ConnectionStrings["DBZdravilnicaConnectionString"].ConnectionString;

        private int AuthenticateUser(int mod)
        {
            WebOperationContext ctx = WebOperationContext.Current;
            string authHeader = ctx.IncomingRequest.Headers[HttpRequestHeader.Authorization];
            if (authHeader == null)
                return -2;

            string[] loginData = authHeader.Split(':');
            dbUser usr = Login(loginData[0], loginData[1]);
            if (loginData.Length == 2 && mod == usr.MOD) 
                return usr.MOD;
            return -1;
        }

        public dbUser Login(string username, string password)
        {
            int i = 0;
            dbUser usr = new dbUser();

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                string sql = "SELECT Username, Mod FROM DbUser" +
                    " WHERE Username = @user AND Password = @pass ";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.Add(new SqlParameter("user", username));
                cmd.Parameters.Add(new SqlParameter("pass", password));

                using (SqlDataReader reader = cmd.ExecuteReader(System.Data.CommandBehavior.SingleRow))
                {
                    if (reader.Read())
                    {
                        usr.Username = reader.GetString(1);
                        usr.MOD = Convert.ToInt32(reader[1]);
                        i++;
                    }
                }
            }
            if(i==1)
                return usr;
            //neki je narobe nesme meti pravic.
            usr.MOD = -1;
            return usr;
        }


        public Zdravilo VrniZdravila(string ime)
        {
            Zdravilo zdravilo = new Zdravilo();

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                string sql = "SELECT  *" +
                    "FROM Medicine" +
                    "WHERE MedicineName LIKE @param" +
                    "ORDER BY MedicineName";
                /*string sql = "SELECT m.MedicineName, m.MedicineName, m.MedicineDescription, m.MedicineInstruction," +
                    " fac.ManufacturerName " +
                    "FROM Medicine as m LEFT JOIN Manufacturer as fac ON m.ID_Manufacturer = fac.ID_Manufacturer" +
                    "WHERE m.MedicineName LIKE @param" +
                    "ORDER BY m.MedicineName";*/
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.Add(new SqlParameter("param", ime));

                using (SqlDataReader reader = cmd.ExecuteReader(System.Data.CommandBehavior.SingleRow))
                {
                    if (reader.Read())
                    {
                        zdravilo.Name = reader.GetString(0);
                        zdravilo.NameLat = reader.GetString(1);
                        zdravilo.Descr = reader.GetString(2);
                        zdravilo.Inst = reader.GetString(3);
                        zdravilo.Id_med = Convert.ToInt32(reader[4]);
                        zdravilo.Id_manu = Convert.ToInt32(reader[5]);
                    }
                }
                con.Close();
                return zdravilo;
            }
            
        }

        public List<Zdravilo> VrniSeznamZdravil()
        {
            var retVal = new List<Zdravilo>();
            
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                /*string sql = "SELECT m.MedicineName, m.MedicineName, m.MedicineDescription, m.MedicineInstruction," +
                    " fac.ManufacturerName " +
                    "FROM Medicine as m LEFT JOIN Manufacturer as fac ON m.ID_Manufacturer = fac.ID_Manufacturer" +
                    "ORDER BY m.MedicineName";*/
                string sql = "SELECT  *" +
                    "FROM Medicine" +
                    "ORDER BY MedicineName";
                SqlCommand cmd = new SqlCommand(sql, con);


                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        retVal.Add(new Zdravilo
                        { Name = reader.GetString(0),
                            NameLat = reader.GetString(1),
                            Descr = reader.GetString(2),
                            Inst = reader.GetString(3),
                            Id_med = Convert.ToInt32(reader[4]),
                            Id_manu = Convert.ToInt32(reader[5])
                        });
                      
                    }
                }
                con.Close();
                return retVal;

            }


            return retVal;
        }


        // zdravila 
        public void DodajZdravilo(Zdravilo zdr)
        {

            if (AuthenticateUser(0)==0)
                throw new FaultException("Napačno uporabniško ime, geslo ali premalo moči.");

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                string sql =
                    "INSERT INTO Medicine (MedicineName, MedicineNameLat, MedicineDescription, MedicineInstruction, ID_Medicine, ID_Manufacturer)" +
                    "VALUES (@Name, @NameLat, @Descr, @Inst, @IdManu)";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.Add(new SqlParameter("Name", zdr.Name));
                cmd.Parameters.Add(new SqlParameter("NameLat", zdr.NameLat));
                cmd.Parameters.Add(new SqlParameter("Descr", zdr.Descr));
                cmd.Parameters.Add(new SqlParameter("Inst", zdr.Inst));
                cmd.Parameters.Add(new SqlParameter("IdManu", zdr.Id_manu));
                cmd.ExecuteNonQuery();
                con.Close();

            }

        }

        public void IzbrisiZdravilo(string Name)
        {

            if (AuthenticateUser(0)==0)
                throw new FaultException("Napačno uporabniško ime, geslo ali premalo moči.");

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                string sql = "DELETE FROM Medicine WHERE MedicineName = @param";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.Add(new SqlParameter("param", Name));
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }

        public void PosodobiZdravilo(Zdravilo zdravilo, string id)
        {
            if (AuthenticateUser(0)==0)
                throw new FaultException("Napačno uporabniško ime, geslo ali premalo moči.");

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                string sql =
                    "UPDATE Medicine set MedicineName=@0 MedicineNameLat@1, MedicineDescription@2, MedicineInstruction=@3 ID_Manufacturer =@4 WHERE MedicineName=@0";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.Add(new SqlParameter("0", zdravilo.Name));
                cmd.Parameters.Add(new SqlParameter("1", zdravilo.NameLat));
                cmd.Parameters.Add(new SqlParameter("2", zdravilo.Descr));
                cmd.Parameters.Add(new SqlParameter("3", zdravilo.Inst));
                cmd.Parameters.Add(new SqlParameter("4", zdravilo.Id_manu));
                cmd.ExecuteNonQuery();
                con.Close();

            }
        }



        // dodaj izbris posodobitev manufac mod 1

        public void DodajManu(Manufacturer zdr)
        {

            if (AuthenticateUser(1) == 1)
                throw new FaultException("Napačno uporabniško ime, geslo ali premalo moči.");

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                string sql =
                    "INSERT INTO Manufacturer (ManufacturerName, ManufacturerTel, ID_address)" +
                    "VALUES (@Name, @Tel, @Addr)";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.Add(new SqlParameter("Name", zdr.Name));
                cmd.Parameters.Add(new SqlParameter("Tel", zdr.NameTel));
                cmd.Parameters.Add(new SqlParameter("Addr", zdr.Id_addr));
                cmd.ExecuteNonQuery();
                con.Close();

            }

        }

        public void IzbrisiManu(string Name)
        {

            if (AuthenticateUser(1) == 1)
                throw new FaultException("Napačno uporabniško ime, geslo ali premalo moči.");

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                string sql = "DELETE FROM Manufacturer WHERE ManufacturerName = @param";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.Add(new SqlParameter("param", Name));
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }

        public void PosodobiManu(Manufacturer zdravilo, string id)
        {
            if (AuthenticateUser(1) == 1)
                throw new FaultException("Napačno uporabniško ime, geslo ali premalo moči.");

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                
                string sql =
                    "UPDATE Manufacturer set ManufacturerName=@0 ManufacturerTel@1, ID_Address@2 WHERE ManufacturerName=@0";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.Add(new SqlParameter("0", zdravilo.Name));
                cmd.Parameters.Add(new SqlParameter("1", zdravilo.NameTel));
                cmd.Parameters.Add(new SqlParameter("2", zdravilo.Id_addr));

                cmd.ExecuteNonQuery();
                con.Close();

            }
        }
        // dodaj izbris posodobitev address mod 1
        //country postnum city street housenum

        public void DodajAddress(Address zdr)
        {

            if (AuthenticateUser(1) == 1)
                throw new FaultException("Napačno uporabniško ime, geslo ali premalo moči.");

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                string sql =
                    "INSERT INTO Address (Country, PostNumber,City, Street, HouseNumber)" +
                    "VALUES (@1, @2, @3, @4, @5)";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.Add(new SqlParameter("1", zdr.Country));
                cmd.Parameters.Add(new SqlParameter("2", zdr.PostNum));
                cmd.Parameters.Add(new SqlParameter("3", zdr.City));
                cmd.Parameters.Add(new SqlParameter("4", zdr.Street));
                cmd.Parameters.Add(new SqlParameter("5", zdr.HouseNum));
                cmd.ExecuteNonQuery();
                con.Close();

            }

        }

        public void IzbrisiAddress(Address addrs)
        {

            if (AuthenticateUser(1) == 1)
                throw new FaultException("Napačno uporabniško ime, geslo ali premalo moči.");

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                string sql = "DELETE FROM Addrs WHERE ID_Address = @param";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.Add(new SqlParameter("param", addrs.Id_Addres));
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }

        public void PosodobiAddress(Address addrs, string id)
        {
            if (AuthenticateUser(1) == 1)
                throw new FaultException("Napačno uporabniško ime, geslo ali premalo moči.");

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                //popravi
                string sql =
                    "UPDATE Address set Country=@1 PostNumber@2, City =@3, Street = @4, HouseNumber= @5 WHERE ID_address=@0";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.Add(new SqlParameter("0", addrs.Id_Addres));
                cmd.Parameters.Add(new SqlParameter("1", addrs.Country));
                cmd.Parameters.Add(new SqlParameter("2", addrs.PostNum));
                cmd.Parameters.Add(new SqlParameter("3", addrs.City));
                cmd.Parameters.Add(new SqlParameter("4", addrs.Street));
                cmd.Parameters.Add(new SqlParameter("5", addrs.HouseNum));

                cmd.ExecuteNonQuery();
                con.Close();

            }
        }
        // add remove user  mod 2

        public void DodajUser(dbUser zdr)
        {

            if (AuthenticateUser(2) == 2)
                throw new FaultException("Napačno uporabniško ime, geslo ali premalo moči.");

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                string sql =
                    "INSERT INTO DBUser (Username, Password, Email, Mod)" +
                    "VALUES (@Name, @Pass, @Mail, @Mod)";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.Add(new SqlParameter("Name", zdr.Username));
                cmd.Parameters.Add(new SqlParameter("Pass", zdr.Pass));
                cmd.Parameters.Add(new SqlParameter("Mail", zdr.Mail));
                cmd.Parameters.Add(new SqlParameter("Mod", zdr.MOD));
                cmd.ExecuteNonQuery();
                con.Close();

            }

        }

        public void IzbrisiUser(string Name)
        {

            if (AuthenticateUser(2) == 2)
                throw new FaultException("Napačno uporabniško ime, geslo ali premalo moči.");

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                string sql = "DELETE FROM DBUser WHERE Username = @param";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.Add(new SqlParameter("param", Name));
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }

    }
}
