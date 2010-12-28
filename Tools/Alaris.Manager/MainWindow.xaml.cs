using System;
using System.ServiceModel;
using System.Windows;
using System.Windows.Input;
using Alaris.Framework;
using Alaris.Irc;

namespace Alaris.Manager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : IDisposable
    {
        private string _server;
        private bool _connected;
        private string _password;
        private RemoteClient _rmc;
        private bool _disposed;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var diffHeight = Height - e.PreviousSize.Height;
            var diffWidth = Width - e.PreviousSize.Width;

            tabbedView.Width += (diffWidth);
            tabbedView.Height += (diffHeight);
        }

        private void ConnectButtonClick(object sender, RoutedEventArgs e)
        {           
            if(string.IsNullOrEmpty(serverBox.Text) || string.IsNullOrEmpty(passwordBox.Password))
            {
                MessageBox.Show("You didn't fill in the required fields.", "Error!",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                return;
            }

            try
            {
                _server = serverBox.Text;
                _password = passwordBox.Password;
                _rmc = new RemoteClient("BasicHttpBinding_IRemote",
                                           new EndpointAddress(_server));

                _rmc.Open();

                if(!_rmc.Authorize(Utilities.MD5String(_password)))
                {
                    MessageBox.Show("Invalid password entered!", "Error!", MessageBoxButton.OK,
                                    MessageBoxImage.Error);

                    _rmc.Close();

                    return;
                }



            }
            catch(Exception x)
            {
                MessageBox.Show(string.Format("Something unexpexted happened! Please report this to devs!{0}{1}---------------------------------------------------------------------------{2}{3}", 
                    Environment.NewLine, 
                    Environment.NewLine, 
                    Environment.NewLine, 
                    x), "Error!",   MessageBoxButton.OK,
                                    MessageBoxImage.Error);

                _rmc.Close();

                return;
            }

            messageTab.IsEnabled = true;

            MessageBox.Show("Connection successful!", "Success!", MessageBoxButton.OK, MessageBoxImage.Information);
            _connected = true;
        }

        private void SendButtonClick(object sender, RoutedEventArgs e)
        {
            if (!_connected)
                return;

            var chan = channelBox.Text;
            var msg = messageBox.Text;

            if(string.IsNullOrEmpty(chan) || string.IsNullOrEmpty(msg))
            {
                MessageBox.Show("You didn't fill in the required fields.", "Error!",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                return;
            }

            if (!chan.StartsWith("#"))
                chan = string.Format("#{0}", chan);

            if(!Rfc2812Util.IsValidChannelName(chan))
            {
                MessageBox.Show("Invalid channel name!", "Error!",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                return;
            }

            _rmc.PublicMessage(chan, msg);

            MessageBox.Show("Message sent!", "Success!", MessageBoxButton.OK,
                                    MessageBoxImage.Asterisk);

            messageBox.Text = string.Empty;

        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (!_disposed)
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if(_rmc.State != CommunicationState.Closed)
                    _rmc.Close(); 
            }

            _disposed = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            messageTab.IsEnabled = false;
        }

        private void MessageBoxKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                SendButtonClick(sender, new RoutedEventArgs());
            }
        }

        
    }
}
