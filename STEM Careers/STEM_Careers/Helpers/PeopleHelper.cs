using STEM_Careers.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;
using CsvHelper;
using System.IO;
using STEM_Careers.Data;
using System.Net;
using Newtonsoft.Json;

namespace STEM_Careers.Helpers
{
    public class PeopleHelper
    {
        public HttpClient client;
        List<People> People = new List<People>();
        public PeopleHelper()
        {
            client = new HttpClient();
        }

        /// <summary>
        /// Queries the Database for stored People
        /// </summary>
        /// <param name="field"></param>
        /// <param name="X"></param>
        /// <returns></returns>
        public async Task<List<People>> GetPeople(string field = "", string X = "")
        {
            return await App.Database.GetPeople(ConvertCategory(field), ConvertCategory(X));
        }


        /// <summary>
        /// Gets the additional data of a person, then adds sends an "AddPerson" message
        /// The "AddPerson" message is caught by the Database and added to it, also by the PeoplePage and added to the list currently on display
        /// </summary>
        /// <see cref="PeopleFromHtml(string, string, string)"/>
        /// <param name="person"></param>
        /// <returns></returns>
        public async Task GetPersonDetails(People person)
        {
            if (person == null)
            {
                throw new ArgumentNullException(nameof(person));
            }

            string html = await await client.GetAsync(person.Href).ContinueWith(async (t) =>
            {
                t.Result.EnsureSuccessStatusCode();
                return await t.Result.Content.ReadAsStringAsync().ContinueWith((readtask) =>
                {
                    return readtask.Result;
                });
            });

            var page = new HtmlDocument();
            page.LoadHtml(html);
            var article = page.DocumentNode.Descendants().FirstOrDefault(x => x.Name == "article");
            var articleBody = article.Descendants().FirstOrDefault(x => x.Attributes.Contains("itemprop") && x.Attributes["itemprop"].Value == "articleBody");
            string str = "";
            foreach (var p in articleBody.Descendants())
            {
                if (p.Name == "p")
                {
                    str += p.InnerText + "\n\n";
                }
            }
            person.Content = WebUtility.HtmlDecode(str);
            MessagingCenter.Send(this, "AddPerson", person);
            return;
        }

        /// <summary>
        /// Gets basic People data (href, title, name..) from an html string, then calls GetPersonDetails(..) for detailed info
        /// </summary>
        /// <param name="html">the Html page as a string (has to be a list of profiles page)</param>
        /// <param name="X"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        private async Task PeopleFromHtml(string html, string X = "", string field = "")
        {
            var page = new HtmlDocument();
            page.LoadHtml(html);

            var firstArticle = page.DocumentNode.Descendants().FirstOrDefault(x => x.Name == "article");
            var articles = firstArticle.ParentNode.Descendants();
            foreach (var article in articles)
            {
                int ID = 0;
                try
                {
                    ID = Int32.Parse(article.Id.Remove(0, 5));
                }
                catch (Exception e)
                {
                    e.ToString();
                }
                //if (await App.Database.GetHighestPeopleID() > ID)
                //{
                //    return;
                //}
                //Non articles are skipped
                if (!article.Name.Equals("article"))
                    continue;
                //Category is found first, so if it doesn't correspond to researched, we skip it
                List<string> categories = new List<string>();
                if (article.Attributes.Contains("class"))
                {
                    string allCats = article.Attributes["class"].Value;
                    foreach (var classAttr in allCats.Split(' '))
                    {
                        if (classAttr.Contains("profile_cat-"))
                        {
                            categories.Add(classAttr.Remove(0, 12));
                        }
                        else if (classAttr.Contains("category-"))
                        {
                            categories.Add(classAttr.Remove(0, 9));
                        }
                    }
                }

                //Making sure we get data only for the required pickers selected
                if (string.IsNullOrWhiteSpace(field) && !string.IsNullOrWhiteSpace(X))
                {
                    if (!CheckInList(X, categories))
                        continue;
                }
                else if (string.IsNullOrWhiteSpace(X) && !string.IsNullOrWhiteSpace(field))
                {
                    if (!CheckInList(field, categories))
                        continue;
                }
                else if (!string.IsNullOrWhiteSpace(X) && !string.IsNullOrWhiteSpace(field))
                {
                    if (!CheckInList(X, categories) || !CheckInList(field, categories))
                        continue;
                }

                //Getting the data
                var aTag = article.Descendants().FirstOrDefault(x => x.Name == "a" && x.Attributes.Contains("href"));
                var imgTag = aTag.Descendants().FirstOrDefault(x => x.Name == "img" && x.Attributes.Contains("src"));
                var section = article.Descendants().FirstOrDefault(x => x.Name == "section");
                var sectionH3 = section.Descendants().FirstOrDefault(x => x.Name == "h3");
                var sectionH4 = section.Descendants().FirstOrDefault(x => x.Name == "h4");
                var tmp = sectionH4.NextSibling.InnerText;
                tmp = tmp.Trim();
                //creating the person and sending it to the PeoplePageViewodel through messaging center
                People person = new People
                {
                    ArticleID = ID,
                    IsFavorite = false,
                    Title = aTag.Attributes["title"].Value,
                    Href = aTag.Attributes["href"].Value,
                    ImgReference = imgTag.Attributes["src"].Value,
                    Name = sectionH3.Descendants().FirstOrDefault(x => x.Name == "a").InnerHtml,
                    Description = tmp,
                    ProfileCategories = JsonConvert.SerializeObject(categories)
                };
                await GetPersonDetails(person);
            }
        }

        /// <summary>
        /// Gets all Poeple articles on "careerswithstem.com/profiles/page/*" untill 404 code
        /// </summary>
        /// <param name="field"></param>
        /// <param name="X"></param>
        /// <returns></returns>
        public async Task<bool> FetchPeopleArticles(string field = "", string X = "")
        {
            try
            {
                bool notFound = false;
                int i = 0;
                while (notFound == false)
                {
                    i++;
                    var uri = new Uri("https://careerswithstem.com/profiles/page/" + i);
                    await client.GetAsync(uri).ContinueWith((t) =>
                    {
                        if (t.Result.StatusCode == System.Net.HttpStatusCode.NotFound)
                        {
                            if (notFound == true)
                            {
                                return;
                            }
                            notFound = true;
                            return;
                        }

                        t.Result.EnsureSuccessStatusCode();
                        t.Result.Content.ReadAsStringAsync().ContinueWith(async (readtask) =>
                        {
                            await PeopleFromHtml(readtask.Result, X, field);
                        });
                    });
                }
            }
            catch (HttpRequestException httpEx)
            {
                httpEx.ToString();
            }
            catch (Exception e)
            {
                e.ToString();
            }
            return true;
        }


        #region Updating helpers

        public async Task UpdatePeople(string field = "", string X = "")
        {
            if (await PeopleUpToDate())
                return;
            await FetchPeopleArticles(field, X);
            return;
        }

        /// <summary>
        /// Checks if the latest post on website is the same as the one in the database
        /// </summary>
        /// <returns></returns>
        public async Task<bool> PeopleUpToDate()
        {
            //check if the newest article on website is the same as the newest one in the database
            int ID = 0;
            var uri = new Uri("https://careerswithstem.com/profiles/");
            return await await await client.GetAsync(uri).ContinueWith(async (t) =>
            {
                t.Result.EnsureSuccessStatusCode();
                return await t.Result.Content.ReadAsStringAsync().ContinueWith(async (readtask) =>
                {
                    var page = new HtmlDocument();
                    page.LoadHtml(readtask.Result);
                    var firstArticle = page.DocumentNode.Descendants().FirstOrDefault(x => x.Name == "article");
                    ID = Int32.Parse(firstArticle.Id.Remove(0, 5));
                    int highestID = await App.Database.GetHighestPeopleID();
                    if (highestID == ID)
                        return true;
                    return false;
                });
            });
        }
        #endregion
        #region Category helpers

        /// <summary>
        /// Checks if str is part of categories (with name to slug conversion)
        /// </summary>
        /// <param name="str"></param>
        /// <param name="categories"></param>
        /// <returns></returns>
        public bool CheckInList(string str, List<string> categories)
        {
            switch (str)
            {
                case "":
                    return true;
                case "Build a smarter future":
                    if (categories.Contains("build-smarter-future"))
                        return true;
                    break;
                case "Be creative":
                    if (categories.Contains("creative-careers"))
                        return true;
                    break;
                case "Build healthy communities":
                    if (categories.Contains("health"))
                        return true;
                    break;
                case "Create social change":
                    if (categories.Contains("social-change"))
                        return true;
                    break;
                case "Get immediate skills":
                    if (categories.Contains("get-immediate-skills"))
                        return true;
                    break;
                case "Get into tech":
                    if (categories.Contains("get-into-tech"))
                        return true;
                    break;
                case "Solve global prblems":
                    if (categories.Contains("solve-global-problem"))
                        return true;
                    break;
                case "Start a business":
                    if (categories.Contains("business") || categories.Contains("startup"))
                        return true;
                    break;
                case "Maths":
                    if (categories.Contains("maths"))
                        return true;
                    break;
                case "Science":
                    if (categories.Contains("science"))
                        return true;
                    break;
                case "Engineering":
                    if (categories.Contains("engineering"))
                        return true;
                    break;
                case "Computer Science":
                    if (categories.Contains("code"))
                        return true;
                    break;
                default:
                    break;
            }
            return false;
        }

        /// <summary>
        /// Converts a category name to it's slug (for DB purposes)
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string ConvertCategory(string str)
        {
            switch (str)
            {
                case "":
                    return "";
                case "Build a smarter future":
                    return "build-smarter-future";
                case "Be creative":
                    return "creative-careers";
                case "Build healthy communities":
                    return "health";
                case "Create social change":
                    return "social";
                case "Get immediate skills":
                    return "get-immediate-skills";
                case "Get into tech":
                    return "get-into-tech";
                case "Solve global prblems":
                    return "solve-global-problem";
                case "Start a business":
                    return "business";
                case "Maths":
                    return "maths";
                case "Science":
                    return "science";
                case "Engineering":
                    return "engineering";
                case "Computer Science":
                    return "code";
                default:
                    break;
            }
            return "";
        }

        #endregion
    }
}
