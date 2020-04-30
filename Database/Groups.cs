using LiteDB;
using System;
using System.Linq;

namespace Database
{
    public class GroupDetails
    {
        public Guid Id { set; get; }
        public string GroupName { set; get; } = string.Empty;
        public int ServerCount { set; get; } = 0;

        public GroupDetails() => Id = Guid.NewGuid();

        public GroupDetails(string groupName)
        {
            Id = Guid.NewGuid();
            GroupName = groupName;
        }
    }

    public class Groups : Database<GroupDetails>
    {
        public static readonly Guid UncategorizedId = new Guid("10000000-0000-0000-0000-000000000012");

        public static readonly Lazy<GroupDetails> Uncategorized = new Lazy<GroupDetails>(() => new GroupDetails("Uncategorized") {
            Id = UncategorizedId
        });

        public void Save(bool isNew, GroupDetails group_details)
        {
            if (isNew)
            {
                Save(group_details);
            }
            else
            {
                Update(group_details);
            }
        }

        private void Save(GroupDetails groupDetails) => Execute(context => context.Insert(new BsonValue(groupDetails.Id), groupDetails));

        private void Update(GroupDetails groupDetails) => Execute(context => context.Upsert(new BsonValue(groupDetails.Id), groupDetails));

        public void DeleteByID(Guid id) => Execute(context => context.Delete(new BsonValue(id)));

        public string GetGroupNameByID(Guid id) => Execute(context => context.FindById(id)?.GroupName);

        public Guid GetIDByGroupName(string name) => Execute(context => context.FindOne(Query.EQ(nameof(GroupDetails.GroupName), name))?.Id ?? UncategorizedId);

        public void GetGroupsWithServerCount()
        {
            ExecuteDb(db => {
                var groupCollection = db.GetCollection<GroupDetails>();
                var servers = db.GetCollection<ServerDetails>().FindAll();
                var groups = groupCollection.FindAll().ToDictionary(kp => kp.Id, kp => {
                    kp.ServerCount = 0;
                    return kp;
                });
                foreach (var server in servers)
                {
                    groups[server.GroupID].ServerCount++;
                }
                groupCollection.Update(groups.Values);
                return 1;
            });
        }

        protected override void OnReset() => Execute(context => {
            context.InsertBulk(new[] {
                    Uncategorized.Value,
                    new GroupDetails("Application Servers"),
                    new GroupDetails("Web Servers")}
            );

            var items = context.FindAll().ToList();
        });
    }
}