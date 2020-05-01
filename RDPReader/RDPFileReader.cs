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
using System.IO;
using System.Text.RegularExpressions;

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

        private string[] _rdpTemplate = {
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

        private WindowsPosition _winPosStr;
        private int _disableCursorSettings = 0;

        #endregion

        #endregion

        #region properties

        public int ScreenMode { get; set; }

        public int DesktopWidth { get; set; }

        public int DesktopHeight { get; set; }

        public SessionBPPs SessionBPP { get; set; }

        public WindowsPosition WinPosStr
        {
            get => _winPosStr;
            set => _winPosStr = value;
        }

        public string FullAddress { get; set; }

        public int Compression { get; set; }

        public KeyboardHooks KeyboardHook { get; set; }

        public AudioModes AudioMode { get; set; }

        public int RedirectDrives { get; set; }

        public int RedirectPrinters { get; set; }

        public int RedirectComPorts { get; set; }

        public int RedirectSmartCards { get; set; }

        public int DisplayConnectionBar { get; set; }

        public int AutoReconnectionEnabled { get; set; }

        public string Username { get; set; }

        public string Domain { get; set; }

        public string AlternateShell { get; set; }

        public string ShellWorkingDirectory { get; set; }

        public string Password { get; set; }

        public int DisableWallpaper { get; set; }

        public int DisableFullWindowDrag { get; set; }

        public int DisableMenuAnims { get; set; }

        public int DisableThemes { get; set; }

        public int DisableCursorSettings
        {
            get
            {
                return _disableCursorSettings;
            }
            set
            {
                DisplayConnectionBar = value;
            }
        }

        public int BitmapCachePersistEnable { get; set; }

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
                            ScreenMode = int.Parse(v);
                            break;

                        case "desktopwidth":
                            DesktopWidth = int.Parse(v);
                            break;

                        case "desktopheight":
                            DesktopHeight = int.Parse(v);
                            break;

                        case "session bpp":
                            SessionBPP = (SessionBPPs)int.Parse(v);
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
                            FullAddress = v;
                            break;

                        case "compression":
                            Compression = int.Parse(v);
                            break;

                        case "keyboardhook":
                            KeyboardHook = (KeyboardHooks)int.Parse(v);
                            break;

                        case "audiomode":
                            AudioMode = (AudioModes)int.Parse(v);
                            break;

                        case "redirectdrives":
                            RedirectDrives = int.Parse(v);
                            break;

                        case "redirectprinters":
                            RedirectPrinters = int.Parse(v);
                            break;

                        case "redirectcomports":
                            RedirectComPorts = int.Parse(v);
                            break;

                        case "redirectsmartcards":
                            RedirectSmartCards = int.Parse(v);
                            break;

                        case "displayconnectionbar":
                            DisplayConnectionBar = int.Parse(v);
                            break;

                        case "autoreconnection enabled":
                            AutoReconnectionEnabled = int.Parse(v);
                            break;

                        case "username":
                            Username = v;
                            break;

                        case "domain":
                            Domain = v;
                            break;

                        case "alternate shell":
                            AlternateShell = v;
                            break;

                        case "shell working directory":
                            ShellWorkingDirectory = v;
                            break;

                        case "password 51":
                            Password = v;
                            break;

                        case "disable wallpaper":
                            DisableWallpaper = int.Parse(v);
                            break;

                        case "disable full window drag":
                            DisableFullWindowDrag = int.Parse(v);
                            break;

                        case "disable menu anims":
                            DisableMenuAnims = int.Parse(v);
                            break;

                        case "disable themes":
                            DisableThemes = int.Parse(v);
                            break;

                        case "disable cursor setting":
                            _disableCursorSettings = int.Parse(v);
                            break;

                        case "bitmapcachepersistenable":
                            BitmapCachePersistEnable = int.Parse(v);
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

            string template = string.Join("\r\n", _rdpTemplate);
            string data = string.Format(template,
                ScreenMode,
                DesktopWidth,
                DesktopHeight,
                (int)SessionBPP,
                (int)_winPosStr.WinState, _winPosStr.Rect.Top, _winPosStr.Rect.Left, _winPosStr.Rect.Width, _winPosStr.Rect.Height,
                FullAddress,
                Compression,
                (int)KeyboardHook,
                (int)AudioMode,
                RedirectDrives,
                RedirectPrinters,
                RedirectComPorts,
                RedirectSmartCards,
                DisplayConnectionBar,
                AutoReconnectionEnabled,
                Username,
                Domain,
                AlternateShell,
                ShellWorkingDirectory,
                Password,
                DisableWallpaper,
                DisableFullWindowDrag,
                DisableMenuAnims,
                DisableThemes,
                _disableCursorSettings,
                BitmapCachePersistEnable
            );

            using (StreamWriter writer = new StreamWriter(filepath))
            {
                writer.Write(data);
            }
        }

        #endregion
    }
}