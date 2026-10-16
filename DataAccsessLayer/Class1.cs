using System.Data;
using System;
using Microsoft.Data.SqlClient;
using System.Data.SqlClient;
namespace DataAccsessLayer
{
    public class DataAccsessLayer
    {
        static string _connectionString = "Server=localhost;Database=DVDL;User Id=sa;Password=123456;Encrypt=False;TrustServerCertificate=True;Connection Timeout=30;";


    }

    public class DTOPeople
    {
        public static bool UpdatePerson(SP person)
        {
            using (SqlConnection connection = new SqlConnection(connectionString: DataAccsessLayer._connectionString))
            using (SqlCommand command = new SqlCommand("SP_Update_People", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@NationalNo", person.NationalNo);
                command.Parameters.AddWithValue("@FirstName", person.FirstName);
                command.Parameters.AddWithValue("@SecondName", person.SecondName);
                command.Parameters.AddWithValue("@ThirdName", person.ThirdName);
                command.Parameters.AddWithValue("@LastName", person.LastName);
                command.Parameters.AddWithValue("@DateOfBirth", person.DateOfBirth);
                command.Parameters.AddWithValue("@Gendor", person.Gendor);
                command.Parameters.AddWithValue("@Address", person.Address);
                command.Parameters.AddWithValue("@Phone", person.Phone);
                command.Parameters.AddWithValue("@Email", person.Email);
                command.Parameters.AddWithValue("@NationalityCountryID", person.NationalityCountryID);
                command.Parameters.AddWithValue("@ImagePath",
                    string.IsNullOrEmpty(person.ImagePath) ? DBNull.Value : person.ImagePath);

                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
        }
    }
}
