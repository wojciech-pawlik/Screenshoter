using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Util.Store;
using System;
using System.IO;
using System.Threading;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Google_Drive_Screenshoter
{
    class DriveHandler
    {
        public static UserCredential LoadCredentials(string[] Scopes)
        {
            UserCredential credential;

            using (var stream =
                new FileStream("client_id.json", FileMode.Open, FileAccess.Read))
            {
                CancellationTokenSource cts = new CancellationTokenSource();
                cts.CancelAfter(TimeSpan.FromSeconds(20));
                CancellationToken ct = cts.Token;

                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    ct,
                    new FileDataStore(credPath, true)).Result;

                if (ct.IsCancellationRequested)
                {
                    Debug.WriteLine("Credential file saved to: " + credPath);
                    return null;
                }
                Debug.WriteLine("Credential file saved to: " + credPath);
            }
            return credential;
        }



        /// <summary>
        /// Print information about the current user along with the Drive API
        /// settings.
        /// </summary>
        /// <param name="service">Drive API service instance.</param>
        public static string PrintUsername(DriveService service)
        {
            string username;
            try
            {
                var request = service.About.Get();
                request.Fields = "user";
                Debug.WriteLine("Current user name: " + request.Execute().User.DisplayName);
                username = request.Execute().User.DisplayName;
            }
            catch (Exception e)
            {
                Debug.WriteLine("An error occurred: " + e.Message);
                username = "failed to load username";
            }
            return username;
        }

        public static string UserLogo(DriveService service)
        {
            string logo;
            try
            {
                var request = service.About.Get();
                request.Fields = "user";
                Debug.WriteLine("User logo uri: " + request.Execute().User.PhotoLink);
                logo = request.Execute().User.PhotoLink;
            }
            catch (Exception e)
            {
                Debug.WriteLine("An error occurred: " + e.Message);
                logo = "http://icon-library.com/images/error-image-icon/error-image-icon-1.jpg";
            }
            Debug.WriteLine(logo);
            return logo;
        }

        public static string ChangeAccount(UserCredential credential)
        {
            credential.RevokeTokenAsync(CancellationToken.None);
            return "No selected account";
        }

        public static ObservableCollection<string> FolderList(DriveService service)
        {
            var request = service.Files.List();
            request.Q = "mimeType='application/vnd.google-apps.folder'";
            var list = request.Execute().Files;
            ObservableCollection<string> folders = new ObservableCollection<string>();
            folders.Add("Root folder");
            foreach (Google.Apis.Drive.v3.Data.File folder in list)
            {
                folders.Add(folder.Name);
            }
            return folders;
        }

        public static string FolderId(string folderName, DriveService service)
        {
            var request = service.Files.List();
            request.Q = "mimeType='application/vnd.google-apps.folder'";
            request.Q = "name='" + folderName + "'";
            var list = request.Execute().Files;
            return list[0].Id;
        }
    }
}
