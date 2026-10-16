using Microsoft.Data.SqlClient;
using System.Data;
using DataLayer.Connection;
using DataLayer.Models;
using DealRealState.Models;


namespace DataLayer.DataAccess
{
    public class PropertyData
    {
        public static async Task<IEnumerable<Models.Property>> GetAllPropertiesAsync(string propertyType = null)
        {
            var properties = new List<Models.Property>();

            using (var connection = new SqlConnection(DbConnection.ConnectionString))
            {
                using (var command = new SqlCommand("sp_GetAllProperties", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@PropertyType", string.IsNullOrEmpty(propertyType) ? (object)DBNull.Value : propertyType);

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            properties.Add(new Models.Property
                            {
                                PropertyID = reader.GetInt32(reader.GetOrdinal("PropertyID")),
                                Title = reader.GetString(reader.GetOrdinal("Title")),
                                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? string.Empty : reader.GetString(reader.GetOrdinal("Description")),
                                Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                                PropertyType = reader.GetString(reader.GetOrdinal("PropertyType")),
                                City = reader.GetString(reader.GetOrdinal("City")),
                                ImageURL = reader.GetString(reader.GetOrdinal("ImageURL"))
                            });
                        }
                    }
                }
            }

            return properties;
        }
        public static List<Property> GetPropertiesBySeller(int sellerID)
        {
            List<Property> properties = new List<Property>();


            using (SqlConnection connection =
                new SqlConnection(DbConnection.ConnectionString))
            {

                SqlCommand command = new SqlCommand(
                    "sp_GetPropertiesBySeller",
                    connection
                );


                command.CommandType =
                    CommandType.StoredProcedure;


                command.Parameters.AddWithValue(
                    "@SellerID",
                    sellerID
                );


                connection.Open();


                SqlDataReader reader =
                    command.ExecuteReader();



                while (reader.Read())
                {
                    Property property = new Property();


                    property.PropertyID =
                        Convert.ToInt32(reader["PropertyID"]);


                    property.Title =
                        reader["Title"].ToString();


                    property.Price =
                        Convert.ToDecimal(reader["Price"]);


                    property.City =
                        reader["City"].ToString();


                  



                    properties.Add(property);
                }

            }


            return properties;
        }

        public static async Task<Property> GetPropertyByIDAsync(int propertyID)
        {
            Property property = null;

            using (SqlConnection connection = new SqlConnection(DbConnection.ConnectionString))
            {
                SqlCommand command = new SqlCommand("sp_GetPropertyByID", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@PropertyID", propertyID);

                await connection.OpenAsync();

                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    // 1. قراءة بيانات العقار الأساسية
                    if (await reader.ReadAsync())
                    {
                        property = new Property
                        {
                            PropertyID = Convert.ToInt32(reader["PropertyID"]),
                            Title = reader["Title"] != DBNull.Value ? reader["Title"].ToString() : string.Empty,
                            Description = reader["Description"] != DBNull.Value ? reader["Description"].ToString() : string.Empty,
                            Price = reader["Price"] != DBNull.Value ? Convert.ToDecimal(reader["Price"]) : 0m,
                            PropertyType = reader["PropertyType"] != DBNull.Value ? reader["PropertyType"].ToString() : string.Empty,
                            Location = reader["Location"] != DBNull.Value ? reader["Location"].ToString() : null,
                            City = reader["City"] != DBNull.Value ? reader["City"].ToString() : string.Empty,
                            Bedrooms = reader["Bedrooms"] != DBNull.Value ? Convert.ToInt32(reader["Bedrooms"]) : 0,
                            Bathrooms = reader["Bathrooms"] != DBNull.Value ? Convert.ToInt32(reader["Bathrooms"]) : 0,
                            AreaSQM = reader["AreaSQM"] != DBNull.Value ? Convert.ToDecimal(reader["AreaSQM"]) : 0m,
                            Floor = reader["Floor"] != DBNull.Value ? Convert.ToInt32(reader["Floor"]) : 0,
                            TotalFloors = reader["TotalFloors"] != DBNull.Value ? Convert.ToInt32(reader["TotalFloors"]) : 0,
                            YearBuilt = reader["YearBuilt"] != DBNull.Value ? Convert.ToInt32(reader["YearBuilt"]) : 0,
                            HasGarage = reader["HasGarage"] != DBNull.Value && Convert.ToBoolean(reader["HasGarage"]),
                            HasPool = reader["HasPool"] != DBNull.Value && Convert.ToBoolean(reader["HasPool"]),
                            HasGarden = reader["HasGarden"] != DBNull.Value && Convert.ToBoolean(reader["HasGarden"]),
                            IsFurnished = reader["IsFurnished"] != DBNull.Value && Convert.ToBoolean(reader["IsFurnished"]),
                            Panoramas = new List<PropertyPanorama>()
                        };
                    }

                    if (property == null) return null;

                    // 2. قراءة قائمة البانوراما (360 Views)
                    if (await reader.NextResultAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            PropertyPanorama panorama = new PropertyPanorama
                            {
                                PanoramaID = Convert.ToInt32(reader["PanoramaID"]),
                                PropertyID = Convert.ToInt32(reader["PropertyID"]),
                                RoomName = reader["RoomName"] != DBNull.Value ? reader["RoomName"].ToString() : string.Empty,
                                PanoramaURL = reader["PanoramaURL"] != DBNull.Value ? reader["PanoramaURL"].ToString() : string.Empty,
                                Description = reader["Description"] != DBNull.Value ? reader["Description"].ToString() : string.Empty,
                                OrderNumber = reader["OrderNumber"] != DBNull.Value ? Convert.ToInt32(reader["OrderNumber"]) : 0,
                                CreatedAt = reader["CreatedAt"] != DBNull.Value ? Convert.ToDateTime(reader["CreatedAt"]) : DateTime.MinValue
                            };

                            property.Panoramas.Add(panorama);
                        }
                    }
                }
            }

            return property;
        }
        public static int AddProperty(Property property)
        {
            int propertyID = 0;


            using (SqlConnection connection =
                new SqlConnection(DbConnection.ConnectionString))
            {

                SqlCommand command = new SqlCommand(
                    "sp_AddProperty",
                    connection
                );


                command.CommandType =
                    CommandType.StoredProcedure;



                command.Parameters.AddWithValue("@Title", property.Title);

                command.Parameters.AddWithValue("@Description", property.Description);

                command.Parameters.AddWithValue("@Price", property.Price);

                command.Parameters.AddWithValue("@PropertyType", property.PropertyType);

                command.Parameters.AddWithValue("@Location", property.Location);

                command.Parameters.AddWithValue("@City", property.City);

                command.Parameters.AddWithValue("@Bedrooms", property.Bedrooms);

                command.Parameters.AddWithValue("@Bathrooms", property.Bathrooms);

                command.Parameters.AddWithValue("@AreaSQM", property.AreaSQM);

                command.Parameters.AddWithValue("@Floor", property.Floor);

                command.Parameters.AddWithValue("@TotalFloors", property.TotalFloors);

                command.Parameters.AddWithValue("@YearBuilt", property.YearBuilt);

                command.Parameters.AddWithValue("@HasGarage", property.HasGarage);

                command.Parameters.AddWithValue("@HasPool", property.HasPool);

                command.Parameters.AddWithValue("@HasGarden", property.HasGarden);

                command.Parameters.AddWithValue("@IsFurnished", property.IsFurnished);




                connection.Open();


                propertyID =
                    Convert.ToInt32(command.ExecuteScalar());

            }


            return propertyID;
        }
        public static bool UpdateProperty(Property property)
        {
            using (SqlConnection connection =
                new SqlConnection(DbConnection.ConnectionString))
            {

                SqlCommand command = new SqlCommand(
                    "sp_UpdateProperty",
                    connection
                );


                command.CommandType =
                    CommandType.StoredProcedure;


                command.Parameters.AddWithValue("@PropertyID", property.PropertyID);

                command.Parameters.AddWithValue("@Title", property.Title);

                command.Parameters.AddWithValue("@Description", property.Description);

                command.Parameters.AddWithValue("@Price", property.Price);

                command.Parameters.AddWithValue("@City", property.City);


                connection.Open();


                return command.ExecuteNonQuery() > 0;
            }
        }

        public static bool DeleteProperty(int propertyID)
        {
            using (var connection = new SqlConnection(DbConnection.ConnectionString))
            {
                using (var command = new SqlCommand("sp_DeleteProperty", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@PropertyID", propertyID);

                    connection.Open();

                    return command.ExecuteNonQuery() > 0;
                }
            }
        }



    }
        }

