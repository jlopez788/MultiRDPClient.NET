using System;

namespace Database
{
    public class Servers : Database<ServerDetails>
    {
        public bool Save(ServerDetails serverDetails)
        {
            if (serverDetails.GroupID == Guid.Empty)
            {
                serverDetails.GroupID = Groups.UncategorizedId;
            }
            return Execute(context => context.Upsert(serverDetails.Id, serverDetails));
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
        public string ServerName { set; get; }
        public string Server { set; get; }
        public int Port { set; get; }
        public string Username { set; get; }
        public Password Password { get; set; }
        public string Description { set; get; }
        public int ColorDepth { set; get; }
        public int DesktopWidth { set; get; }
        public int DesktopHeight { set; get; }
        public bool Fullscreen { set; get; }
        public Guid GroupID { set; get; }

        public ServerDetails() => Id = Guid.NewGuid();

        public override bool Equals(object obj) => obj is ServerDetails srv && (Server, Port, Username, Password.Encrypted) == (srv.Server, srv.Port, srv.Username, srv.Password.Encrypted);

        public override int GetHashCode() => (Server, Port, Username, Password.Encrypted).GetHashCode();

        public override string ToString() => $"@{Username} {Server}:{Port}";
    }
}