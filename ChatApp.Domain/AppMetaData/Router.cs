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
            public const string PinOrUnpin = Prefix + "pinOrUnpin/" + SingleRoute;
            public const string MakeAsAdminOrUnadmin = Prefix + "makeAsAdminOrUnadmin/" + SingleRoute;
            public const string RemoveMemberFromGroup = Prefix + "removeMemberFromGroup/" + SingleRoute;
            public const string AddMembersToGroup = Prefix + "addMembersToGroup";
            public const string LeftGroup = Prefix + "leftGroup/" + SingleRoute;
            public const string DeleteGroup = Prefix + "deleteGroup/" + SingleRoute;
        }

        public static class Contact
        {
            public const string Prefix = Rule + "contacts/";
            public const string AddToContactsByPhoneNumber = Prefix + "addToContactsByPhoneNumber";
            public const string GetAll = Prefix + "getAll";
            public const string ViewContact = Prefix + "viewContact";
            public const string RemoveFromContacts = Prefix + "removeFromContacts/" + SingleRoute;
        }

        public static class Message
        {
            public const string Prefix = Rule + "messages/";
            public const string SendMessage = Prefix + "sendMessage";
            public const string DeleteMessage = Prefix + "deleteMessage/" + SingleRoute;
            public const string UpdateMessage = Prefix + "updateMessage";
        }

        public static class Chat
        {
            public const string Prefix = Rule + "chats/";
            public const string GetChatWithMessages = Prefix + "getChatWithMessages";
            public const string CreateGroup = Prefix + "createGroup";
            public const string UpdateGroup = Prefix + "updateGroup";
            public const string UpdateGroupImage = Prefix + "updateGroupImage";
        }

        public static class MessageStatus
        {
            public const string Prefix = Rule + "messageStatuses/";
            public const string GetMessageStatuses = Prefix + "getStatuses/" + SingleRoute;
        }

        public static class ConnectionTest
        {
            public const string Prefix = Rule + "connectionTest/";
            public const string Connect = Prefix + "connect";
            public const string Disconnect = Prefix + "disconnect";
        }

        public static class User
        {
            public const string Prefix = Rule + "users/";
            public const string GetUserStatus = Prefix + "getUserStatus/" + SingleRoute;
            public const string GetCurrentUser = Prefix + "getCurrentUser";
            public const string GetCurrentUserId = Prefix + "getCurrentUserId/";
            public const string UpdateProfile = Prefix + "updateProfile/";
            public const string UpdateProfileImage = Prefix + "updateProfileImage/";
            public const string DeleteProfileImage = Prefix + "deleteProfileImage/";
        }
    }
}
