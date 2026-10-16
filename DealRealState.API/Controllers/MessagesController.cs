using DataLayer.Models;
using BusinessLayer.Services;
using Microsoft.AspNetCore.Mvc;
using DealRealState.Models;

namespace DealRealState.API.Controllers
{
    [ApiController]
    [Route("api/messages")]
    public class MessagesController : ControllerBase
    {

        private readonly MessageService _messageService;


        public MessagesController(MessageService messageService)
        {
            _messageService = messageService;
        }



        // Get all messages for user

        [HttpGet("user/{userID}")]
        public IActionResult GetUserMessages(int userID)
        {
            if (userID <= 0)
                return BadRequest();


            var messages =
                _messageService.GetUserMessages(userID);


            return Ok(messages);
        }




        // Get conversation between two users

        [HttpGet("conversation/{user1ID}/{user2ID}")]
        public IActionResult GetConversation(
            int user1ID,
            int user2ID)
        {

            if (user1ID <= 0 || user2ID <= 0)
                return BadRequest();


            var messages =
                _messageService.GetConversation(
                    user1ID,
                    user2ID
                );


            return Ok(messages);
        }





        // Send message

        [HttpPost]
        public IActionResult SendMessage(Message message)
        {

            if (message == null)
                return BadRequest();



            var messageID =
                _messageService.SendMessage(message);



            if (messageID == 0)
                return BadRequest();



            return Ok(new
            {
                MessageID = messageID
            });

        }




        // Mark message as read

        [HttpPut("{messageID}/read")]
        public IActionResult MarkMessageAsRead(int messageID)
        {

            if (messageID <= 0)
                return BadRequest();



            var result =
                _messageService.MarkMessageAsRead(messageID);



            if (!result)
                return BadRequest();



            return Ok();

        }

    }
}