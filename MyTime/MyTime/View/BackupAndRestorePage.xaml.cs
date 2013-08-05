using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.IsolatedStorage;
using System.Text.RegularExpressions;
using System.Windows;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Live;
using Microsoft.Live.Controls;
using Microsoft.Phone.Controls;

namespace FieldService.View
{
	public partial class BackupAndRestorePage : PhoneApplicationPage
	{
		private LiveConnectClient _client;
		private string _newestFile;
		private ProcessType _processing = ProcessType.Login;

		public BackupAndRestorePage() { InitializeComponent(); }

		#region Events

		private void _client_BackgroundUploadCompleted(object sender, LiveOperationCompletedEventArgs e)
		{
			//
			bBackup.IsEnabled = true;
			bRestore.IsEnabled = true;

			pbProgress.Visibility = Visibility.Collapsed;
			lblLastBackup.Text = DateTime.Now.ToString("MM/dd/yyyy");
		}

		private void _client_BackgroundUploadProgressChanged(object sender, LiveUploadProgressChangedEventArgs e)
		{
			bBackup.IsEnabled = false;
			bRestore.IsEnabled = false;
			pbProgress.Visibility = Visibility.Visible;
			pbProgress.Value = e.ProgressPercentage;
			lblLastBackup.Text = string.Format(StringResources.BackupAndRestorePage_Messages_Progress, e.BytesSent, e.TotalBytesToSend);
		}

		private void _client_DownloadCompleted(object sender, LiveDownloadCompletedEventArgs e)
		{
			bBackup.IsEnabled = true;
			bRestore.IsEnabled = true;

			pbProgress.Visibility = Visibility.Collapsed;

			lblLastBackup.Text = StringResources.BackupAndRestorePage_Messages_Restoring;
			MemoryStream fs = e.Result as MemoryStream;
			if (fs != null) RestoreFromFile(fs);
			//try {
			//    IsolatedStorageFile.GetUserStoreForApplication().DeleteFile("restore.zip");
			//} catch { }
		}

		private void _client_DownloadProgressChanged(object sender, LiveDownloadProgressChangedEventArgs e)
		{
			//
			//throw new NotImplementedException();
			bBackup.IsEnabled = false;
			bRestore.IsEnabled = false;
			pbProgress.Visibility = Visibility.Visible;
			pbProgress.Value = e.ProgressPercentage;
			lblLastBackup.Text = string.Format(StringResources.BackupAndRestorePage_Messages_DwnlProgress, e.BytesReceived, e.TotalBytesToReceive);
		}

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

		private void bLiveSignIn_SessionChanged(object sender, LiveConnectSessionChangedEventArgs e)
		{
			if (e.Status != LiveConnectSessionStatus.Connected) return;

			_client = new LiveConnectClient(e.Session);
			App.LiveSession = e.Session;

			_client.GetCompleted += skyDriveClient_GetCompleted;
			_client.GetAsync("me", null);
		}

		private void bRestore_Click(object sender, RoutedEventArgs e) { DownloadLatestBackupFile(); }

		private void skyDriveClient_GetCompleted(object sender, LiveOperationCompletedEventArgs e)
		{
			if (e.Error != null) {
				return;
			}
			switch (_processing) {
				case ProcessType.Login:
					try {
						lblLoginResult.Text = string.Format(StringResources.BackupAndRestorePage_Messages_Welcome,
															e.Result.ContainsKey("first_name") && e.Result["first_name"] != null ? e.Result["first_name"].ToString() : "",
															e.Result.ContainsKey("last_name") && e.Result["last_name"] != null ? e.Result["last_name"].ToString() : "").TrimEnd() + "!";
					} catch {
						lblLoginResult.Text = "Welcome!";
					}
					_processing = ProcessType.GetInfo;
					try {
						_client.GetAsync("/me/skydrive/files");
					} catch (Exception ee) {
						MessageBox.Show(StringResources.BackupAndRestorePage_Messages_SkyDriveListingFailed);
						NavigationService.GoBack();
					}
					return;
				case ProcessType.GetInfo:
					var data = (List<object>) e.Result["data"];
					GetLastBackupFile(data);
					break;
			}
		}

		#endregion

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
					}
				}
			}
			lblLastBackup.Text = StringResources.BackupAndRestorePage_Messages_RestoreSuccess;
			App.ToastMe(StringResources.BackupAndRestorePage_Messages_RestoreSuccess);
		}

		private void DownloadLatestBackupFile()
		{
			//
			//
			_client.DownloadProgressChanged += _client_DownloadProgressChanged;
			_client.DownloadCompleted += _client_DownloadCompleted;
			_client.DownloadAsync(_newestFile+"/Content", new MemoryStream());
		}

		private void BackupFilesAndUpload(string _backupFileName)
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


			using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication()) {
				using (var file = new IsolatedStorageFileStream(filename, FileMode.Create, store)) {
					var zz = new ZipOutputStream(file);
					foreach (string f in store.GetFileNames()) {
						if (f.Equals(filename)) continue;
						var ze = new ZipEntry(f);
						zz.PutNextEntry(ze);
						using (IsolatedStorageFileStream ss = store.OpenFile(f, FileMode.Open)) {
							var bytes = new byte[100];
							int x = ss.Read(bytes, 0, 100);
							while (x > 0) {
								zz.Write(bytes, 0, x);
								bytes = new byte[100];
								x = ss.Read(bytes, 0, 100);
							}
						}
					}
					zz.Finish();
				}

				_client.UploadCompleted += _client_BackgroundUploadCompleted;
				_client.UploadProgressChanged += _client_BackgroundUploadProgressChanged;
				IsolatedStorageFileStream bkup = store.OpenFile(filename, FileMode.Open);
				_client.UploadAsync("/me/skydrive", filename, bkup, OverwriteOption.Overwrite);
			}
		}

		private void GetLastBackupFile(IEnumerable<object> data)
		{
			_newestFile = null;
			DateTime newestDate = DateTime.MinValue;
			foreach (IDictionary<string, object> d in data) {
				string name = d["name"].ToString();
				if (name.Contains("fieldservicebackup")) {
					string date = Regex.Match(name, @"^fieldservicebackup_(?<date>\d\d\-\d\d\-\d\d\d\d)\.zip$").Groups["date"].Value;
					DateTime dateT = DateTime.ParseExact(date, "MM-dd-yyyy", CultureInfo.InvariantCulture);

					if (dateT > newestDate) {
						_newestFile = d["id"].ToString();
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
			return;
		}

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