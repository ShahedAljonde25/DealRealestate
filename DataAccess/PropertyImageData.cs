using Microsoft.Data.SqlClient;
using System.Data;
using DataLayer.Connection;
using DataLayer.Models;
using DealRealState.Models;


namespace DealRealState.Data.DataAccess
{
    public class PropertyImageData
    {

        // Get Images By Property

        public static List<PropertyImage> GetImagesByProperty(int propertyID)
        {
            List<PropertyImage> images = new List<Models.PropertyImage>();


            using (SqlConnection connection =
                new SqlConnection(DbConnection.ConnectionString))
            {

                SqlCommand command = new SqlCommand(
                    "sp_GetImagesByProperty",
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
                    PropertyImage image = new PropertyImage();


                    image.ImageID =
                        Convert.ToInt32(reader["ImageID"]);


                    image.PropertyID =
                        Convert.ToInt32(reader["PropertyID"]);


                    image.ImageURL =
                        reader["ImageURL"].ToString();


                    image.Description =
                        reader["Description"].ToString();


                    image.OrderNumber =
                        Convert.ToInt32(reader["OrderNumber"]);


                    image.CreatedAt =
                        Convert.ToDateTime(reader["CreatedAt"]);



                    images.Add(image);
                }

            }


            return images;
        }



        // Add Image

        public static int AddPropertyImage(PropertyImage image)
        {
            int imageID = 0;


            using (SqlConnection connection =
                new SqlConnection(DbConnection.ConnectionString))
            {

                SqlCommand command = new SqlCommand(
                    "sp_AddPropertyImage",
                    connection
                );


                command.CommandType =
                    CommandType.StoredProcedure;


                command.Parameters.AddWithValue(
                    "@PropertyID",
                    image.PropertyID
                );


                command.Parameters.AddWithValue(
                    "@ImageURL",
                    image.ImageURL
                );


                command.Parameters.AddWithValue(
                    "@Description",
                    image.Description
                );


                command.Parameters.AddWithValue(
                    "@OrderNumber",
                    image.OrderNumber
                );


                connection.Open();


                imageID =
                    Convert.ToInt32(
                        command.ExecuteScalar()
                    );

            }


            return imageID;
        }



        // Delete Image

        public static bool DeletePropertyImage(int imageID)
        {

            using (SqlConnection connection =
                new SqlConnection(DbConnection.ConnectionString))
            {

                SqlCommand command = new SqlCommand(
                    "sp_DeletePropertyImage",
                    connection
                );


                command.CommandType =
                    CommandType.StoredProcedure;


                command.Parameters.AddWithValue(
                    "@ImageID",
                    imageID
                );


                connection.Open();


                return command.ExecuteNonQuery() > 0;

            }

        }


      
        }

    }
