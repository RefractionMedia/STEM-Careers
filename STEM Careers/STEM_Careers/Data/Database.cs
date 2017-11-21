using STEM_Careers.Helpers;
using STEM_Careers.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace STEM_Careers.Data
{

    ///<summary>
    ///The database for all csv files fetched from the web
    /// </summary>
    public class Database
    {
        readonly SQLiteAsyncConnection database;
        readonly SQLiteConnection databaseDirect;

        private bool DegreeFinderInitialized;
        private bool JobListInitialized;
        private bool UniRankingInitialized;
        private bool PeopleTableInitialized;

        public bool IsInitializing { get; private set; }


        public Database(string dbPath)
        {
            database = new SQLiteAsyncConnection(dbPath);
            databaseDirect = new SQLiteConnection(dbPath);
            UniRankingInitialized = false;
            JobListInitialized = false;
            DegreeFinderInitialized = false;
            PeopleTableInitialized = false;
            IsInitializing = true;
            //if database exists we don't have to initialize it 
            try
            {
                //database.DropTableAsync<Degree>().Wait();
                //database.DropTableAsync<Job>().Wait();
                //database.DropTableAsync<University>().Wait();  //DEBUG ONLY
                //We query the db for an item of id 21, since both csv have at least 22 items, there is a problem only if the app didn't initialize them correctly 
                int countPeople = databaseDirect.Table<People>().Count();
                PeopleTableInitialized = true;
                databaseDirect.Table<Degree>().Where(d => d.ID == 21).First();
                databaseDirect.Table<University>().Where(d => d.ID == 21).First();
                databaseDirect.Table<Job>().Where(d => d.ID == 21).First();
                JobListInitialized = true;
                UniRankingInitialized = true;
                DegreeFinderInitialized = true;
                IsInitializing = false;
            }
            catch (Exception sqle)
            {
                var exception = sqle.ToString();
                IsInitializing = false;
                InitializeAsync();
            }
            //try
            //{
            //    databaseDirect.Table<Favorite>().Count();
            //}catch(Exception e)
            //{
            //    e.ToString();
            //    database.CreateTableAsync<Favorite>();
            //}

            MessagingCenter.Subscribe<PeopleHelper, People>(this, "AddPerson", async (helper, person) =>
            {
                try
                {
                    var test = await database.Table<People>().Where(p => p.ArticleID == person.ArticleID).FirstOrDefaultAsync();
                    if(test == null)
                    {
                        await database.InsertAsync(person);
                    }
                }catch(Exception e) {
                    e.ToString();
                }
            });
        }

        public async Task<int> GetHighestPeopleID()
        {
            People tmp = await database.Table<People>().OrderByDescending(p => p.ArticleID).FirstOrDefaultAsync();
            return tmp == null ? -1 : tmp.ArticleID;
        }

        public bool RetryInitAsync()
        {
            if (!IsInitialized() && !IsInitializing)
            {
                InitializeAsync();
                return true;
            }
            return false;
        }

        public bool IsInitialized()
        {
            return DegreeFinderInitialized && UniRankingInitialized && JobListInitialized;
        }

        private async Task InitializeAsync()
        {
            if (!IsInitializing)
            {
                IsInitializing = true;
            }
            else
            {
                return;
            }
            try
            {
                await Task.Delay(500);
                MessagingCenter.Send(this, "DatabaseInfo", "Initializing");
                //Cleanup
                await Task.WhenAll(database.DropTableAsync<Degree>(), database.DropTableAsync<Job>(), database.DropTableAsync<University>());
                MessagingCenter.Send(this, "DatabaseInfo", "Tables dropped");
                //Create new tables
                await Task.WhenAll(database.CreateTableAsync<Degree>(), database.CreateTableAsync<Job>(), database.CreateTableAsync<University>());
                MessagingCenter.Send(this, "DatabaseInfo", "Tables created");
                if (PeopleTableInitialized == false)
                {
                    try
                    {
                        PeopleTableInitialized = await await  database.CreateTableAsync<People>().ContinueWith(async (t) =>
                        {
                            PeopleHelper peopleHelper = new PeopleHelper();
                            await peopleHelper.FetchPeopleArticles();
                            return true;
                        });
                    }catch(Exception e)
                    {
                        e.ToString();
                    }
                }
                //Initialize
                CsvCenter CsvCenter = new CsvCenter();
                JobListInitialized = await CsvCenter.JobList(db: database).ContinueWith((t) =>
                {
                    //if (t.Result == true)
                    //    MessagingCenter.Send(this, "DatabaseInfo", "JobTask Done");
                    return t.Result;
                });
                DegreeFinderInitialized = await CsvCenter.DegreeFinderInit(db: database).ContinueWith((t) =>
                {
                    //if (t.Result == true)
                    //    MessagingCenter.Send(this, "DatabaseInfo", "DegreeTask Done");
                    return t.Result;
                });
                UniRankingInitialized = await CsvCenter.UniRankInit(db: database).ContinueWith((t) =>
                {
                    //if (t.Result == true)
                    //    MessagingCenter.Send(this, "DatabaseInfo", "UniTask Done");
                    return t.Result;
                });
                if (!App.HasInternetConnexion())
                {
                    IsInitializing = false;
                    MessagingCenter.Send(this, "DatabaseInfo", "Initialization error");
                }
            }
            catch (Exception e)
            {
                e.ToString();
            }
            finally
            {
                IsInitializing = false;
                if (IsInitialized())
                {
                    MessagingCenter.Send(this, "DatabaseInfo", "Initialized");
                }
                else
                {
                    MessagingCenter.Send(this, "DatabaseInfo", "Initialization error");
                }

            }

        }

        public Task<List<Degree>> GetDegreesAsync()
        {
            return database.Table<Degree>().ToListAsync();
        }

        public Task<Degree> GetSingleDegreeAsync(int id)
        {
            return database.Table<Degree>().Where(i => i.ID == id).FirstAsync();
        }

        public Task<List<Degree>> GetDegreesFieldXStateAsync(string field = "", string X = "", string state = "")
        {
            field = field.Equals("Any") ? "" : field;
            X = X.Equals("Any") ? "" : X;
            state = state.Equals("Any") ? "" : state;

            var query = database.Table<Degree>().Where(d => d.Field.Contains(field) && d.YourX.Contains(X) && d.State.Contains(state));
            var returnVal = query.ToListAsync();
            return returnVal;
        }

        public Task<List<Job>> GetJobsAsync(string field = "", string X = "")
        {
            field = field.Equals("Any") ? "" : field;
            X = X.Equals("Any") ? "" : X;
            var query = database.Table<Job>().Where(d => d.Field.Contains(field) && d.YourX.Contains(X));
            var jobsListed = query.ToListAsync();
            return jobsListed;
        }

        public async Task<Stack<List<string>>> GetPickerDataAsync()
        {
            List<Degree> databaseListed = await database.Table<Degree>().ToListAsync();

            List<string> States = new List<string>();
            List<string> Fields = new List<string>();
            List<string> YourX = new List<string>();

            foreach (var degree in databaseListed)
            {
                if (!States.Contains(degree.State))
                {
                    States.Add(degree.State);
                }
                if (!Fields.Contains(degree.Field))
                {
                    Fields.Add(degree.Field);
                }
                if (!YourX.Contains(degree.YourX))
                {
                    YourX.Add(degree.YourX);
                }
            }
            Stack<List<string>> result = new Stack<List<string>>();
            result.Push(States);
            result.Push(YourX);
            result.Push(Fields);

            return result;
        }

        internal async Task<int> GetPeopleCount()
        {
            return await database.Table<People>().CountAsync();
        }

        public async Task<Stack<List<string>>> GetSTEMPickerDataAsync()
        {
            List<Job> databaseListed = await database.Table<Job>().ToListAsync();

            List<string> Fields = new List<string>();
            List<string> YourX = new List<string>();

            foreach (var degree in databaseListed)
            {
                if (!Fields.Contains(degree.Field))
                {
                    Fields.Add(degree.Field);
                }
                if (!YourX.Contains(degree.YourX))
                {
                    YourX.Add(degree.YourX);
                }
            }
            Stack<List<string>> result = new Stack<List<string>>();
            result.Push(YourX);
            result.Push(Fields);

            return result;
        }

        internal async Task<List<People>> GetPeople(string field, string X)
        {
            return await database.Table<People>().Where(p => p.ProfileCategories.Contains(field) && p.ProfileCategories.Contains(X)).OrderByDescending(p=>p.ArticleID).ToListAsync();
        }

        public async Task<University> GetUniversityWithName(string name = "")
        {
            var listOfUnis = await database.Table<University>().ToListAsync();
            foreach (var uni in listOfUnis)
            {
                if (string.Equals(uni.Name, name, StringComparison.CurrentCultureIgnoreCase))
                {
                    return uni;
                }
            }
            return null;
        }

        //Not used but old habbits die hard
        public Task<int> SaveDegreeAsync(Degree degree)
        {
            if (degree.ID == 0)
            {
                return database.UpdateAsync(degree);
            }
            else
            {
                return database.InsertAsync(degree);
            }
        }
        public Task<int> DeleteDegreeAsync(Degree degree)
        {
            return database.DeleteAsync(degree);
        }
    }
}
