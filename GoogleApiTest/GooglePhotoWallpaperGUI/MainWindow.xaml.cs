using GooglePhotoWallpaperREST;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GooglePhotoWallpaperGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<GooglePhotosAlbum> ContentSelectorListBoxSource { get; set; }

        Program prog;

        private SlideshowSettings slideshowSettings;

        public SlideshowSettings SlideshowSettings
        {
            get
            {
                if (slideshowSettings == null) slideshowSettings = new SlideshowSettings();
                return slideshowSettings;
            }
        }


        public MainWindow()
        {
            InitializeComponent();

            StartLogic();
        }

        private async void StartLogic()
        {
            try
            {
                prog = new Program();
                await prog.SignInAndInitiateService();

                if (prog.service != null)
                {
                    EnableConfigView();


                }
                else NotifyUserAboutProblem();
            }
            catch (AggregateException ex)
            {
                Console.WriteLine(ex.ToString());
                foreach (var exception in ex.InnerExceptions)
                {
                    Console.WriteLine("ERROR: " + exception.Message);
                }
                NotifyUserAboutProblem();
            }
        }

        private void NotifyUserAboutProblem()
        {
            UserInfo.Text = "There was an error during sigin. Restart this application to retry!";
        }

        private async void EnableConfigView()
        {
            this.Show();

            signin.Visibility = Visibility.Hidden;
            signedin.Visibility = Visibility.Visible;

            ContentSelectorListBoxSource = new ObservableCollection<GooglePhotosAlbum>();
            ContentSelectorListBoxSource.Add(new GooglePhotosAlbum { title = "Favorite Pictures", id = "favPic", IsSelected = true });
            SlideshowSettings.AddSelectedAlbumId(ContentSelectorListBoxSource[0].id);
            this.DataContext = this;

            GooglePhotosAlbumsCollection albums = await prog.service.FetchAllAlbums();

            foreach (var anAlbum in albums.albums)
            {
                ContentSelectorListBoxSource.Add(anAlbum);
            }

            LoadingText.Visibility = Visibility.Collapsed;

            //DisplayConfiguratorOrderBy.Children.Add(new RadioButton() { GroupName = "OrderBy", Content = "File name" });
            //DisplayConfiguratorOrderBy.Children.Add(new RadioButton() { GroupName = "OrderBy", Content = "Creation date", IsChecked = true });

            //DisplayConfiguratorOrder.Children.Add(new RadioButton() { GroupName = "Order", Content = "Ascending" });
            //DisplayConfiguratorOrder.Children.Add(new RadioButton() { GroupName = "Order", Content = "Descending", IsChecked = true });
            //DisplayConfiguratorOrder.Children.Add(new RadioButton() { GroupName = "Order", Content = "Random" });


        }


        private void OrderSelected(object sender, RoutedEventArgs e)
        {
            RadioButton aRadioButton = sender as RadioButton;

            if (aRadioButton.Content.ToString().Equals(Order.Ascending.ToString())) SlideshowSettings.order = Order.Ascending;
            else if (aRadioButton.Content.ToString().Equals(Order.Descending.ToString())) SlideshowSettings.order = Order.Descending;
            else if (aRadioButton.Content.ToString().Equals(Order.Random.ToString())) SlideshowSettings.order = Order.Random;
        }

        private void OrderBySelected(object sender, RoutedEventArgs e)
        {
            RadioButton aRadioButton = sender as RadioButton;

            if (aRadioButton.Name.ToString().Equals(OrderBy.DateTime.ToString())) SlideshowSettings.orderBy = OrderBy.DateTime;
            else if (aRadioButton.Name.ToString().Equals(OrderBy.FileName.ToString())) SlideshowSettings.orderBy = OrderBy.FileName;
            
        }

        private void AnAlbumSelected(object sender, RoutedEventArgs e)
        {
            SlideshowSettings.AddSelectedAlbumId((sender as CheckBox).Uid);
        }

        private void AnAlbumDeSelected(object sender, RoutedEventArgs e)
        {
            SlideshowSettings.RemoveSelectedAlbumId((sender as CheckBox).Uid);
        }
    }
}
