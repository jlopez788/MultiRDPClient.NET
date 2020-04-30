using System;

namespace Database
{
    public class Servers : Database<ServerDetails>
    {
        public void Save(ServerDetails serverDetails)
        {
            if (serverDetails.Password != string.Empty)
            {
                serverDetails.Password = RijndaelSettings.Encrypt(serverDetails.Password);
            }

            if (serverDetails.GroupID == Guid.Empty)
            {
                serverDetails.GroupID = Groups.UncategorizedId;
            }
            Execute(context => context.Upsert(serverDetails.Id, serverDetails));
        }

        public void UpdateGroupIdByID(Guid id, Guid newGroupID)
        {
            Execute(context => {
                var server = context.FindById(id);
                server.GroupID = newGroupID;
                context.Update(id, server);
            });
        }

        public void DeleteByID(Guid id) => Execute(context => context.Delete(id));
    }

    public class ServerDetails
    {
        public Guid Id { set; get; }

        public string ServerName { set; get; } = string.Empty;

        public string Server { set; get; } = string.Empty;

        public string Domain { set; get; } = string.Empty;

        public int Port { set; get; } = 0;

        public string Username { set; get; } = string.Empty;

        public string Password { get; set; }

        public string Description { set; get; } = string.Empty;

        public int ColorDepth { set; get; } = 0;

        public int DesktopWidth { set; get; } = 0;

        public int DesktopHeight { set; get; } = 0;

        public bool Fullscreen { set; get; } = false;

        public Guid GroupID { set; get; }

        public ServerDetails() => Id = Guid.NewGuid();
    }
}