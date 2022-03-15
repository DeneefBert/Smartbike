using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace smartbike_G2.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();
            GetImagesAndText();
        }

        private void GetImagesAndText()
        {
            imgContact.Source = ImageSource.FromResource("smartbike_G2.Resources.icon_contact.png");
            imgAbout.Source = ImageSource.FromResource("smartbike_G2.Resources.icon_over.png");
            lblAboutUsText.Text = "Hallo, wij zijn een groep studenten uit 2MCT.\n\nWij zijn studenten uit het tweede jaar MCT (Multimedia and Creative Technologies) en wij hebben deze app ontwikkeld voor een project dat we moesten maken op vraag van de richting Sport en Beweging binnen de hogeschool Howest. De app dat u nu ziet is het resultaat van dit project. Veel fietsplezier.";

            imgPhone.Source = ImageSource.FromResource("smartbike_G2.Resources.icon_phonenumber.png");
            imgMap.Source = ImageSource.FromResource("smartbike_G2.Resources.icon_map.png");
            imgSite.Source = ImageSource.FromResource("smartbike_G2.Resources.icon_site.png");
            imgMail.Source = ImageSource.FromResource("smartbike_G2.Resources.icon_mail.png");

            lblPhone.Text = "+32 56 24 12 44";
            lblMap.Text = "Graaf Karel de Goedelaan 5,\n8500 Kortrijk";
            lblSite.Text = "www.mct.be";
            lblMail.Text = "mct@howest.be";

            imgHowest.Source = ImageSource.FromResource("smartbike_G2.Resources.logo_howest.png");
            imgSportEnBewegen.Source = ImageSource.FromResource("smartbike_G2.Resources.logo_sport_en_bewegen.png");
            imgMCT.Source = ImageSource.FromResource("smartbike_G2.Resources.logo_mct.png");
        }
    }
}