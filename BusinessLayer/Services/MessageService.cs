using DealRealState.Data.DataAccess;
using DealRealState.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public class MessageService
    {

            public List<Message> GetUserMessages(int userID)
            {
                if (userID <= 0)
                    return new List<Message>();

                return MessageData.GetUserMessages(userID);
            }


            public List<Message> GetConversation(int user1ID, int user2ID)
            {
                if (user1ID <= 0 || user2ID <= 0)
                    return new List<Message>();

                return MessageData.GetConversation(user1ID, user2ID);
            }


            public int SendMessage(Message message)
            {
                if (message == null)
                    return 0;

                if (message.SenderID <= 0)
                    return 0;

                if (message.ReceiverID <= 0)
                    return 0;

                if (string.IsNullOrWhiteSpace(message.MessageText))
                    return 0;

                return MessageData.SendMessage(message);
            }


            public bool MarkMessageAsRead(int messageID)
            {
                if (messageID <= 0)
                    return false;

                return MessageData.MarkMessageAsRead(messageID);
            }
        }
    }

