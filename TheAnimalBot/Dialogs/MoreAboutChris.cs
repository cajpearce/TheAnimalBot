using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;

namespace TheAnimalBot.Dialogs
{
    [Serializable]
    public class MoreAboutChris : IDialog<object>
    {

        private static readonly List<string> options = new List<string>
        {
            "about him",
            "his university days",
            "his high school days",
            "his hobbies",
            "I'm done"
        };
        public async Task StartAsync(IDialogContext context)
        {
            PromptDialog.Choice(context, ResumeAfterAsync,
                            new PromptOptions<string>("What would you like to know about?",
                                "I didn't catch that, what would you like to know about?",
                                "Sorry, I didn't understand.",
                                options,
                                1));
        }

        public async Task ResumeAfterAsync(IDialogContext context, IAwaitable<string> result)
        {
            string retter = null;
            try
            {
                context.UserData.TryGetValue("latest_message", out retter);
                retter = await result;
                    // this will break if one of the pre-existing options did not exist, hence why it's in the same line

                switch (retter)
                {
                    case "about him":
                        await context.PostAsync("I am, at heart, a data scientist + developer. I love data. " +
                                                "At a young age I fell in love with coding, numbers and statistics and I have been working on developing my skills for to use it in my career. " +
                                                "I struggled with school, but my love of all things dev & data means I can concentrate for hours and hours on the task at hand. Heck, I'm writing this at 11PM right now!");
                        break;
                    case "his university days":
                        await context.PostAsync(
                            "At university I flipped between multiple majors before finally settling on statistics and computer science with a second degree in law. " +
                            "I am about to enter my Honours year of my Statistics degree and am putting my final year of law on hold till I've established myself in the workforce.");
                        break;
                    case "his high school days":
                        await context.PostAsync(
                            "In high school I was a massive nerd. I played a lot of video games, hosting quite a few servers, but I also played a lot of sport and was involved in multiple public speaking events. " +
                            "I won my high school public speaking competition in year 8, and regularly won awards for mathematics.");
                        break;
                    case "his hobbies":
                        await context.PostAsync(
                            "In my spare time I enjoy gaming, hiking and photography. I used to play football regularly till my knee injury, which I make up for by being an avid supporter of Bournemouth in the English Premier League.");
                        break;
                    case "I'm done":
                    default:
                        throw new Exception();

                }
                context.Call(new MoreAboutChris(), ResumeAfterOptionDialog);
                
            }
            catch (Exception)
            {
                context.Done<object>(null);
            }
        }

        public async Task ResumeAfterOptionDialog(IDialogContext context, IAwaitable<object> result)
        {
            var message = await result;
            context.Done<object>(null);
        }
    }
}