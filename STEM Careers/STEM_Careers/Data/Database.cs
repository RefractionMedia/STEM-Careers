using STEM_Careers.Helpers;
using STEM_Careers.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Xamarin.Forms;
using System.Collections.ObjectModel;

namespace STEM_Careers.Data
{

    ///<summary>
    ///The database for all files fetched from the web (degrees and jobs)
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
                //For Debugging
                //database.DropTableAsync<Degree>().Wait();
                //database.DropTableAsync<Job>().Wait();
                //database.DropTableAsync<University>().Wait();
                int countPeople = databaseDirect.Table<People>().Count();
                PeopleTableInitialized = true;
                databaseDirect.Table<Degree>().Where(d => d.ID == 21).First();
                DegreeFinderInitialized = true;
                databaseDirect.Table<University>().Where(d => d.ID == 21).First();
                UniRankingInitialized = true;
                databaseDirect.Table<Job>().Where(d => d.ID == 21).First();
                JobListInitialized = true;
                IsInitializing = false;
            }
            catch (Exception sqle)
            {
                var exception = sqle.ToString();
                IsInitializing = false;
                InitializeAsync();
            }

            //Suscribe to an add person event, work is done at startup and everytime a people page is opened
            MessagingCenter.Subscribe<PeopleHelper, People>(this, "AddPerson", async (helper, person) =>
            {
                try
                {
                    var test = await database.Table<People>().Where(p => p.ArticleID == person.ArticleID).FirstOrDefaultAsync();
                    if (test == null)
                    {
                        await database.InsertAsync(person);
                    }
                }
                catch (Exception e)
                {
                    e.ToString();
                }
            });
        }

        #region Init Methods
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
                if (!App.HasInternetConnexion())
                {
                    IsInitializing = false;
                    MessagingCenter.Send(this, "DatabaseInfo", "Initialization error");
                }
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
                        PeopleTableInitialized = await await database.CreateTableAsync<People>().ContinueWith(async (t) =>
                        {
                            PeopleHelper peopleHelper = new PeopleHelper();
                            return await peopleHelper.FetchPeopleArticles().ContinueWith((task) =>
                            {
                                if (!t.IsFaulted)
                                    return true;
                                return false;
                            });
                        });
                    }
                    catch (Exception e)
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
        #endregion

        #region Degree Methods
        internal async Task UpdateDegreeAsync(Degree degree)
        {
            Degree old = await database.Table<Degree>().Where(d => d.ID == degree.ID).FirstOrDefaultAsync();
            if (old != null)
            {
                await database.UpdateAsync(degree);
            }
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

        /// <summary>
        /// Gets the picker data for degrees only
        /// </summary>
        /// <returns></returns>
        public async Task<Stack<List<string>>> GetPickerDataAsync()
        {
            List<Degree> databaseListed = await database.Table<Degree>().ToListAsync();

            HashSet<string> States = new HashSet<string>();
            HashSet<string> Fields = new HashSet<string>();
            HashSet<string> YourX = new HashSet<string>();

            foreach (var degree in databaseListed)
            {
                if (!States.Contains(degree.State))
                {
                    States.Add(degree.State);
                }
                foreach (string str in degree.Field.Split(','))
                {
                    if (!Fields.Contains(str.Trim()))
                        Fields.Add(str.Trim());
                }
                foreach (string str in degree.YourX.Split(','))
                {
                    if (!YourX.Contains(str.Trim()))
                        YourX.Add(str.Trim());
                }
            }
            Stack<List<string>> result = new Stack<List<string>>();
            result.Push(States.ToList());
            result.Push(YourX.ToList());
            result.Push(Fields.ToList());
            return result;
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

        #endregion


        #region Job Methods
        internal async Task UpdateJobAsync(Job job)
        {
            try
            {
                Job old = await database.Table<Job>().Where(d => d.ID == job.ID).FirstOrDefaultAsync();
                if (old != null)
                {
                    await database.UpdateAsync(job);
                }
            }
            catch(Exception e)
            {
                e.ToString();
            }
            
        }
        public Task<List<Job>> GetJobsAsync(string field = "", string X = "")
        {
            field = field.Equals("Any") ? "" : field;
            X = X.Equals("Any") ? "" : X;
            var query = database.Table<Job>().Where(d => d.Field.Contains(field) && d.YourX.Contains(X));
            var jobsListed = query.ToListAsync();
            return jobsListed;
        }

        /// <summary>
        /// This is used to build the pickers' data for people and jobs, not degrees
        /// </summary>
        /// <returns></returns>
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
        #endregion

        internal async Task UpdatePersonAsync(People person)
        {
            People old = await database.Table<People>().Where(d => d.ArticleID == person.ArticleID).FirstOrDefaultAsync();
            if (old != null)
            {
                await database.UpdateAsync(person);
            }
        }

        public async Task<int> GetHighestPeopleID()
        {
            People tmp = await database.Table<People>().OrderByDescending(p => p.ArticleID).FirstOrDefaultAsync();
            return tmp == null ? -1 : tmp.ArticleID;
        }



        internal async Task<int> GetPeopleCount()
        {
            return await database.Table<People>().CountAsync();
        }

        internal async Task<List<People>> GetPeople(string field, string X)
        {
            return await database.Table<People>().Where(p => p.ProfileCategories.Contains(field) && p.ProfileCategories.Contains(X)).OrderByDescending(p => p.ArticleID).ToListAsync();
        }

        internal async Task<ObservableRangeCollection<object>> GetFavorites()
        {
            ObservableRangeCollection<object> faves = new ObservableRangeCollection<object>();
            faves.AddRange(await database.Table<People>().Where(fave => fave.IsFavorite == true).ToListAsync());
            faves.AddRange(await database.Table<Job>().Where(fave => fave.IsFavorite == true).ToListAsync());
            faves.AddRange(await database.Table<Degree>().Where(fave => fave.IsFavorite == true).ToListAsync());
            return faves;
        }

    }
}
