using System;
using System.Collections.Generic;

namespace Entities.AI
{
    /// <summary>
    /// Represents the response to something said by an AI.
    /// Can either lead to another convo or an event being passed along.
    /// </summary>
    /// <Owner>Justin Dale</Owner>
    public class ConversationResponse
    {
        public readonly String text;
        public readonly Conversation convo;
        public readonly Stack<Event> eventsToSend;
        public readonly AIBase ai;
        /// <summary>
        /// Next is convo constructor
        /// </summary>
        /// <param name="_text"></param>
        /// <param name="_conversation"></param>
        /// <param name="owner"></param>
        public ConversationResponse(String _text, Conversation _conversation, AIBase owner)
        {
            ai = owner;
            text = _text;
            convo = _conversation;
        }

        /// <summary>
        /// Next is event constructor
        /// </summary>
        /// <param name="_text"></param>
        /// <param name="_events"></param>
        /// <param name="owner"></param>
        public ConversationResponse(String _text, Stack<Event> _events, AIBase owner)
        {
            ai = owner;
            text = _text;
            eventsToSend = _events;
        }

    }
}
