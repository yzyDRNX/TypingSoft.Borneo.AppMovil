#if ANDROID
using Android.Content;

namespace TypingSoft.Borneo.AppMovil.Helpers
{
    public class RawBtPrinter : IRawBtPrinter
    {
        public Task PrintTextAsync(string text)
        {
            var context = Android.App.Application.Context;
            var intent = new Intent();
            intent.SetAction(Intent.ActionSend);
            intent.SetType("text/plain");
            intent.PutExtra(Intent.ExtraText, text);
            //intent.SetPackage("ru.a402d.rawbtprinter");
            intent.AddFlags(ActivityFlags.NewTask);
            context.StartActivity(intent);
            return Task.CompletedTask;
        }
    }
}
#endif
