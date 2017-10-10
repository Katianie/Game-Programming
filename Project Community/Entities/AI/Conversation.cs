using System;
using System.Collections.Generic;
using System.Xml;

namespace Entities.AI
{
    /// <summary>
    /// Represents an in game conversation.
    /// </summary>
    /// <Owner>Justin Dale</Owner>
    public class Conversation
    {
        public String Question{get;set;}
        public List<ConversationResponse> responses{get;set;}
        private AIBase owner;
        public string next = string.Empty;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_owner"></param>
        public Conversation(AIBase _owner)
        {
            owner = _owner;
            responses = new List<ConversationResponse>();
        }

        /// <summary>
        /// Starting load method
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="_owner"></param>
        /// <returns></returns>
        public static Conversation loadConversation(String filename, AIBase _owner, String nextConvo)
        {
            Conversation newConvo = new Conversation(_owner) {next = nextConvo};


            XmlTextReader xmlReader = new XmlTextReader(filename);
           
            xmlReader.ReadToFollowing("Conversation");
            newConvo.Question = xmlReader.GetAttribute("value");
            xmlReader.ReadToDescendant("Response");
            do
            {
                String responseText = xmlReader.GetAttribute("value");

                String type = xmlReader.GetAttribute("type");
                if (type.Equals("Event"))
                {

                    xmlReader.ReadToDescendant("Events");
                    xmlReader.ReadToDescendant("Event");
                    Stack<Event> events = new Stack<Event>();
                    do
                    {
                        String eventName = xmlReader.GetAttribute("eventName");
                        String eventValue = xmlReader.GetAttribute("eventValue");
                        events.Push(new Event(eventName, eventValue));

                    } while (xmlReader.ReadToNextSibling("Event"));

                    ConversationResponse cr = new ConversationResponse(responseText, events, _owner);
                    newConvo.responses.Add(cr);
                    xmlReader.Read();
                    xmlReader.Read();
                }
                else if (type.Equals("Convo"))
                {
                    ConversationResponse cr = new ConversationResponse(responseText, loadConversation(_owner, xmlReader), _owner);
                    newConvo.responses.Add(cr);
                    xmlReader.Read();
                    xmlReader.Read();
                    xmlReader.Read();
                    xmlReader.Read();
                   
                }
                else
                    throw new Exception("Bad Convo");


            } while (xmlReader.ReadToNextSibling("Response"));
            xmlReader.Close();
            return newConvo;



        }

        /// <summary>
        /// Inner load method (Recursion and all that)
        /// </summary>
        /// <param name="_owner"></param>
        /// <param name="xmlReader"></param>
        /// <returns></returns>
        public static Conversation loadConversation(AIBase _owner, XmlReader xmlReader)
        {
            Conversation newConvo = new Conversation(_owner);

            
           
            xmlReader.ReadToFollowing("Conversation");
            newConvo.Question = xmlReader.GetAttribute("value");
            xmlReader.ReadToFollowing("Response");
            do
            {

                String type = xmlReader.GetAttribute("type");
                String responseText = xmlReader.GetAttribute("value");
            
                if (type.Equals("Event"))
                {
                    xmlReader.ReadToDescendant("Events");
                    xmlReader.ReadToDescendant("Event");
                    Stack<Event> events = new Stack<Event>();
                    do
                    {
                        String eventName = xmlReader.GetAttribute("eventName");
                        String eventValue = xmlReader.GetAttribute("eventValue");
                        events.Push(new Event(eventName, eventValue));

                    } while (xmlReader.ReadToNextSibling("Event"));

                    ConversationResponse cr = new ConversationResponse(responseText, events, _owner);
                    newConvo.responses.Add(cr);
                    xmlReader.Read();
                    xmlReader.Read();
                }
                else if (type.Equals("Convo"))
                {
                   // String outS = xmlReader.ReadInnerXml();
                    ConversationResponse cr = new ConversationResponse(responseText, loadConversation(_owner, xmlReader), _owner);
                    newConvo.responses.Add(cr);
                    xmlReader.Read();
                    xmlReader.Read();
                    xmlReader.Read();
                    xmlReader.Read();
                }
                else
                    throw new Exception("Bad Convo");


            } while (xmlReader.ReadToNextSibling("Response"));

            return newConvo;

        }


        /// <summary>
        /// Returns an array of the responses.
        /// </summary>
        /// <returns></returns>
        public String[] getResponseStringArray()
        {
            String[] strs = new String[responses.Count];
            int i = 0;
            foreach(ConversationResponse cr in responses)
            {
                strs[i] = responses[i].text;
                i++;
            }

            return strs;
        }
    }
}
