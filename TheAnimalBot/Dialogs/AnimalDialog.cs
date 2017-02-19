using System;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using Newtonsoft.Json.Linq;

namespace TheAnimalBot.Dialogs
{
    //[LuisModel("5632e984-be3a-474e-bd12-0cdd7af08ef1", "c88bc943d78141da8da965244458a9b7")]
    [Serializable]
    [LuisModel("4473a376-7ead-44d3-8e41-76125eb81232", "3f1d2c6f421141058f59023da5d88f70")]
    public class AnimalDialog : LuisDialog<object>
    {
        public static string WhichAnimals = "Currently I can look up [these animals](https://en.wikipedia.org/wiki/List_of_animals_by_common_name) for you.";
        public static string HelpString = BuildHelpString();
        
        public static string BuildHelpString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("These are some of the things you can ask me to do: ");
            sb.Append("\n* Show you pictures of cute animals (using [Google](https://developers.google.com/image-search/))");
            sb.Append("\n* Get the weather from a location in New Zealand or abroad (using [OpenWeatherMap](http://openweathermap.org/))");
            sb.Append("\n* Tell you about Christopher");
            sb.Append("\n* Tell you about myself");
            sb.Append("\n\n");
            sb.Append(WhichAnimals);
            return sb.ToString();
        }

        


        [LuisIntent("None")]
        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            //await context.PostAsync("Sorry, I don't understand. Let me know if you need help!");
            context.Done<object>(null);
        }

        [LuisIntent("SendAnimal")]
        public async Task SendAnimalPhoto(IDialogContext context, LuisResult result)
        {
            string wordFound;
            try
            {
                wordFound = result.Entities[0].Entity;
            }
            catch (Exception)
            {
                wordFound = null;
            }
            
            if (wordFound == null)
            {
                await context.PostAsync(WhichAnimals);
            }
            else
            {

                // the max number of photos I want to choose from
                int max_count = 5;

                // the URL for my image API
                string googleUri = $"https://www.googleapis.com/customsearch/v1?q={wordFound}&safe=high&searchType=image&imgSize=medium&imgType=photo&num={max_count}&start=1&cx=013822286513499914710:e5w83vaxdai&key=AIzaSyDjIshslyR6ZFVJjSSPtjXpSbhK1Upb8GE";

                // generate the random number (which photo are we going to use)
                Random r = new Random();
                
                using (WebClient wc = new WebClient())
                {
                    int whichPhoto = r.Next(max_count);
                    string json = wc.DownloadString(googleUri);
                    JToken topToken = JObject.Parse(json);
                    JToken nav = topToken.SelectToken($"items[{whichPhoto}].link");
                    await context.PostAsync($"![{wordFound}]({nav})");
                }

            }
            context.Done<object>(null);
        }

        [LuisIntent("Help")]
        public async Task Help(IDialogContext context, LuisResult result)
        {
            await context.PostAsync(HelpString);
            context.Done<object>(null);
        }

        [LuisIntent("TheWeather")]
        public async Task TheWeather(IDialogContext context, LuisResult result)
        {
            

            try
            {
                string city = result.Entities[0].Entity;
                using (WebClient wc = new WebClient())
                {

                    string json =
                        wc.DownloadString(
                            $"http://api.openweathermap.org/data/2.5/weather?q={city}&APPID=ca25f08da36deabf957d7a4e96acb82b");
                    JToken topToken = JObject.Parse(json);
                    JToken weather = topToken.SelectToken("weather").First.SelectToken("description");
                    JToken name = topToken.SelectToken("name");
                    JToken temperature = topToken.SelectToken("main");
                    temperature = temperature.SelectToken("temp");
                    double temp = Math.Round(double.Parse(temperature.ToString()) - 273.15);
                    await context.PostAsync($"The temperature in **{name}** is {temp}° and the forecast is *{weather}*.");


                }
            }
            catch
            {
                await context.PostAsync("Sorry, I couldn't figure out the placename! I've only been trained on New Zealand and major cities worldwide.");
            }
            finally
            {
                context.Done<object>(null);
            }

            
        }

        [LuisIntent("AboutChris")]
        public async Task AboutChris(IDialogContext context, LuisResult result)
        {
            context.Call(new MoreAboutChris(),ResumeAfterAsync);
        }

        public async Task ResumeAfterAsync(IDialogContext context, IAwaitable<object> result)
        {
            var message = await result;
            context.Done<object>(null);
        }

        [LuisIntent("AboutTim")]
        public async Task AboutTim(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("\"Tim is a chatbot, created as part of the Microsoft Bot Framework, utilising a few APIs for showcasing and Microsoft's LUIS for language understanding.\"");
            context.Done<object>(null);
        }

        [LuisIntent("Farewell")]
        public async Task Farewell(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("I hope I was helpful!");
            context.Done<object>(null);
        }

        [LuisIntent("Greeting")]
        public async Task Greeting(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Hello!");
            context.Done<object>(null);

        }
        
    }
}