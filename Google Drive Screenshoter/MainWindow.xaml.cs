using System.Windows;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Google_Drive_Screenshoter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/drive-dotnet-quickstart.json
        readonly string[] Scopes = { DriveService.Scope.Drive };
        readonly string ApplicationName = "Google Drive Screnshoter";
        static UserCredential credential;
        static DriveService service;

        private GlobalKeyboardHook hook;

        public object EnablePrtScr { get; private set; }

        public MainWindow()
        {

            InitializeComponent();
            credential = DriveHandler.LoadCredentials(Scopes);

            // Create Drive API service.
            service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            //// Define parameters of request.
            //FilesResource.ListRequest listRequest = service.Files.List();
            //listRequest.PageSize = 10;
            //listRequest.Fields = "nextPageToken, files(id, name)";

            //// List files.
            //IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute()
            //    .Files;
            //if (files != null && files.Count > 0)
            //{
            //    foreach (var file in files)
            //    {
            //        Console.WriteLine("{0} ({1})", file.Name, file.Id);
            //    }
            //}
            //else
            //{
            //    Console.WriteLine("No files found.");
            //}
            //Console.Read();

            Username.Content = DriveHandler.PrintUsername(service);
            Logo.Source = ImageHandler.LoadImage(DriveHandler.UserLogo(service));
            ChooseFolder.ItemsSource = DriveHandler.FolderList(service);

            //Instance of global keyboard hook to 
            SetupKeyboardHooks();
        }

        private void LogOut_Click(object sender, RoutedEventArgs e)
        {
            Username.Content = DriveHandler.ChangeAccount(credential);
            Logo.Source = ImageHandler.LoadImage("https://firebasestorage.googleapis.com/v0/b/drive-assets.google.com.a.appspot.com/o/Asset%20-%20Drive%20Icon512.png?alt=media");
            ChooseFolder.ItemsSource = null;
        }

        private void SignIn_Click(object sender, RoutedEventArgs e)
        {
            credential = DriveHandler.LoadCredentials(Scopes);
            service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
            Username.Content = DriveHandler.PrintUsername(service);
            var userLogo = DriveHandler.UserLogo(service);
            if (userLogo == "" || userLogo == null)
                Logo.Source = ImageHandler.TextImage(
                    Username.Content.ToString(),
                    new System.Drawing.Font("arial", 12),
                    System.Drawing.Color.Black,
                    System.Drawing.Color.Aquamarine);
            else
                Logo.Source = ImageHandler.LoadImage(userLogo);
        }

        public void SetupKeyboardHooks()
        {
            hook = new GlobalKeyboardHook();
            hook.KeyboardPressed += OnKeyPressed;
        }

        private void OnKeyPressed(object sender, GlobalKeyboardHookEventArgs e)
        {
            Debug.WriteLine(e.KeyboardData.VirtualCode);

            if (e.KeyboardData.VirtualCode != GlobalKeyboardHook.VkSnapshot)
                return;

            // seems, not needed in the life.
            //if (e.KeyboardState == GlobalKeyboardHook.KeyboardState.SysKeyDown &&
            //    e.KeyboardData.Flags == GlobalKeyboardHook.LlkhfAltdown)
            //{
            //    MessageBox.Show("Alt + Print Screen");
            //    e.Handled = true;
            //}
            //else

            if (e.KeyboardState == GlobalKeyboardHook.KeyboardState.KeyDown)
            {
                if(Username.Content.ToString() != "No selected account")
                {
                    var filename = Screenshoter.MakeScreenshot(Filename.Text, Datetime.IsChecked.Value);
                    var folderName = ChooseFolder.SelectedItem.ToString();
                    UploadScreenshot(filename, folderName);
                }
                //MessageBox.Show("Print Screen");
                e.Handled = true;
            }
        }

        public void Dispose()
        {
            hook?.Dispose();
        }

        public void UploadScreenshot(string filename, string folderName)
        {
            var fileMetadata = new File()
            {
                Name = filename
            };
            if (folderName != "Root folder")
            {
                var parents = new List<string>();
                parents.Add(DriveHandler.FolderId(folderName, service));
                fileMetadata.Parents = parents;
            }
            Console.WriteLine(filename);
            FilesResource.CreateMediaUpload request;
            
            using (var stream = new System.IO.FileStream(filename,
                                    System.IO.FileMode.Open))
            {
                Console.WriteLine(stream.Name);
                request = service.Files.Create(
                    fileMetadata, stream, "image/png");
                request.Fields = "id";
                request.Upload();
            }
            var file = request.ResponseBody;

            Console.WriteLine(filename);
            Console.WriteLine("File ID: " + file.Id);
            System.IO.File.Delete(filename);
        }
    }
}
