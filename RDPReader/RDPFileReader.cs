/*
    Author: Jayson Ragasa | aka: Nullstring
    Application Developer - Anomalist Designs LLC
 *  ---
 *  RDPFileReader 1.0
 *  
 *  RDP File Settings - http://dev.remotenetworktechnology.com/ts/rdpfile.htm
 *  Terminal Services Team Blog - http://blogs.msdn.com/ts/archive/2008/09/02/specifying-the-ts-client-start-location-on-the-virtual-desktop.aspx
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace RDPFileReader
{
    public class RDPFile
    {
        #region enum

        public enum KeyboardHooks
        {
            ON_THE_LOCAL_COMPUTER = 0,
            ON_THE_REMOTE_COMPUTER = 1,
            IN_FULL_SCREEN_MODE_ONLY = 2
        };

        public enum AudioModes
        {
            BRING_TO_THIS_COMPUTER = 0,
            DO_NOT_PLAY = 1,
            LeAVE_AT_REMOTE_COMOPUTER = 2
        };

        public enum WindowState : int
        {
            NORMAL = 1,
            MAXMIZE = 3
        }

        public enum SessionBPPs
        {
            BPP_8 = 8,
            BPP_15 = 15,
            BPP_16 = 16,
            BPP_24 = 24
        }

        #endregion

        #region structs

        public struct RECT
        {
            public int Top;
            public int Left;
            public int Width;
            public int Height;
        }

        public struct WindowsPosition
        {
            public WindowState WinState;
            public RECT Rect;
        }

        #endregion

        #region variables

        private string _filename = string.Empty;

        #region RDP template
        string[] _rdpTemplate = {
                                    "screen mode id:i:{0}",
                                    "desktopwidth:i:{1}",
                                    "desktopheight:i:{2}",
                                    "session bpp:i:{3}",
                                    "winposstr:s:0,{4},{5},{6},{7},{8}",
                                    "full address:s:{9}",
                                    "compression:i:{10}",
                                    "keyboardhook:i:{11}",
                                    "audiomode:i:{12}",
                                    "redirectdrives:i:{13}",
                                    "redirectprinters:i:{14}",
                                    "redirectcomports:i:{15}",
                                    "redirectsmartcards:i:{16}",
                                    "displayconnectionbar:i:{17}",
                                    "autoreconnection enabled:i:{18}",
                                    "username:s:{19}",
                                    "domain:s:{20}",
                                    "alternate shell:s:{21}",
                                    "shell working directory:s:{22}",
                                    "password 51:b:{23}",
                                    "disable wallpaper:i:{24}",
                                    "disable full window drag:i:{25}",
                                    "disable menu anims:i:{26}",
                                    "disable themes:i:{27}",
                                    "disable cursor setting:i:{28}",
                                    "bitmapcachepersistenable:i:{29}"
                                };
        #endregion

        #region member fields

        int _screenMode = 0;
        int _desktopWidth = 0;
        int _desktopHeight = 0;
        SessionBPPs _sessionBPP = 0;
        WindowsPosition _winPosStr;
        string _fullAddress = string.Empty;
        int _compression = 0;
        KeyboardHooks _keyboardHook = 0;
        AudioModes _audiomode = 0;
        int _redirectDrives = 0;
        int _redirectPrinters = 0;
        int _redirectComPorts = 0;
        int _redirectSmartCards = 0;
        int _displayConnectionBar = 0;
        int _autoReconnectionEnabled = 0;
        string _username = string.Empty;
        string _domain = string.Empty;
        string _alternateShell = string.Empty;
        string _shellWorkingDirectory = string.Empty;
        string _password = string.Empty;
        int _disableWallpaper = 0;
        int _disableFullWindowDrag = 0;
        int _disableMenuAnims = 0;
        int _disableThemes = 0;
        int _disableCursorSettings = 0;
        int _bitmapCachePersistEnable = 0;

        #endregion

        #endregion

        #region properties

        public int ScreenMode
        {
            get
            {
                return _screenMode;
            }
            set
            {
                _screenMode = value;
            }
        }

        public int DesktopWidth
        {
            get
            {
                return _desktopWidth;
            }
            set
            {
                _desktopWidth = value;
            }
        }

        public int DesktopHeight
        {
            get
            {
                return _desktopHeight;
            }
            set
            {
                _desktopHeight = value;
            }
        }

        public SessionBPPs SessionBPP
        {
            get
            {
                return _sessionBPP;
            }
            set
            {
                _sessionBPP = value;
            }
        }

        public WindowsPosition WinPosStr
        {
            get
            {
                return _winPosStr;
            }
            set
            {
                _winPosStr = value;
            }
        }

        public string FullAddress
        {
            get
            {
                return _fullAddress;
            }
            set
            {
                _fullAddress = value;
            }
        }

        public int Compression
        {
            get
            {
                return _compression;
            }
            set
            {
                _compression = value;
            }
        }

        public KeyboardHooks KeyboardHook
        {
            get
            {
                return _keyboardHook;
            }
            set
            {
                _keyboardHook = value;
            }
        }

        public AudioModes AudioMode
        {
            get
            {
                return _audiomode;
            }
            set
            {
                _audiomode = value;
            }
        }

        public int RedirectDrives
        {
            get
            {
                return _redirectDrives;
            }
            set
            {
                _redirectDrives = value;
            }
        }

        public int RedirectPrinters
        {
            get
            {
                return _redirectPrinters;
            }
            set
            {
                _redirectPrinters = value;
            }
        }

        public int RedirectComPorts
        {
            get
            {
                return _redirectComPorts;
            }
            set
            {
                _redirectComPorts = value;
            }
        }

        public int RedirectSmartCards
        {
            get
            {
                return _redirectSmartCards;
            }
            set
            {
                _redirectSmartCards = value;
            }
        }

        public int DisplayConnectionBar
        {
            get
            {
                return _displayConnectionBar;
            }
            set
            {
                _displayConnectionBar = value;
            }
        }

        public int AutoReconnectionEnabled
        {
            get
            {
                return _autoReconnectionEnabled;
            }
            set
            {
                _autoReconnectionEnabled = value;
            }
        }

        public string Username
        {
            get
            {
                return _username;
            }
            set
            {
                _username = value;
            }
        }

        public string Domain
        {
            get
            {
                return _domain;
            }
            set
            {
                _domain = value;
            }
        }

        public string AlternateShell
        {
            get
            {
                return _alternateShell;
            }
            set
            {
                _alternateShell = value;
            }
        }

        public string ShellWorkingDirectory
        {
            get
            {
                return _shellWorkingDirectory;
            }
            set
            {
                _shellWorkingDirectory = value;
            }
        }

        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
            }
        }

        public int DisableWallpaper
        {
            get
            {
                return _disableWallpaper;
            }
            set
            {
                _disableWallpaper = value;
            }
        }

        public int DisableFullWindowDrag
        {
            get
            {
                return _disableFullWindowDrag;
            }
            set
            {
                _disableFullWindowDrag = value;
            }
        }

        public int DisableMenuAnims
        {
            get
            {
                return _disableMenuAnims;
            }
            set
            {
                _disableMenuAnims = value;
            }
        }

        public int DisableThemes
        {
            get
            {
                return _disableThemes;
            }
            set
            {
                _disableThemes = value;
            }
        }

        public int DisableCursorSettings
        {
            get
            {
                return _disableCursorSettings;
            }
            set
            {
                _displayConnectionBar = value;
            }
        }

        public int BitmapCachePersistEnable
        {
            get
            {
                return _bitmapCachePersistEnable;
            }
            set
            {
                _bitmapCachePersistEnable = value;
            }
        }

        #endregion

        #region methods

        public void Read(string filepath)
        {
            _filename = filepath;

            string data = string.Empty;

            using (StreamReader reader = new StreamReader(filepath))
            {
                data = reader.ReadToEnd();
            }

            string[] settings = data.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string thisSetting in settings)
            {
                string regex = "(?<type>.*)\\:(?<dtype>\\w)\\:(?<value>.*)";
                
                RegexOptions options = ((RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline) | RegexOptions.IgnoreCase);
                Regex reg = new Regex(regex, options);

                if (reg.IsMatch(thisSetting))
                {
                    Match m = reg.Match(thisSetting);

                    string v = m.Groups["value"].Value;

                    switch (m.Groups["type"].Value)
                    {
                        case "screen mode id":
                            _screenMode = int.Parse(v);
                            break;

                        case "desktopwidth":
                            _desktopWidth = int.Parse(v);
                            break;

                        case "desktopheight":
                            _desktopHeight = int.Parse(v);
                            break;

                        case "session bpp":
                            _sessionBPP = (SessionBPPs)int.Parse(v);
                            break;

                        case "winposstr":
                            string[] vals = v.Split(',');

                            _winPosStr.WinState = (WindowState)int.Parse(vals[1]);

                            _winPosStr.Rect.Top = int.Parse(vals[2]);
                            _winPosStr.Rect.Left = int.Parse(vals[3]);
                            _winPosStr.Rect.Width = int.Parse(vals[4]);
                            _winPosStr.Rect.Height = int.Parse(vals[5]);

                            break;

                        case "full address":
                            _fullAddress = v;
                            break;
                            
                        case "compression":
                            _compression = int.Parse(v);
                            break;

                        case "keyboardhook":
                            _keyboardHook = (KeyboardHooks)int.Parse(v);
                            break;

                        case "audiomode":
                            _audiomode = (AudioModes)int.Parse(v);
                            break;

                        case "redirectdrives":
                            _redirectDrives = int.Parse(v);
                            break;

                        case "redirectprinters":
                            _redirectPrinters = int.Parse(v);
                            break;

                        case "redirectcomports":
                            _redirectComPorts = int.Parse(v);
                            break;

                        case "redirectsmartcards":
                            _redirectSmartCards = int.Parse(v);
                            break;

                        case "displayconnectionbar":
                            _displayConnectionBar = int.Parse(v);
                            break;

                        case "autoreconnection enabled":
                            _autoReconnectionEnabled = int.Parse(v);
                            break;

                        case "username":
                            _username = v;
                            break;

                        case "domain":
                            _domain = v;
                            break;

                        case "alternate shell":
                            _alternateShell = v;
                            break;

                        case "shell working directory":
                            _shellWorkingDirectory = v;
                            break;

                        case "password 51":
                            _password = v;
                            break;

                        case "disable wallpaper":
                            _disableWallpaper = int.Parse(v);
                            break;

                        case "disable full window drag":
                            _disableFullWindowDrag = int.Parse(v);
                            break;

                        case "disable menu anims":
                            _disableMenuAnims = int.Parse(v);
                            break;

                        case "disable themes":
                            _disableThemes = int.Parse(v);
                            break;

                        case "disable cursor setting":
                            _disableCursorSettings = int.Parse(v);
                            break;

                        case "bitmapcachepersistenable":
                            _bitmapCachePersistEnable = int.Parse(v);
                            break;
                    }
                }
            }
        }

        public void Update()
        {
            Save(_filename);
        }

        public void Save(string filepath)
        {
            _filename = filepath;

            string template = string.Empty;

            foreach (string temp in _rdpTemplate)
            {
                template += temp + "\r\n";
            }

            string data = string.Format(template,
                _screenMode,
                _desktopWidth,
                _desktopHeight,
                (int)_sessionBPP,
                (int)_winPosStr.WinState, _winPosStr.Rect.Top, _winPosStr.Rect.Left, _winPosStr.Rect.Width, _winPosStr.Rect.Height,
                _fullAddress,
                _compression,
                (int)_keyboardHook,
                (int)_audiomode,
                _redirectDrives,
                _redirectPrinters,
                _redirectComPorts,
                _redirectSmartCards,
                _displayConnectionBar,
                _autoReconnectionEnabled,
                _username,
                _domain,
                _alternateShell,
                _shellWorkingDirectory,
                _password,
                _disableWallpaper,
                _disableFullWindowDrag,
                _disableMenuAnims,
                _disableThemes,
                _disableCursorSettings,
                _bitmapCachePersistEnable
            );

            using (StreamWriter writer = new StreamWriter(filepath))
            {
                writer.Write(data);
            }
        }

        #endregion
    }
}
