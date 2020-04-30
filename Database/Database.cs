using LiteDB;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Database
{
    public class Database
    {
        static Database()
        {
            BsonMapper.Global.RegisterType(
                serialize: pwd => new BsonValue(pwd.Encrypted),
                deserialize: str => new Password(str.AsString, true)
            );
        }

        protected string Filename { get; }

        public Database()
        {
            Filename = ConfigurationManager.AppSettings["DatbaseFilepath"].ToString();
            Filename = Path.Combine(System.Windows.Forms.Application.StartupPath, Filename);
            var directory = Path.GetDirectoryName(Filename);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        public void Delete(bool all)
        {
            string dbpath = Path.GetDirectoryName(Filename);
            try
            {
                foreach (string f in Directory.GetFiles(dbpath))
                {
                    if (!all)
                    {
                        if (f.ToLower() != Filename.ToLower())
                        {
                            File.Delete(f);
                        }
                    }
                    else
                    {
                        File.Delete(f);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        public void ResetDatabase()
        {
            Delete(true);
            OnReset();
        }

        protected virtual void OnReset() => ExecuteDb(db => {
            var groups = db.GetCollection<GroupDetails>();
            var servers = db.GetCollection<ServerDetails>();
            groups.EnsureIndex(nameof(GroupDetails.GroupName), true);
            groups.InsertBulk(new[] {
                    new GroupDetails(Groups.UncategorizedId,"Uncategorized"),
                    new GroupDetails("Application Servers"),
                    new GroupDetails("Web Servers")
            });

            return 1;
        });

        protected TResult ExecuteDb<TResult>(Func<LiteDatabase, TResult> execute)
        {
            TResult result = default;
            try
            {
                using (var context = new LiteDatabase(Filename))
                {
                    result = execute(context);
                }
            }
            catch (LiteException ex)
            {
                if (ex.Message.Contains("duplicate key"))
                {
                    MessageBox.Show("A group by that name is already exist. Please give a different name.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to save the group due to error.\r\n\r\nError Message: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return result;
        }
    }

    public class Database<TCollection> : Database
    {
        public IReadOnlyCollection<TCollection> Items => Execute(context => {
            var items = context.FindAll();
            return items.ToList().AsReadOnly();
        });

        protected TResult Execute<TResult>(Func<LiteCollection<TCollection>, TResult> execute) => ExecuteDb(db => execute(db.GetCollection<TCollection>()));

        protected void Execute(Action<LiteCollection<TCollection>> execute) => Execute(context => {
            execute(context);
            return 1;
        });
    }
}