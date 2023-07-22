#region Copyright Syncfusion Inc. 2001-2021.
// Copyright Syncfusion Inc. 2001-2021. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using YoutubeDownloader.Model;
using YoutubeExplode;
using YoutubeExplode.Common;

namespace YoutubeDownloader
{
    //ViewModel class for ListViewComponent
    public class VideoFormatListViewModel : INotifyPropertyChanged
    {
        #region Fields

        private readonly YoutubeClient _youtube = new();
        private ObservableCollection<VideoInfo> _videos;
        private string _videoUrl;
        private string _videoTitle;
        private double _progress;
        private bool _isDownloading;
        private string _progressReport;
        private bool _isBusy;
        private Thumbnail _videoThumbnail;
        private string _videoDuration;
        private bool _showThumbnail;
        private bool _isPopupOpen;
        private string _downloadedMessage;

        #endregion

        #region Constructor

        public VideoFormatListViewModel()
        {
            GetVideoCommand = new Command(
            execute: async () =>
            {
                Reset(true);
                try
                {
                    IsBusy = true;
                    var videos = new ObservableCollection<VideoInfo>();
                    await Task.Run(async () =>
                    {
                        var video = await _youtube.Videos.GetAsync(_videoUrl);
                        VideoThumbnail = video.Thumbnails.GetWithHighestResolution();
                        VideoDuration = video.Duration?.ToString() ?? string.Empty;
                        var streamManifest = await _youtube.Videos.Streams.GetManifestAsync(_videoUrl);
                        var muxedStreamInfos = streamManifest
                        .GetMuxedStreams()
                        .OrderByDescending(s => s.VideoQuality)
                        .ToList();

                        VideoTitle = video.Title;
                        if (!string.IsNullOrWhiteSpace(VideoThumbnail?.Url))
                        {
                            ShowThumbnail = true;
                        }

                        foreach (var streamInfo in muxedStreamInfos)
                        {
                            try
                            {
                                var videoInfo = new VideoInfo
                                {
                                    Title = video.Title,
                                    VideoSize = $"{streamInfo.Size.MegaBytes: 0.00} MB",
                                    Resolution = streamInfo.VideoQuality.Label,
                                    Format = streamInfo.Container.Name,
                                    VideoStream = streamInfo
                                };

                                videos.Add(videoInfo);
                            }
                            catch (Exception ex)
                            {
                                Console.Write(ex.Message);
                                continue;
                            }
                        }
                    });

                    Videos = videos;
                }
                finally
                {
                    IsBusy = false;
                }
            },
            canExecute: () =>
            {
                return !string.IsNullOrWhiteSpace(VideoUrl);
            });

            DownloadCommand = new Command<VideoInfo>(
                execute: async (VideoInfo video) =>
                {
                    try
                    {
                        Reset(false);
                        IsDownloading = true;
                        await Task.Run(async () =>
                        {

                            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "Youtube");
                            if (!Directory.Exists(path))
                            {
                                Directory.CreateDirectory(path);
                            }
                            var fileName = SanitizeFileName($"{video.Title}_{video.Resolution}.{video.VideoStream.Container.Name}");
                            var filePath = Path.Combine(path, fileName);
                            // Set up progress reporting
                            var progressHandler = new Progress<double>(p => Progress = p);

                            // Download to file
                            await _youtube.Videos.Streams.DownloadAsync(video.VideoStream, filePath, progressHandler);
                            DownloadedMessage = $"{fileName} is saved to {path} .";
                        });
                    }
                    catch (Exception ex)
                    {
                        Console.Write(ex.Message);
                    }
                    finally
                    {
                        IsDownloading = false;
                        IsPopupOpen = true;
                    }
                });
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        #endregion

        #region Properties

        public ObservableCollection<VideoInfo> Videos
        {
            get { return _videos; }
            set
            {
                if (_videos != value)
                {
                    _videos = value;
                    OnPropertyChanged("Videos");
                }
            }
        }

        public string VideoUrl
        {
            get
            {
                return _videoUrl;
            }

            set
            {
                if (value != _videoUrl)
                {
                    _videoUrl = value;
                    OnPropertyChanged(nameof(VideoUrl));
                    (GetVideoCommand as Command).ChangeCanExecute();
                }
            }
        }

        public Thumbnail VideoThumbnail
        {
            get => _videoThumbnail;
            set
            {
                if (_videoThumbnail != value)
                {
                    _videoThumbnail = value;
                    OnPropertyChanged(nameof(VideoThumbnail));
                }
            }
        }

        public string VideoDuration
        {
            get => _videoDuration;
            set
            {
                if (_videoDuration != value)
                {
                    _videoDuration = value;
                    OnPropertyChanged(nameof(VideoDuration));
                }
            }
        }

        public string VideoTitle
        {
            get
            {
                return _videoTitle;
            }

            set
            {
                if (value != _videoTitle)
                {
                    _videoTitle = value;
                    OnPropertyChanged(nameof(VideoTitle));
                }
            }
        }

        public double Progress
        {
            get => _progress;
            private set
            {
                if (value != _progress)
                {
                    _progress = value;
                    ProgressReport = $"{(int)Math.Round(_progress * 100)}%";
                    OnPropertyChanged(nameof(Progress));
                }
            }
        }

        public bool IsDownloading
        {
            get => _isDownloading;
            set
            {
                if (value != _isDownloading)
                {
                    _isDownloading = value;
                    OnPropertyChanged("IsDownloading");
                }
            }
        }

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (value != _isBusy)
                {
                    _isBusy = value;
                    OnPropertyChanged("IsBusy");
                }
            }
        }

        public bool ShowThumbnail
        {
            get => _showThumbnail;

            set
            {
                if (value != _showThumbnail)
                {
                    _showThumbnail = value;
                    OnPropertyChanged(nameof(ShowThumbnail));
                }
            }
        }

        public string ProgressReport
        {
            get => _progressReport;

            set
            {
                if (value != _progressReport)
                {
                    _progressReport = value;
                    OnPropertyChanged(nameof(ProgressReport));
                }
            }
        }

        public bool IsPopupOpen
        {
            get => _isPopupOpen;

            set
            {
                _isPopupOpen = value;
                OnPropertyChanged("IsPopupOpen");
            }
        }

        public string DownloadedMessage
        {
            get => _downloadedMessage;

            set
            {
                if (value != _downloadedMessage)
                {
                    _downloadedMessage = value;
                    OnPropertyChanged(nameof(DownloadedMessage));  
                }
            }
        }

        public ICommand GetVideoCommand { private set; get; }

        public ICommand DownloadCommand { private set; get; }

        #endregion

        #region private helper

        private void Reset(bool resetAll)
        {
            IsDownloading = false;
            Progress = 0;
            IsPopupOpen = false;
            DownloadedMessage = null;

            if (resetAll)
            {
                VideoTitle = null;
                VideoDuration = null;
                Videos?.Clear();
                ShowThumbnail = false;
                IsBusy = false;
            }
        }
        private string SanitizeFileName(string fileName)
        {
            foreach (var invalidChar in Path.GetInvalidFileNameChars())
                fileName = fileName.Replace(invalidChar, '_');

            return fileName;
        }

        #endregion

    }
}
