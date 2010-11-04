using System;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.Windows;
using Alaris.Administration;
using Alaris.Irc;

namespace Alaris.Manager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private HttpChannel _channel;
        private RemoteManager _manager;
        private string _server;
        private bool _connected;
        private string _password;
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
                _channel = new HttpChannel();
                _server = serverBox.Text;
                _password = passwordBox.Password;
                ChannelServices.RegisterChannel(_channel, false);

                _manager = (RemoteManager) Activator.GetObject(typeof (RemoteManager), _server);

                if (_manager == null)
                {
                    MessageBox.Show("Couldn't retrieve the instance from the given server!", "Error!",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);

                    ChannelServices.UnregisterChannel(_channel);

                    return;
                }

                if (!_manager.Initialize(passwordBox.Password))
                {
                    MessageBox.Show("Invalid password entered!", "Error!", MessageBoxButton.OK,
                                    MessageBoxImage.Error);

                    ChannelServices.UnregisterChannel(_channel);

                    return;
                }

                _connected = true;
                

                MessageBox.Show("Connection successful!", "Success!", MessageBoxButton.OK,
                                    MessageBoxImage.Asterisk);
            }
            catch(Exception x)
            {
                MessageBox.Show(string.Format("Something unexpexted happened! Please report this to devs!{0}{1}---------------------------------------------------------------------------{2}{3}", 
                    Environment.NewLine, 
                    Environment.NewLine, 
                    Environment.NewLine, 
                    x), "Error!",   MessageBoxButton.OK,
                                    MessageBoxImage.Error);

                ChannelServices.UnregisterChannel(_channel);

                return;
            }
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

            _manager.PublicMessage(chan, msg);

            MessageBox.Show("Message sent!", "Success!", MessageBoxButton.OK,
                                    MessageBoxImage.Asterisk);

            messageBox.Text = string.Empty;

        }
    }
}
