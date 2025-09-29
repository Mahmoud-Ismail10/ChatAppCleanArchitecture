namespace ChatApp.Domain.AppMetaData
{
    public class Router
    {
        public const string SingleRoute = "{id}";

        public const string Root = "api";
        public const string Version = "v1";
        public const string Rule = Root + "/" + Version + "/";

        public static class Authentication
        {
            public const string Prefix = Rule + "authenticate/";
            public const string SendOtp = Prefix + "sendOtp";
            public const string VerifyOtp = Prefix + "verifyOtp";
            public const string Register = Prefix + "register";
            public const string CreateSession = Prefix + "createSession";
        }

        public static class ChatMember
        {
            public const string Prefix = Rule + "chatsMember/";
            public const string GetAll = Prefix + "getAll";
        }

        public static class Contact
        {
            public const string Prefix = Rule + "contacts/";
            public const string AddToContactsByPhoneNumber = Prefix + "addToContactsByPhoneNumber";
            public const string GetAll = Prefix + "getAll";
        }

        public static class Message
        {
            public const string Prefix = Rule + "messages/";
            public const string SendMessageToContact = Prefix + "sendMessageToContact";
        }
    }
}
