using Microsoft.Data.SqlClient;
using System.Data;
using DataLayer.Connection;
using DataLayer.Models;
using DealRealState.Models;

namespace DealRealState.Data.DataAccess
{
    public class MessageData
    {


        // Send Message

        public static int SendMessage(Message message)
        {
            int messageID = 0;


            using (SqlConnection connection =
                new SqlConnection(DbConnection.ConnectionString))
            {

                SqlCommand command = new SqlCommand(
                    "sp_SendMessage",
                    connection
                );


                command.CommandType =
                    CommandType.StoredProcedure;



                command.Parameters.AddWithValue(
                    "@SenderID",
                    message.SenderID
                );


                command.Parameters.AddWithValue(
                    "@ReceiverID",
                    message.ReceiverID
                );


                command.Parameters.AddWithValue(
                    "@MessageText",
                    message.MessageText
                );


                connection.Open();



                messageID =
                    Convert.ToInt32(
                        command.ExecuteScalar()
                    );

            }


            return messageID;
        }




        // Get Conversation Between Two Users

        public static List<Message> GetConversation(
            int user1ID,
            int user2ID)
        {

            List<Message> messages =
                new List<Message>();


            using (SqlConnection connection =
                new SqlConnection(DbConnection.ConnectionString))
            {

                SqlCommand command = new SqlCommand(
                    "sp_GetConversation",
                    connection
                );


                command.CommandType =
                    CommandType.StoredProcedure;



                command.Parameters.AddWithValue(
                    "@User1ID",
                    user1ID
                );


                command.Parameters.AddWithValue(
                    "@User2ID",
                    user2ID
                );


                connection.Open();



                SqlDataReader reader =
                    command.ExecuteReader();



                while (reader.Read())
                {

                    Message message =
                        new Message();



                    message.MessageID =
                        Convert.ToInt32(reader["MessageID"]);



                    message.SenderID =
                        Convert.ToInt32(reader["SenderID"]);



                    message.ReceiverID =
                        Convert.ToInt32(reader["ReceiverID"]);



                    message.MessageText =
                        reader["MessageText"].ToString();



                    message.IsRead =
                        Convert.ToBoolean(reader["IsRead"]);



                    message.CreatedAt =
                        Convert.ToDateTime(reader["CreatedAt"]);



                    messages.Add(message);

                }

            }


            return messages;
        }





        // Get User Messages

        public static List<Message> GetUserMessages(
            int userID)
        {

            List<Message> messages =
                new List<Message>();


            using (SqlConnection connection =
                new SqlConnection(DbConnection.ConnectionString))
            {

                SqlCommand command = new SqlCommand(
                    "sp_GetUserMessages",
                    connection
                );


                command.CommandType =
                    CommandType.StoredProcedure;



                command.Parameters.AddWithValue(
                    "@UserID",
                    userID
                );


                connection.Open();



                SqlDataReader reader =
                    command.ExecuteReader();



                while (reader.Read())
                {

                    Message message =
                        new Message();



                    message.MessageID =
                        Convert.ToInt32(reader["MessageID"]);



                    message.SenderID =
                        Convert.ToInt32(reader["SenderID"]);



                    message.ReceiverID =
                        Convert.ToInt32(reader["ReceiverID"]);



                    message.MessageText =
                        reader["MessageText"].ToString();



                    message.IsRead =
                        Convert.ToBoolean(reader["IsRead"]);



                    message.CreatedAt =
                        Convert.ToDateTime(reader["CreatedAt"]);



                    messages.Add(message);

                }

            }


            return messages;
        }





        // Mark Message As Read

        public static bool MarkMessageAsRead(
            int messageID)
        {

            using (SqlConnection connection =
                new SqlConnection(DbConnection.ConnectionString))
            {

                SqlCommand command = new SqlCommand(
                    "sp_MarkMessageAsRead",
                    connection
                );


                command.CommandType =
                    CommandType.StoredProcedure;



                command.Parameters.AddWithValue(
                    "@MessageID",
                    messageID
                );



                connection.Open();



                return command.ExecuteNonQuery() > 0;

            }

        }

    }
}