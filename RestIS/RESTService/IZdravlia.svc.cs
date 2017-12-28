﻿using System;
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

        private bool AuthenticateUser()
        {
            WebOperationContext ctx = WebOperationContext.Current;
            string authHeader = ctx.IncomingRequest.Headers[HttpRequestHeader.Authorization];
            if (authHeader == null)
                return false;

            string[] loginData = authHeader.Split(':');
            if (loginData.Length == 2 && Login(loginData[0], loginData[1])) 
                return true;
            return false;
        }

        public bool Login(string username, string password)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                string sql = "SELECT Username, Password, Mod FROM DbUser" +
                    " WHERE Username = @user ";
                SqlCommand cmd = new SqlCommand(sql, con);

                using (SqlDataReader reader = cmd.ExecuteReader(System.Data.CommandBehavior.SingleRow))
                {
                    if (reader.Read())
                    {
                        /*zdravilo.Name = reader.GetString(0);
                        zdravilo.NameLat = reader.GetString(1);
                        zdravilo.Descr = reader.GetString(2);
                        zdravilo.Inst = reader.GetString(3);
                        zdravilo.Id_manu = Convert.ToInt32(reader[4]);*/
                    }
                }
            }
                if (username.Equals("admin") && password.Equals("test"))
                return true;
            return false;
        }


        public Zdravilo VrniZdravila(string ime)
        {
            Zdravilo zdravilo = new Zdravilo();



            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                string sql = "SELECT m.MedicineName, m.MedicineName, m.MedicineDescription, m.MedicineInstruction," +
                    " fac.ManufacturerName " +
                    "FROM Medicine as m LEFT JOIN Manufacturer as fac ON m.ID_Manufacturer = fac.ID_Manufacturer" +
                    "WHERE m.MedicineName LIKE @param" +
                    "ORDER BY m.MedicineName";
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
                        zdravilo.Id_manu = Convert.ToInt32( reader[4]);
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
                string sql = "SELECT m.MedicineName, m.MedicineName, m.MedicineDescription, m.MedicineInstruction," +
                    " fac.ManufacturerName " +
                    "FROM Medicine as m LEFT JOIN Manufacturer as fac ON m.ID_Manufacturer = fac.ID_Manufacturer" +
                    "ORDER BY m.MedicineName";
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
                            Id_manu = Convert.ToInt32(reader[4])
                        });
                      
                    }
                }
                con.Close();

                return retVal;
            }
        }

        public void DodajZdravilo(Zdravilo zdr)
        {

            if (!AuthenticateUser())
                throw new FaultException("Napačno uporabniško ime ali geslo.");

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

        public void IzbrisiZdravilo(string Name, string NameLat)
        {

            if (!AuthenticateUser())
                throw new FaultException("Napačno uporabniško ime ali geslo.");

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                //popravi
                string sql = "DELETE FROM Medicine WHERE MedicineName = @param";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.Add(new SqlParameter("param1", Name));
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }

        public void PosodobiZdravilo(Zdravilo zdravilo, string id)
        {
            if (!AuthenticateUser())
                throw new FaultException("Napačno uporabniško ime ali geslo.");

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                //popravi
                string sql =
                    "UPDATE Persons set FirstName=@1, LastName=@2, Address=@3, City=@4 WHERE PersonID=@0";
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

        

        // dodaj izbris posodobitev manufac/address

        // add remove user

    }
}
