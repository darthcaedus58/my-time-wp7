using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Windows.Storage;
using Coding4Fun.Phone.Controls.Converters;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Live;
using Microsoft.Live.Controls;
using Microsoft.Phone.BackgroundTransfer;
using Microsoft.Phone.Controls;
using Salient.SharpZipLib.Zip;
using Telerik.Charting;

namespace FieldService.View
{
	public partial class BackupAndRestorePage : PhoneApplicationPage
	{
		private LiveConnectClient _client;
		private string _newestFile;
		private ProcessType _processing = ProcessType.Login;
	        private CancellationTokenSource _ctsUpload;
	        private CancellationTokenSource _ctsDownload;

	        public BackupAndRestorePage() { InitializeComponent(); }

		private void ClearOldBackupFiles()
		{
			//othrow new NotImplementedException();
			try {
				using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication()) {
					foreach (string f in store.GetFileNames()) {
						if (f.EndsWith("zip")) {
							store.DeleteFile(f);
						}
					}
				}
			} catch { 
				//and release
			}
                }

                private void RestoreFromFile(Stream file)
                {
                        //
                        //
                        using (var store = IsolatedStorageFile.GetUserStoreForApplication()) {
                                using (var zip = new ZipInputStream(file)) {
                                        try {
                                                while (true) {
                                                        var ze = zip.GetNextEntry();
                                                        if (ze == null) break;
                                                        using (var f = new IsolatedStorageFileStream(ze.Name, FileMode.Create, store)) {
                                                                var fs = new byte[ze.Size];
                                                                zip.Read(fs, 0, fs.Length);
                                                                f.Write(fs, 0, fs.Length);
                                                        }
                                                }
                                        } catch {
                                                lblLastBackup.Text = StringResources.BackupAndRestorePage_Messages_RestoreFailed;
                                                App.ToastMe(StringResources.BackupAndRestorePage_Messages_RestoreFailed);
                                                return;
                                        } finally {
                                                file.Close();
                                                ClearOldBackupFiles();
                                                App.ViewModel.IsRvDataChanged = true;
                                        }
                                }
                        }
                        lblLastBackup.Text = StringResources.BackupAndRestorePage_Messages_RestoreSuccess;
                        App.ToastMe(StringResources.BackupAndRestorePage_Messages_RestoreSuccess);
                }

                private async void DownloadLatestBackupFile()
                {
                        //
                        //

                        try
                        {
                                bBackup.IsEnabled = false;
                                bRestore.IsEnabled = false;
                                pbProgress.Value = 0;
                                var progressHandler = new Progress<LiveOperationProgress>(
                                        (e) =>
                                        {
                                                pbProgress.Value = e.ProgressPercentage;

                                                pbProgress.Visibility = Visibility.Visible;
                                                lblLastBackup.Text =
                                                        string.Format(
                                                                StringResources
                                                                        .BackupAndRestorePage_Messages_DwnlProgress,
                                                                e.BytesTransferred, e.TotalBytes);
                                        });
                                _ctsDownload = new CancellationTokenSource();
                                _client = new LiveConnectClient(App.LiveSession);
                                var reqList = BackgroundTransferService.Requests.ToList();
                                foreach (var request in reqList)
                                {
                                        if (request.DownloadLocation.Equals(new Uri(@"\shared\transfers\restore.zip", UriKind.Relative)))
                                                BackgroundTransferService.Remove(BackgroundTransferService.Find(request.RequestId));
                                }
                                var token = await _client.DownloadAsync(_newestFile+"/Content", _ctsDownload.Token, progressHandler);
                                
                                lblLastBackup.Text = StringResources.BackupAndRestorePage_Messages_Restoring;
                                RestoreFromFile(token.Stream);
                        }
                        catch (TaskCanceledException)
                        {
                                lblLastBackup.Text = "Download Cancelled.";
                        }
                        catch (LiveConnectException ee)
                        {
                                lblLastBackup.Text = string.Format("Error Downloading: {0}", ee.Message);
                        }
                        catch (Exception e)
                        {
                                lblLastBackup.Text = e.Message;
                        }
                        finally
                        {
                                bBackup.IsEnabled = true;
                                bRestore.IsEnabled = true;
                                pbProgress.Value = 0;
                                pbProgress.Visibility = Visibility.Collapsed;
                        }
                }

                private async void BackupFilesAndUpload(string _backupFileName)
                {
                        if (_backupFileName == null) throw new ArgumentNullException("_backupFileName");
                        //
                        //
                        string filename = _backupFileName;
                        //byte[] b = new byte[0x16] {0x50,0x4b,0x05,0x06,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00};
                        var b = new byte[0x16];
                        b[0] = 0x50;
                        b[1] = 0x4b;
                        b[2] = 0x05;
                        b[4] = 0x06;
                        using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication()) {
                                using (IsolatedStorageFileStream f = store.OpenFile(filename, FileMode.Create)) {
                                        f.Write(b, 0, b.Length);
                                }
                        }


                        using (var store = IsolatedStorageFile.GetUserStoreForApplication()) {
                                using (var file = new IsolatedStorageFileStream(filename, FileMode.Create, store))
                                {
                                        var zz = new ZipOutputStream(file);
                                        foreach (string f in store.GetFileNames())
                                        {
                                                if (f.Equals(filename)) continue;
                                                var ze = new ZipEntry(f);
                                                zz.PutNextEntry(ze);
                                                using (IsolatedStorageFileStream ss = store.OpenFile(f, FileMode.Open))
                                                {
                                                        var bytes = new byte[100];
                                                        int x = ss.Read(bytes, 0, 100);
                                                        while (x > 0)
                                                        {
                                                                zz.Write(bytes, 0, x);
                                                                bytes = new byte[100];
                                                                x = ss.Read(bytes, 0, 100);
                                                        }
                                                }
                                        }
                                        zz.Finish();

                                }
                                try
                                        {
                                                pbProgress.Value = 0;
                                                bBackup.IsEnabled = false;
                                                bRestore.IsEnabled = false;
                                                var progressHandler = new Progress<LiveOperationProgress>(
                                                        (e) =>
                                                        {
                                                                pbProgress.Value = e.ProgressPercentage;
                                                                pbProgress.Visibility = Visibility.Visible;
                                                                lblLastBackup.Text =
                                                                        string.Format(
                                                                                StringResources
                                                                                        .BackupAndRestorePage_Messages_Progress,
                                                                                e.BytesTransferred, e.TotalBytes);
                                                        });
                                                _ctsUpload = new CancellationTokenSource();
                                                _client = new LiveConnectClient(App.LiveSession);
                                                using (
                                                        var file = new IsolatedStorageFileStream(filename, FileMode.Open,
                                                                FileAccess.Read, FileShare.Read,
                                                                IsolatedStorageFile.GetUserStoreForApplication()))
                                                {
                                                        await
                                                                _client.UploadAsync("me/skydrive", filename, file,
                                                                        OverwriteOption.Overwrite, _ctsUpload.Token,
                                                                        progressHandler);
                                                }
                                                pbProgress.Visibility = Visibility.Collapsed;

                                        }
                                        catch (TaskCanceledException)
                                        {
                                                App.ToastMe("Upload Cancelled");
                                        }
                                        catch (LiveConnectException ee)
                                        {
                                                lblLastBackup.Text = string.Format("Error uploading File:{0}",
                                                        ee.Message);
                                        }
                                        finally
                                        {
                                                bBackup.IsEnabled = true;
                                                bRestore.IsEnabled = true;
                                                lblLastBackup.Text = DateTime.Now.ToString("MM/dd/yyyy");
                                        }
                                
                        }
                }

		#region Events

		private void bBackup_Click(object sender, RoutedEventArgs e)
		{
			try {
				ClearOldBackupFiles();
				BackupFilesAndUpload(string.Format("fieldservicebackup_{0:MM-dd-yyyy}.zip", DateTime.Now));
			} catch {
				MessageBox.Show(StringResources.BackupAndRestorePage_Messages_UploadFailed);
				NavigationService.GoBack();
			}
		}


		private void bLiveSignIn_SessionChanged(object sender, LiveConnectSessionChangedEventArgs e)
		{
			if (e.Status != LiveConnectSessionStatus.Connected) return;

			_client = new LiveConnectClient(e.Session);
			App.LiveSession = e.Session;

		        GetMeAndFileListing();
		}

	        private async void GetMeAndFileListing()
	        {
	                try
	                {
	                        _client = new LiveConnectClient(App.LiveSession);
	                        var opResult = await _client.GetAsync("me");
	                        dynamic e = opResult.Result;
	                        try
	                        {
	                                lblLoginResult.Text = string.Format(
	                                        StringResources.BackupAndRestorePage_Messages_Welcome, e.name,"!");
	                        }
	                        catch
	                        {
	                                lblLoginResult.Text = "Welcome!";
	                        }
	                        try
	                        {
	                                _client = new LiveConnectClient(App.LiveSession);
	                                var filesResult = await _client.GetAsync("/me/skydrive/files");
	                                IDictionary<string,object> ee = filesResult.Result;
	                                List<object> data = (List<object>) ee["data"];
                                        _newestFile = null;
                                        DateTime newestDate = DateTime.MinValue;
	                                foreach (dynamic d in data) {
                                                string name = d.name.ToString();
                                                if (name.Contains("fieldservicebackup")) {
                                                        string date = Regex.Match(name, @"^fieldservicebackup_(?<date>\d\d\-\d\d\-\d\d\d\d)\.zip$").Groups["date"].Value;
                                                        DateTime dateT = DateTime.ParseExact(date, "MM-dd-yyyy", CultureInfo.InvariantCulture);

                                                        if (dateT > newestDate) {
                                                                _newestFile = d.id.ToString();
                                                                newestDate = dateT;
                                                        }
                                                }
                                        }
                                        bBackup.Visibility = Visibility.Visible;
                                        lblLastBackup.Visibility = Visibility.Visible;
                                        if (string.IsNullOrEmpty(_newestFile)) {
                                                lblLastBackup.Text = StringResources.BackupAndRestorePage_Messages_NeverBackedUp;
                                                bRestore.Visibility = Visibility.Collapsed;
                                        } else {
                                                lblLastBackup.Text = string.Format("{0:MM/dd/yyyy}", newestDate);
                                                bRestore.Visibility = Visibility.Visible;
                                        }
	                        } catch( Exception)
	                        {
	                                
	                        }
	                }
	                catch (LiveConnectException)
	                {
	                        lblLastBackup.Text = StringResources.BackupAndRestorePage_Messages_SkyDriveListingFailed;
	                }
	        }

	        private void bRestore_Click(object sender, RoutedEventArgs e) { DownloadLatestBackupFile(); }
                #endregion

		#region Nested type: ProcessType

		private enum ProcessType
		{
			Login,
			GetInfo,
			Backup,
			Restore
		};

		#endregion
	}
}