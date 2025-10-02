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
            public const string Logout = Prefix + "logout";
        }

        public static class ChatMember
        {
            public const string Prefix = Rule + "chatsMember/";
            public const string GetAll = Prefix + "getAll";
            public const string DeleteForMe = Prefix + "deleteForMe/" + SingleRoute;
        }

        public static class Contact
        {
            public const string Prefix = Rule + "contacts/";
            public const string AddToContactsByPhoneNumber = Prefix + "addToContactsByPhoneNumber";
            public const string GetAll = Prefix + "getAll";
            public const string ViewContact = Prefix + "viewContact";
        }

        public static class Message
        {
            public const string Prefix = Rule + "messages/";
            public const string SendMessageToContact = Prefix + "sendMessageToContact";
            public const string DeleteMessage = Prefix + "deleteMessage/" + SingleRoute;
        }

        public static class Chat
        {
            public const string Prefix = Rule + "chats/";
            public const string GetChatWithMessages = Prefix + "getChatWithMessages";
        }

        public static class User
        {
            public const string Prefix = Rule + "users/";
            public const string GetUserStatus = Prefix + "getUserStatus/" + SingleRoute;
            public const string GetCurrentUser = Prefix + "getCurrentUser";
            public const string GetCurrentUserId = Prefix + "getCurrentUserId/";
        }
    }
}
