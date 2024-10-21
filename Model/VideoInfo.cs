using System.ComponentModel;
using YoutubeExplode.Videos.Streams;

namespace YoutubeDownloader.Model
{
    public class VideoInfo : INotifyPropertyChanged
    {
        #region Fields

        private string _title;
        private string _resolution;
        private string _videoSize;
        private string _format;

        private IStreamInfo _videoStream;
        private IStreamInfo _audioStream;

        #endregion
        public string Title 
        {
            get
            {
                return _title;
            }

            set
            {
                if( _title != value ) 
                {
                    _title = value;
                    RaisePropertyChanged("Title" );
                }
            }
        }

        public string  Resolution 
        {
            get
            {
                return _resolution;
            }

            set
            {
                if ( _resolution != value ) 
                {
                    _resolution = value;
                    RaisePropertyChanged("Resolution");
                }
            }
        }

        public string VideoSize 
        {
            get
            {
                return _videoSize;
            }

            set
            {
                if (_videoSize != value )
                {
                    _videoSize = value;
                    RaisePropertyChanged("VideoSize");
                }
            }
        }

        public string Format 
        { 
            get
            {
                return _format;
            }

            set
            {
                if(_format != value )
                {
                    _format = value;
                    RaisePropertyChanged("Format");
                }
            }
        }

        public IStreamInfo VideoStream
        {
            get
            {
                return _videoStream;
            }

            set
            {
                if (_videoStream != value)
                {
                    _videoStream = value;
                    RaisePropertyChanged("VideoStream");
                }
            }
        }

        public IStreamInfo AudioStream
        {
            get
            {
                return _audioStream;
            }

            set
            {
                if (_audioStream != value)
                {
                    _audioStream = value;
                }
            }
        }


        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(String name)
        {
            if (PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}
