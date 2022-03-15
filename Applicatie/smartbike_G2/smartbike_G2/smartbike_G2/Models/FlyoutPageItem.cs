using Xamarin.Forms;

namespace smartbike_G2.Models
{
    public class FlyoutPageItem
    {
        public FlyoutPageItem(string title, string route, string imageUrl)
        {
            Title = title;
            Route = route;
            ImageUrl = imageUrl;
        }
        public string Title { get; set; }
        public string Route { get; set; }
        public string ImageUrl { get; set; }

        public ImageSource DefaultImage { get { return ImageSource.FromResource("smartbike_G2.Resources." + ImageUrl + ".png"); } }
        public ImageSource SelectedImage { get { return ImageSource.FromResource("smartbike_G2.Resources." + ImageUrl + "_selected.png"); } }
        public ImageSource HomeImage { get { return ImageSource.FromResource("smartbike_G2.Resources." + ImageUrl + "_black.png"); } }
    }
}