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
        public ObservableCollection<BoolTextClass> ContentSelectorListBoxSource { get; set; }

        Program prog;

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

            ContentSelectorListBoxSource = new ObservableCollection<BoolTextClass>();
            ContentSelectorListBoxSource.Add(new BoolTextClass { AlbumName = "Favorite Pictures", AlbumId = "favPic", IsSelected = true});
            GooglePhotosAlbumsCollection albums = await prog.service.FetchAllAlbums();
            
            foreach (var anAlbum in albums.albums)
            {
                ContentSelectorListBoxSource.Add(new BoolTextClass { AlbumName = anAlbum.title, AlbumId= anAlbum.id});
            }
            LoadingText.Visibility = Visibility.Collapsed;
            this.DataContext = this;

            //DisplayConfiguratorOrderBy.Children.Add(new RadioButton() { GroupName = "OrderBy", Content = "File name" });
            //DisplayConfiguratorOrderBy.Children.Add(new RadioButton() { GroupName = "OrderBy", Content = "Creation date", IsChecked = true });

            //DisplayConfiguratorOrder.Children.Add(new RadioButton() { GroupName = "Order", Content = "Ascending" });
            //DisplayConfiguratorOrder.Children.Add(new RadioButton() { GroupName = "Order", Content = "Descending", IsChecked = true });
            //DisplayConfiguratorOrder.Children.Add(new RadioButton() { GroupName = "Order", Content = "Random" });


        }
    }
}
