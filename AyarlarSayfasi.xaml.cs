namespace MyMauiApp;

public partial class AyarlarSayfasi : ContentPage
{
	public AyarlarSayfasi()
	{
		InitializeComponent();
		      // Sayfa yüklendiğinde mevcut tema durumunu Switch'e yansıt
		      if (Application.Current != null)
		      {
		          KoyuModSwitch.IsToggled = Application.Current.RequestedTheme == AppTheme.Dark;
		      }
		  }

		  private void KoyuModSwitch_Toggled(object sender, ToggledEventArgs e)
		  {
		      if (Application.Current != null)
		      {
		          Application.Current.UserAppTheme = e.Value ? AppTheme.Dark : AppTheme.Light;
		      }
		  }
}