using STEM_Careers.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace STEM_Careers.Helpers
{
    public class WebContentHelper
    {
        public HttpClient client;
        public WebContentHelper()
        {
            client = new HttpClient();
        }

        //public async Task GetPerson(People person)
        //{
        //    var page = new HtmlDocument();
        //    page.LoadHtml(person.Href);
        //    return;
        //}

        public bool CheckInList(string str, List<string> categories)
        {
            switch(str){
                case "":
                    return true;
                case "Build a smarter future":
                    if (categories.Contains("build-smarter-future"))
                        return true;
                    break;
                case "Be creative":
                    if (categories.Contains("creators") || categories.Contains("creative-careers"))
                        return true;
                    break;
                case "Build healthy communities":
                    if (categories.Contains("health")  || categories.Contains("healers"))
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
                    if (categories.Contains("business"))
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
                case "Technology":
                    if (categories.Contains("technology"))
                        return true;
                    break;
                default:
                    break;
            }
            return false;
        }

        private void PeopleFromHtml(string html, string X = "", string field = "")
        {
            List<People> items = new List<People>();
            var page = new HtmlDocument();
            page.LoadHtml(html);

            var firstArticle = page.DocumentNode.Descendants().FirstOrDefault(x => x.Name == "article");
            var articles = firstArticle.ParentNode.Descendants();
            foreach (var article in articles)
            {
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
                    ArticleID = article.Id,
                    Title = aTag.Attributes["title"].Value,
                    Href = aTag.Attributes["href"].Value,
                    ImgReference = imgTag.Attributes["src"].Value,
                    Name = sectionH3.Descendants().FirstOrDefault(x => x.Name == "a").InnerHtml,
                    Description = tmp,
                    ProfileCategories = categories
                };

                MessagingCenter.Send(this, "AddPerson", person);
            }
        }


        public async Task<bool> FetchPeopleArticles(string field = "", string X = "")
        {
            try
            {
                bool notFound = false;
                int i = 0;
                while (notFound == false )
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
                            MessagingCenter.Send<WebContentHelper>(this, "All pages retrieved");
                            return;
                        }
                            
                        t.Result.EnsureSuccessStatusCode();

                        t.Result.Content.ReadAsStringAsync().ContinueWith((readtask) =>
                        {
                            PeopleFromHtml(readtask.Result, X, field);
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
    }
}
