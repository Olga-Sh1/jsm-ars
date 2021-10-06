using BaseElements;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using JSMBase.Flights;
using JSMBase.RNK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JSMBase.Medicine.RWServices.Excel
{
    /// <summary>Считывание данных с Google Drive</summary>
    public sealed class ParserGDrive : ParserCommon
    {
        static readonly String folderDrivePath = "0Byn_MQOSV93HQ1N3REVNZ0U4S3M";
        /// <summary>Получить данные</summary>
        /// <param name="path">Директория</param>
        /// <returns></returns>
        public override async Task<IEnumerable<Flight>> Parse(string path, IProgress<ProgressReport> pr)
        {
            String driveId = null;
            List<Flight> results = new List<Flight>();

            //Scopes for use with the Google Drive API
            string[] scopes = new string[] { DriveService.Scope.Drive, DriveService.Scope.DriveFile };
            var clientId = "987812926785-8tgaj88j5o0olev9lb0ucl9dqf2bcbuj.apps.googleusercontent.com";      // From https://console.developers.google.com
            var clientSecret = "XdKvOgoa7QRmzvYS3vvTne_s";          // From https://console.developers.google.com
                                                                    // here is where we Request the user to give us access, or use the Refresh Token that was previously stored in %AppData%
            var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(new ClientSecrets
            {
                ClientId = clientId,
                ClientSecret = clientSecret
            },
            scopes,
            Environment.UserName,
            CancellationToken.None,
            new FileDataStore("MyTest")).Result;

            using (var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Drive API Sample",
            }))
            {
                //ID папки по имени
                var requestD = service.Files.List();
                requestD.Q = String.Format("mimeType='application/vnd.google-apps.folder' and name='{0}'", path, ParserCommon.FILE_SUMMARY_NAME);
                requestD.Spaces = "drive";
                var resultD = await requestD.ExecuteAsync();
                if (resultD.Files.Count > 0)
                {
                    driveId = resultD.Files[0].Id;
                }
                if (driveId == null)
                    return results;

                //Список всех файлов
                var request = service.Files.List();
                request.Q = String.Format("name='schema.ini'", driveId);
                request.Spaces = "drive";
                var result = await request.ExecuteAsync();
                if (result.Files.Count > 0)
                {
                    String id = result.Files[0].Id;
                    var requestGet = service.Files.Get(id);
                    using (var stream = new System.IO.MemoryStream())
                    {
                        await requestGet.DownloadAsync(stream);
                        stream.Seek(0, SeekOrigin.Begin);
                        using (var reader = new StreamReader(stream, Encoding.UTF8))
                        {
                            IEnumerable<IPropInfo> infos = INI.Parser.Parse(reader);
                            Flight.TableInfos = infos.ToArray();
                            stream.Seek(0, SeekOrigin.Begin);
                            IEnumerable<IPropInfo> sinfos = INI.Parser.Parse(reader, ParserCommon.FILE_SUMMARY_NAME);
                            Flight.Infos = sinfos.ToArray();
                        }
                    }
                }

                //Сводная инфа
                var requestList = service.Files.List();
                requestList.Q = String.Format("mimeType='application/vnd.ms-excel' and '{0}' in parents", driveId);
                requestList.Spaces = "drive";
                Google.Apis.Drive.v3.Data.FileList resultList = null;

                do
                {
                    resultList = await requestList.ExecuteAsync();
                    foreach (var file in resultList.Files)
                    {
                        if (file.Name == ParserCommon.FILE_SUMMARY_NAME) continue;
                        String id = file.Id;
                        var requestGet = service.Files.Get(id);
                        Flight f = new Flight();
                        using (var stream = new System.IO.MemoryStream())
                        {
                            await requestGet.DownloadAsync(stream);
                            stream.Seek(0, SeekOrigin.Begin);
                            f = ParseFlight(stream, file.Name);
                        }
                        //SetIds(f, file.Name);
                        results.Add(f);
                    }
                    requestList.PageToken = resultList.NextPageToken;
                } while (resultList.NextPageToken != null);
                

                var requestS = service.Files.List();
                requestS.Q = String.Format("name='{1}'", driveId, ParserCommon.FILE_SUMMARY_NAME);
                requestS.Spaces = "drive";
                var resultS = requestS.Execute();
                if (resultS.Files.Count > 0)
                {
                    String id = resultS.Files[0].Id;
                    var requestGet = service.Files.Get(id);
                    using (var stream = new System.IO.MemoryStream())
                    {
                        requestGet.Download(stream);
                        stream.Seek(0, SeekOrigin.Begin);
                        AddInfo(stream, results);
                       
                    }
                }
            }
            return results;
        }

        public IEnumerable<String> FindAllDrives()
        {
            string[] scopes = new string[] { DriveService.Scope.Drive, DriveService.Scope.DriveFile };
            var clientId = "987812926785-8tgaj88j5o0olev9lb0ucl9dqf2bcbuj.apps.googleusercontent.com";      // From https://console.developers.google.com
            var clientSecret = "XdKvOgoa7QRmzvYS3vvTne_s";          // From https://console.developers.google.com
                                                                    // here is where we Request the user to give us access, or use the Refresh Token that was previously stored in %AppData%
            var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(new ClientSecrets
            {
                ClientId = clientId,
                ClientSecret = clientSecret
            },
            scopes,
            Environment.UserName,
            CancellationToken.None,
            new FileDataStore("MyTest")).Result;

            using (var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Drive API Sample",
            }))
            {
                //Все доступные папки
                var requestD = service.Files.List();
                requestD.Q = "mimeType='application/vnd.google-apps.folder' and trashed = false";
                requestD.Spaces = "drive";
                var resultD = requestD.Execute();
                return resultD.Files.Select(f => createRecursive(resultD.Files, f)).ToArray();
            }
        }

        private String createRecursive(IEnumerable<Google.Apis.Drive.v3.Data.File> lst, Google.Apis.Drive.v3.Data.File f)
        {
            var parent = lst.FirstOrDefault(ff => f.Parents != null && f.Parents.Contains(ff.Id));
            return (parent != null ? createRecursive(lst, parent) + "/" : "") + f.Name;
        }
    }
}
