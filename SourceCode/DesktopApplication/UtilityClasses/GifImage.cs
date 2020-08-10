using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

// ReSharper disable All

namespace TvSeriesCalendar.UtilityClasses
{
    class GifImage : Image
    {
        public static readonly DependencyProperty FrameIndexProperty =
            DependencyProperty.Register("FrameIndex", typeof(int), typeof(GifImage),
                new UIPropertyMetadata(0, new PropertyChangedCallback(ChangingFrameIndex)));

        public static readonly DependencyProperty AutoStartProperty =
            DependencyProperty.Register("AutoStart", typeof(bool), typeof(GifImage),
                new UIPropertyMetadata(false, AutoStartPropertyChanged));

        public static readonly DependencyProperty GifSourceProperty =
            DependencyProperty.Register("GifSource", typeof(string), typeof(GifImage),
                new UIPropertyMetadata(string.Empty, GifSourcePropertyChanged));

        private Int32Animation _animation;
        private GifBitmapDecoder _gifDecoder;
        private bool _isInitialized;

        static GifImage()
        {
            VisibilityProperty.OverrideMetadata(typeof(GifImage),
                new FrameworkPropertyMetadata(VisibilityPropertyChanged));
        }

        public int FrameIndex
        {
            get => (int) GetValue(FrameIndexProperty);
            set => SetValue(FrameIndexProperty, value);
        }

        /// <summary>
        ///     Defines whether the animation starts on it's own
        /// </summary>
        public bool AutoStart
        {
            get => (bool) GetValue(AutoStartProperty);
            set => SetValue(AutoStartProperty, value);
        }

        public string GifSource
        {
            get => (string) GetValue(GifSourceProperty);
            set => SetValue(GifSourceProperty, value);
        }

        private void Initialize()
        {
            _gifDecoder = new GifBitmapDecoder(new Uri("pack://application:,,," + this.GifSource),
                BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            _animation = new Int32Animation(0,
                _gifDecoder.Frames.Count - 1,
                new Duration(new TimeSpan(0,
                    0,
                    0,
                    _gifDecoder.Frames.Count / 30,
                    (int) ((_gifDecoder.Frames.Count / 30.0 - _gifDecoder.Frames.Count / 30) * 1000))));
            _animation.RepeatBehavior = RepeatBehavior.Forever;
            this.Source = _gifDecoder.Frames[0];

            _isInitialized = true;
        }

        private static void VisibilityPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if ((Visibility) e.NewValue == Visibility.Visible)
            {
                ((GifImage) sender).StartAnimation();
            }
            else
            {
                ((GifImage) sender).StopAnimation();
            }
        }

        static void ChangingFrameIndex(DependencyObject obj, DependencyPropertyChangedEventArgs ev)
        {
            var gifImage = obj as GifImage;
            gifImage.Source = gifImage._gifDecoder.Frames[(int) ev.NewValue];
        }

        private static void AutoStartPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool) e.NewValue)
                (sender as GifImage).StartAnimation();
        }

        private static void GifSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as GifImage).Initialize();
        }

        /// <summary>
        ///     Starts the animation
        /// </summary>
        public void StartAnimation()
        {
            if (!_isInitialized)
                this.Initialize();

            BeginAnimation(FrameIndexProperty, _animation);
        }

        /// <summary>
        ///     Stops the animation
        /// </summary>
        public void StopAnimation()
        {
            BeginAnimation(FrameIndexProperty, null);
        }
    }
}