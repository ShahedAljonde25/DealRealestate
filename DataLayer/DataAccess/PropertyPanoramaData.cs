using Microsoft.Data.SqlClient;
using System.Data;
using DataLayer.Connection;
using DealRealState.Models;


namespace DealRealState.Data.DataAccess
{
    public class PropertyPanoramaData
    {


        // Get Panoramas By Property

        public static List<Models.PropertyPanorama> GetPanoramasByProperty(int propertyID)
        {
            List<PropertyPanorama> panoramas =
                new List<PropertyPanorama>();


            using (SqlConnection connection =
                new SqlConnection(DbConnection.ConnectionString))
            {

                SqlCommand command = new SqlCommand(
                    "sp_GetPanoramasByProperty",
                    connection
                );


                command.CommandType =
                    CommandType.StoredProcedure;


                command.Parameters.AddWithValue(
                    "@PropertyID",
                    propertyID
                );


                connection.Open();


                SqlDataReader reader =
                    command.ExecuteReader();



                while (reader.Read())
                {

                    PropertyPanorama panorama =
                        new PropertyPanorama();



                    panorama.PanoramaID =
                        Convert.ToInt32(reader["PanoramaID"]);



                    panorama.PropertyID =
                        Convert.ToInt32(reader["PropertyID"]);



                    panorama.RoomName =
                        reader["RoomName"].ToString();



                    panorama.PanoramaURL =
                        reader["PanoramaURL"].ToString();



                    panorama.Description =
                        reader["Description"].ToString();



                    panorama.OrderNumber =
                        Convert.ToInt32(reader["OrderNumber"]);



                    panorama.CreatedAt =
                        Convert.ToDateTime(reader["CreatedAt"]);



                    panoramas.Add(panorama);

                }

            }


            return panoramas;
        }




        // Add Panorama

        public static int AddPropertyPanorama(
            PropertyPanorama panorama)
        {

            int panoramaID = 0;


            using (SqlConnection connection =
                new SqlConnection(DbConnection.ConnectionString))
            {

                SqlCommand command = new SqlCommand(
                    "sp_AddPropertyPanorama",
                    connection
                );


                command.CommandType =
                    CommandType.StoredProcedure;



                command.Parameters.AddWithValue(
                    "@PropertyID",
                    panorama.PropertyID
                );


                command.Parameters.AddWithValue(
                    "@RoomName",
                    panorama.RoomName
                );


                command.Parameters.AddWithValue(
                    "@PanoramaURL",
                    panorama.PanoramaURL
                );


                command.Parameters.AddWithValue(
                    "@Description",
                    panorama.Description
                );


                command.Parameters.AddWithValue(
                    "@OrderNumber",
                    panorama.OrderNumber
                );



                connection.Open();



                panoramaID =
                    Convert.ToInt32(
                        command.ExecuteScalar()
                    );

            }


            return panoramaID;
        }



        public static async Task<bool> AddPanoramaToPropertyAsync(int propertyId, string fileUrl)
        {
            string connectionString = DbConnection.ConnectionString; // ضع اتصال قاعدة البيانات الخاص بك

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // استخدام SqlCommand مع تحديد نوع الأمر بأنه StoredProcedure
                using (SqlCommand command = new SqlCommand("sp_addpanoramaimage", connection)) // أو اسم الـ SP الخاص بك
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // تمرير البارامترات المطلوبة للـ Stored Procedure
                    command.Parameters.AddWithValue("@PropertyID", propertyId);
                    command.Parameters.AddWithValue("@PanoramaURL", fileUrl); // تأكد أن اسم البارامتر مطابق لما في الـ SQL

                    try
                    {
                        await connection.OpenAsync();

                        // استخدمنا ExecuteScalarAsync بدلاً من ExecuteNonQueryAsync 
                        // لكي نقرأ النتيجة القادمة من جملة SELECT @@ROWCOUNT
                        object result = await command.ExecuteScalarAsync();

                        // تحويل النتيجة إلى رقم والتحقق من أنها أكبر من 0
                        if (result != null && int.TryParse(result.ToString(), out int rowsAffected))
                        {
                            return rowsAffected > 0;
                        }

                        return false;
                    }
                    catch (Exception ex)
                    {
                        // رمي الخطأ ليظهر في الـ API في حال كان العقار غير موجود
                        throw new Exception($"Database Error: {ex.Message}");
                    }
                }

                // Delete Panorama


            }
        }
    }
}