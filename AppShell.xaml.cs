namespace MyMauiApp;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute(nameof(GirisSayfasi), typeof(GirisSayfasi)); 
		Routing.RegisterRoute(nameof(KayitOlSayfasi), typeof(KayitOlSayfasi)); 
		Routing.RegisterRoute(nameof(AnaSayfa), typeof(AnaSayfa)); 
		Routing.RegisterRoute(nameof(DovizKurlariSayfasi), typeof(DovizKurlariSayfasi)); 
		Routing.RegisterRoute(nameof(HaberlerSayfasi), typeof(HaberlerSayfasi)); 
		Routing.RegisterRoute(nameof(HaberDetaySayfasi), typeof(HaberDetaySayfasi)); 
		Routing.RegisterRoute(nameof(HavaDurumuSayfasi), typeof(HavaDurumuSayfasi));
		Routing.RegisterRoute(nameof(IlSecimSayfasi), typeof(IlSecimSayfasi));
        Routing.RegisterRoute(nameof(YapilacaklarSayfasi), typeof(YapilacaklarSayfasi));
        Routing.RegisterRoute(nameof(GorevEklemeSayfasi), typeof(GorevEklemeSayfasi)); // Added GorevEklemeSayfasi
        Routing.RegisterRoute(nameof(AyarlarSayfasi), typeof(AyarlarSayfasi));
    }
}
