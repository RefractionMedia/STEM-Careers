using Newtonsoft.Json;
using STEM_Careers.MobileAppService.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace STEM_Careers.MobileAppService.Helpers
{
    public class CwSTEMapi
    {
        private readonly string _base = "https://careerswithstem.com/wp-json/wp/v2/";

        private static readonly CwSTEMapi _instance = new CwSTEMapi();

        public static CwSTEMapi Instance { get { return _instance; } }

        private HttpClient client;
        public List<Category> Categories { get; private set; }

        public int quizzesID = 547;

        public CwSTEMapi()
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            Categories = new List<Category>();

        }

        internal async Task GetCategories()
        {
            Uri uri = new Uri(_base + "categories?per_page=100");
            await client.GetAsync(uri).ContinueWith((t) =>
            {
                //if (t.Result.StatusCode == System.Net.HttpStatusCode.NotFound)
                //{
                //    MessagingCenter.Send<CwSTEMapi>(this, "Categories retrieved");
                //}

                t.Result.EnsureSuccessStatusCode();
                t.Result.Content.ReadAsStringAsync().ContinueWith((readtask) =>
                {
                    try
                    {
                        JsonTextReader jsonTextReader = new JsonTextReader(new StringReader(readtask.Result));
                        JsonSerializer serializer = new JsonSerializer();
                        Categories = serializer.Deserialize<List<Category>>(jsonTextReader);
                        //quizzesID = Categories.Find(x => x.Name.Equals("Quizzes", StringComparison.OrdinalIgnoreCase)).ID;
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.ToString());
                    }
                });
            });
        }

        internal async Task<List<Story>> GetStories(Category category)
        {
            string cat = category.ID == 0 ? "" : "categories=" + category.ID.ToString() + "&";
            Uri uri = new Uri(_base + "posts?" + cat + "per_page=100");
            List<Story> Stories = new List<Story>();
            //List<Story> StoriesWithoutQuizzes = new List<Story>();
            return await await client.GetAsync(uri).ContinueWith(async (t) =>
             {
                 if (t.Result.StatusCode == System.Net.HttpStatusCode.NotFound)
                 {
                     return Stories;
                 }

                 t.Result.EnsureSuccessStatusCode();
                 return await t.Result.Content.ReadAsStringAsync().ContinueWith((readtask) =>
                 {
                     try
                     {
                         string decoded = WebUtility.HtmlDecode(readtask.Result).ToString();
                         JsonTextReader jsonTextReader = new JsonTextReader(new StringReader(decoded));
                         JsonSerializer serializer = new JsonSerializer();
                         Stories = serializer.Deserialize<List<Story>>(jsonTextReader);
                     }
                     catch (Exception e)
                     {
                         Debug.WriteLine(e.ToString());
                     }
                     return Stories;
                 });
             });
        }
    }
}
