using Android.App;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
#if DEBUG                                   // connect to local service on the
[assembly: Application(UsesCleartextTraffic = true)]  // emulator's host for debugging,
#else                                       // access via http://10.0.2.2
[Application]                               
#endif