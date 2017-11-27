using STEM_Careers.Converters;
using STEM_Careers.Models;
using CsvHelper;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace STEM_Careers.Helpers
{
    class CsvCenter
    {

        static readonly string UniversityRankingCSVLink = "https://docs.google.com/spreadsheets/d/e/2PACX-1vQ6gv1dYQLy6RmQnC3WgQBRzmmJVgG65FeIVX65pA8gc1rN0bzmneNrv3UPlV1vtSsST2l0MDjIEnaC/pub?output=csv";
        static readonly string DegreeFinderCSVLink = "https://docs.google.com/spreadsheets/d/e/2PACX-1vR6F5zziuTvzOR2b6032jBCNGp0LTf_a1L2hMxi000afILLr90atnytTNhyRgAec2yzQ3ht9SLLUsJ3/pub?output=csv";
        static readonly string JobListCSVLink = "https://docs.google.com/spreadsheets/d/e/2PACX-1vSOkra4mxnp_E0RGSVbdf923UYLTqtbqnlv4Ob2kPP1jpJ9qn4fGWL4sy5NywRHcHoAZpVgYwOm0EyC/pub?output=csv";
        static readonly string JobsDescriptionsCSVLink = "https://docs.google.com/spreadsheets/d/e/2PACX-1vSKzU-TIFvaSEIjMQUky556BIMtNeJX53oQRgN8E8-aSIQr0Hnl6Qut0078xd7F29aAEQFqJZ-kJeTI/pub?output=csv";

        public async Task<bool> JobList(SQLiteAsyncConnection db)
        {
            List<Job> Jobs = new List<Job>();
            StreamReader reader = null;
            CsvReader csv = null;
            Uri uri = new Uri(JobListCSVLink);
            await App.webHelper.client.GetAsync(uri).ContinueWith(async (t) =>
            {
                t.Result.EnsureSuccessStatusCode();
                Stream stream = await t.Result.Content.ReadAsStreamAsync();
                reader = new StreamReader(stream);
                csv = new CsvReader(reader);
            });
            if (csv == null)
                return false;
            csv.Configuration.HasHeaderRecord = false; //so it doesn't skip first line
            Job job;
            string[] StemPlusXs = new string[8];
            string field = "";

            while (csv.Read())
            {
                //Xs
                if (csv.GetField<string>(0).Contains("IMPORTANT"))
                {
                    int counter = 0;
                    for (int roll = 1; roll < 17; roll += 2)
                    {
                        StemPlusXs[counter] = csv.GetField<string>(roll);
                        counter++;
                    }
                    csv.Read();
                    csv.Read();
                    //Skip the next if and go to next line
                    continue;
                }
                // we check the field is present and format it to be eye friendly (Science, Tech.., Engin.., Maths)
                if (csv.GetField<string>(0).Length > 3)
                {
                    field = "";
                    char[] fieldArray = csv.GetField<string>(0).ToLower().ToCharArray();
                    fieldArray[0] = char.ToUpper(fieldArray[0]);
                    foreach (char c in fieldArray)
                    {
                        field += c;
                    }
                }
                //Then for each pair of columns (job, pay) we create a job and add it to the list
                ClearTextFormater formater = new ClearTextFormater();
                for (int x = 1; x <= 16; x += 2)
                {
                    if (csv.GetField(x).Length > 2)
                    {
                        job = new Job()
                        {
                            Field = field,
                            YourX = StemPlusXs[(x - 1) / 2],
                            Name = csv.GetField<string>(x).Trim(),
                            MedianSalary = csv.GetField<string>(x + 1)
                        };
                        Jobs.Add(job);
                    }
                }
            }
            await db.InsertAllAsync(Jobs).ContinueWith(async (t) =>
            {
                bool success =  await JobDescription(db);
            });

            return true;
        }

        public async Task<bool> JobDescription(SQLiteAsyncConnection db)
        {
            StreamReader reader = null;
            CsvReader csv = null;
            Uri uri = new Uri(JobsDescriptionsCSVLink);
            await App.webHelper.client.GetAsync(uri).ContinueWith(async (t) =>
            {
                t.Result.EnsureSuccessStatusCode();
                Stream stream = await t.Result.Content.ReadAsStreamAsync();
                reader = new StreamReader(stream);
                csv = new CsvReader(reader);
            });
            if (csv == null)
                return false;
            csv.Configuration.HasHeaderRecord = true;
            while (csv.Read())
            {
                try
                {
                    string jobName = csv.GetField(0).Trim();
                    string jobDescription = string.IsNullOrWhiteSpace(csv.GetField(1))? "No description, yet" : csv.GetField(1);
                    List<Job> jobs = await db.Table<Job>().Where(j => j.Name.ToLower() == jobName.ToLower()).ToListAsync();
                    if (jobs != null)
                    {
                        foreach (Job job in jobs)
                        {
                            job.Description = jobDescription;
                        }
                    }
                    else
                    {
                        foreach (Job job in jobs)
                        {
                            job.Description = "Sorry, no description yet..";
                        }
                    }
                    await db.UpdateAllAsync(jobs);
                }
                catch (Exception e)
                {
                    e.ToString();
                }
            }
            return true;
        }

        public async Task<bool> UniRankInit(SQLiteAsyncConnection db)
        {
            List<University> Unis = new List<University>();
            StreamReader reader;
            CsvReader csv = null;
            await App.webHelper.client.GetAsync(new Uri(UniversityRankingCSVLink)).ContinueWith(async (t) =>
            {
                t.Result.EnsureSuccessStatusCode();
                Stream stream = await t.Result.Content.ReadAsStreamAsync();
                reader = new StreamReader(stream);
                csv = new CsvReader(reader);
            });
            if (csv == null)
                return false;
            csv.Configuration.HasHeaderRecord = true;
            University uni;
            while (csv.Read())
            {
                uni = new University
                {
                    Name = csv.GetField<string>(0).TrimEnd(' '),
                    Rank = csv.GetField<string>(1),
                    Description = csv.GetField<string>(2)
                };
                Unis.Add(uni);
            }
            var insertionReturn = db.InsertAllAsync(Unis);
            return true;
        }

        public async Task<bool> DegreeFinderInit(SQLiteAsyncConnection db)
        {
            List<Degree> Degrees = new List<Degree>();
            StreamReader reader;
            CsvReader csv = null;
            await App.webHelper.client.GetAsync(new Uri(DegreeFinderCSVLink)).ContinueWith(async (t) =>
            {
                t.Result.EnsureSuccessStatusCode();
                Stream stream = await t.Result.Content.ReadAsStreamAsync();
                reader = new StreamReader(stream);
                csv = new CsvReader(reader);
            });
            if (csv == null)
                return false;
            csv.Configuration.HasHeaderRecord = true;
            Degree degree;
            while (csv.Read())
            {
                degree = new Degree();
                int id = -1;
                if (!csv.TryGetField<int>(5, out id))
                {
                    var insertionReturnn = db.InsertAllAsync(Degrees);
                    return true;
                }
                degree.Name = csv.GetField<string>(0);
                degree.Field = csv.GetField<string>(2);
                degree.YourX = csv.GetField<string>(3);

                //The following is a beautification of the plain-nouppercase-naming to suit the other CSV, see ClearTextFormater class
                ClearTextFormater formater = new ClearTextFormater();
                string university = csv.GetField<string>(4);

                degree.University = formater.MakeClearer(university);
                degree.ID = id;
                degree.LinkToWebsite = csv.GetField<string>(6);


                //Making sure the state names are all uppercase (except New Zealand) and similar 
                string tmp = csv.GetField<string>(1);
                if (tmp.Contains("VIC, QLD"))
                {
                    tmp = "NSW/ACT";
                }
                else if (tmp == "New Zealand")
                { }
                else if (tmp.Contains("NSW"))
                {
                    tmp = "NSW/ACT";
                }
                else if (string.Equals(tmp, "Vic", StringComparison.OrdinalIgnoreCase))
                {
                    tmp = "VIC";
                }
                degree.State = tmp;
                Degrees.Add(degree);

            }
            var insertionReturn = db.InsertAllAsync(Degrees);
            return true;
        }
    }
}
