using LiteDB;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace Database
{
    public class Database
    {
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

        protected virtual void OnReset() { }
    }

    public class Database<TCollection> : Database
    {
        public IReadOnlyCollection<TCollection> Items => Execute(context => {
            var items = context.FindAll();
            return items.ToList().AsReadOnly();
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
            catch (Exception)
            {
            }
            return result;
        }

        protected TResult Execute<TResult>(Func<LiteCollection<TCollection>, TResult> execute) => ExecuteDb(db => execute(db.GetCollection<TCollection>()));

        protected void Execute(Action<LiteCollection<TCollection>> execute) => Execute(context => {
            execute(context);
            return 1;
        });
    }
}