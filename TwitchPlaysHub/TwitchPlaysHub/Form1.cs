using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Windows;
using WindowsInput;
using WindowsInput.Native;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Specialized;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace TwitchPlaysHub
{
    public partial class Form1 : Form
    {
        ITwitchClient twitchClient = null;
        IMessageParser messageParser = null;
        PingSender ping = null;
        //AutoReconnect autoconnect = null;
        Process[] processlist = null;
        Boolean PressedConnect = false;
        Boolean truestop = false;
        Boolean activechat = true;
        Boolean autoreconnecting = false;
        Boolean chatdisabled = false;
        Int32 activechattick = 0;
        Int32 timeschatted = 0;
        IntPtr selectedgamewindow;
        List<string> commandorderlist = new List<string> { };
        List<string> multiplecommands = new List<string> { };
        List<Int32> multiplecommandtimers = new List<Int32> { };
        List<string> multiplecommandtypes = new List<string> { };
        List<string> heldkeys = new List<string> { };
        System.Timers.Timer activitytimer = new System.Timers.Timer();
        System.Timers.Timer connectiontimer = new System.Timers.Timer();
        List<System.Timers.Timer> remindertimers = new List<System.Timers.Timer> { };
        /// <summary> 
        /// ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// </summary>
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.CommandList != null)
            {
                MasterControlList.Items.Clear();
                foreach (string item in Properties.Settings.Default.CommandList)
                {
                    MasterControlList.Items.Add(item);
                }
            }
            if (Properties.Settings.Default.TwitchModList != null)
            {
                ModList.Items.Clear();
                foreach (string item in Properties.Settings.Default.TwitchModList)
                {
                    ModList.Items.Add(item);
                }
            }
            if (Properties.Settings.Default.TwitchLurkList != null)
            {
                LurkList.Items.Clear();
                foreach (string item in Properties.Settings.Default.TwitchLurkList)
                {
                    LurkList.Items.Add(item);
                }
            }
            if (Properties.Settings.Default.RemindersList != null)
            {
                RemindersListBox.Items.Clear();
                foreach (string item in Properties.Settings.Default.RemindersList)
                {
                    RemindersListBox.Items.Add(item);
                }
                UpdateControlTypeBox();
            }
            if (Properties.Settings.Default.TwitchUsername != null)
            {
                TwitchUsernameTextbox.Text = Properties.Settings.Default.TwitchUsername;
            }
            if (Properties.Settings.Default.TwitchChannelName != null)
            {
                TwitchChannelNameTextbox.Text = Properties.Settings.Default.TwitchChannelName;
            }
            if (Properties.Settings.Default.TwitchOAuthCode != null)
            {
                TwitchOAuthTextbox.Text = Properties.Settings.Default.TwitchOAuthCode;
            }
            if (Properties.Settings.Default.TwitchChatClient != null)
            {
                TwitchChatClientChoice.Text = Properties.Settings.Default.TwitchChatClient;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            if (twitchClient != null)
            {
                twitchClient.Disconnect();
            }

            base.OnClosed(e);
        }

        //int Tempint = 0;
        public void SendDirectInput(string type, short keycd)
        {
            
            MasterDirectInput MDI = new MasterDirectInput();
            //Console.WriteLine(Tempint);
            if (type == "KeyDown")
            {
                MDI.Send_Key(keycd, 0);
                //MDI.Send_Key(Convert.ToInt16(Tempint), 0);
            }
            else if (type == "KeyUp")
            {
                MDI.Send_Key(keycd, 2);
                //MDI.Send_Key(Convert.ToInt16(Tempint), 2);
                //Tempint += 1;
            }
            
            
        }
        /*
        new Input
            {

            }
    public void SendDirectInput()
        {
            Input[] inputs = new Input[]
        {
            new Input
            {
                type = (int)InputType.Keyboard,
                u = new InputUnion
                {
                    ki = new KEYBDInput
                    {
                        wVk = 0,
                        wScan = 0x11, // W
                        dwFlags = (uint)(KeyEventF.KeyDown | KeyEventF.Scancode),
                        dwExtraInfo = GetMessageExtraInfo()
                    }
                }
            }
            new MasterDirectInput
            
            {
                type = (int)InputType.Keyboard,
                u = new InputUnion
                {
                    ki = new KeyboardInput
                    {
                        wVk = 0,
                        wScan = 0x11, // W
                        dwFlags = (uint)(KeyEventF.KeyUp | KeyEventF.Scancode),
                        dwExtraInfo = GetMessageExtraInfo()
                    }
                }
            }
        };

                    SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(Input)));
        }
        */
        public short ConvertTextToShortKeyCode(string textinput)
        {
            //----------------------------------------letters
            if (textinput == "A")
            {
                return Convert.ToInt16(30); //a
            }
            else if (textinput == "B")
            {
                return Convert.ToInt16(48);
            }
            else if (textinput == "C")
            {
                return Convert.ToInt16(46);
            }
            else if (textinput == "D")
            {
                return Convert.ToInt16(32);
            }
            else if (textinput == "E")
            {
                return Convert.ToInt16(18);
            }
            else if (textinput == "F")
            {
                return Convert.ToInt16(33);
            }
            else if (textinput == "G")
            {
                return Convert.ToInt16(34);
            }
            else if (textinput == "H")
            {
                return Convert.ToInt16(35);
            }
            else if (textinput == "I")
            {
                return Convert.ToInt16(23);
            }
            else if (textinput == "J")
            {
                return Convert.ToInt16(36);
            }
            else if (textinput == "K")
            {
                return Convert.ToInt16(37);
            }
            else if (textinput == "L")
            {
                return Convert.ToInt16(38);
            }
            else if (textinput == "M")
            {
                return Convert.ToInt16(50);
            }
            else if (textinput == "N")
            {
                return Convert.ToInt16(49);
            }
            else if (textinput == "O")
            {
                return Convert.ToInt16(24);
            }
            else if (textinput == "P")
            {
                return Convert.ToInt16(25);
            }
            else if (textinput == "Q")
            {
                return Convert.ToInt16(16);
            }
            else if (textinput == "R")
            {
                return Convert.ToInt16(19);
            }
            else if (textinput == "S")
            {
                return Convert.ToInt16(31);
            }
            else if (textinput == "T")
            {
                return Convert.ToInt16(20);
            }
            else if (textinput == "U")
            {
                return Convert.ToInt16(22);
            }
            else if (textinput == "V")
            {
                return Convert.ToInt16(47);
            }
            else if (textinput == "W")
            {
                return Convert.ToInt16(17);
            }
            else if (textinput == "X")
            {
                return Convert.ToInt16(45);
            }
            else if (textinput == "Y")
            {
                return Convert.ToInt16(21);
            }
            else if (textinput == "Z")
            {
                return Convert.ToInt16(44);
            }
            //---------------------------------------------------------num
            else if (textinput == "D0")
            {
                return Convert.ToInt16(11);
            }
            else if (textinput == "D1")
            {
                return Convert.ToInt16(2);
            }
            else if (textinput == "D2")
            {
                return Convert.ToInt16(3);
            }
            else if (textinput == "D3")
            {
                return Convert.ToInt16(4);
            }
            else if (textinput == "D4")
            {
                return Convert.ToInt16(5);
            }
            else if (textinput == "D5")
            {
                return Convert.ToInt16(6);
            }
            else if (textinput == "D6")
            {
                return Convert.ToInt16(7);
            }
            else if (textinput == "D7")
            {
                return Convert.ToInt16(8);
            }
            else if (textinput == "D8")
            {
                return Convert.ToInt16(9);
            }
            else if (textinput == "D9")
            {
                return Convert.ToInt16(10);
            }
            //--------------------------------------cntrls
            else if (textinput == "Up")
            {
                return Convert.ToInt16(200);
            }
            else if (textinput == "Down")
            {
                return Convert.ToInt16(208);
            }
            else if (textinput == "Left")
            {
                return Convert.ToInt16(203);
            }
            else if (textinput == "Right")
            {
                return Convert.ToInt16(205);
            }
            else if (textinput == "Abnt_C1")
            {
                return Convert.ToInt16(115);
            }
            else if (textinput == "Abnt_C2")
            {
                return Convert.ToInt16(126);
            }
            else if (textinput == "Add")
            {
                return Convert.ToInt16(78);
            }
            else if (textinput == "Apostrophe")
            {
                return Convert.ToInt16(40);
            }
            else if (textinput == "Apps")
            {
                return Convert.ToInt16(221);
            }
            else if (textinput == "At")
            {
                return Convert.ToInt16(145);
            }
            else if (textinput == "Ax")
            {
                return Convert.ToInt16(150);
            }
            else if (textinput == "Back")
            {
                return Convert.ToInt16(14);
            }
            else if (textinput == "Backslash")
            {
                return Convert.ToInt16(43);
            }
            else if (textinput == "Calculator")
            {
                return Convert.ToInt16(161);
            }
            else if (textinput == "Capital")
            {
                return Convert.ToInt16(58);
            }
            else if (textinput == "Colon")
            {
                return Convert.ToInt16(146);
            }
            else if (textinput == "Comma")
            {
                return Convert.ToInt16(51);
            }
            else if (textinput == "Convert")
            {
                return Convert.ToInt16(121);
            }
            else if (textinput == "Decimal")
            {
                return Convert.ToInt16(83);
            }
            else if (textinput == "Delete")
            {
                return Convert.ToInt16(211);
            }
            else if (textinput == "Divide")
            {
                return Convert.ToInt16(181);
            }
            else if (textinput == "Equals")
            {
                return Convert.ToInt16(13);
            }
            else if (textinput == "Escape")
            {
                return Convert.ToInt16(1);
            }
            else if (textinput == "F1")
            {
                return Convert.ToInt16(59);
            }
            else if (textinput == "F2")
            {
                return Convert.ToInt16(60);
            }
            else if (textinput == "F3")
            {
                return Convert.ToInt16(61);
            }
            else if (textinput == "F4")
            {
                return Convert.ToInt16(62);
            }
            else if (textinput == "F5")
            {
                return Convert.ToInt16(63);
            }
            else if (textinput == "F6")
            {
                return Convert.ToInt16(64);
            }
            else if (textinput == "F7")
            {
                return Convert.ToInt16(65);
            }
            else if (textinput == "F8")
            {
                return Convert.ToInt16(66);
            }
            else if (textinput == "F9")
            {
                return Convert.ToInt16(67);
            }
            else if (textinput == "F10")
            {
                return Convert.ToInt16(68);
            }
            else if (textinput == "F11")
            {
                return Convert.ToInt16(87);
            }
            else if (textinput == "F12")
            {
                return Convert.ToInt16(88);
            }
            else if (textinput == "F13")
            {
                return Convert.ToInt16(100);
            }
            else if (textinput == "F14")
            {
                return Convert.ToInt16(101);
            }
            else if (textinput == "F15")
            {
                return Convert.ToInt16(102);
            }
            else if (textinput == "Grave")
            {
                return Convert.ToInt16(41);
            }
            else if (textinput == "Home")
            {
                return Convert.ToInt16(199);
            }
            else if (textinput == "End")
            {
                return Convert.ToInt16(207);
            }
            else if (textinput == "Insert")
            {
                return Convert.ToInt16(210);
            }
            else if (textinput == "Kana")
            {
                return Convert.ToInt16(112);
            }
            else if (textinput == "Kanji")
            {
                return Convert.ToInt16(148);
            }
            else if (textinput == "Mail")
            {
                return Convert.ToInt16(236);
            }
            else if (textinput == "MediaSelect")
            {
                return Convert.ToInt16(237);
            }
            else if (textinput == "MediaStop")
            {
                return Convert.ToInt16(164);
            }
            else if (textinput == "Minus")
            {
                return Convert.ToInt16(12);
            }
            else if (textinput == "Multiply")
            {
                return Convert.ToInt16(55);
            }
            else if (textinput == "Mute")
            {
                return Convert.ToInt16(160);
            }
            else if (textinput == "MyComputer")
            {
                return Convert.ToInt16(235);
            }
            else if (textinput == "NextTrack")
            {
                return Convert.ToInt16(153);
            }
            else if (textinput == "NoConvert")
            {
                return Convert.ToInt16(123);
            }
            else if (textinput == "NumLock")
            {
                return Convert.ToInt16(69);
            }
            else if (textinput == "NumPad0")
            {
                return Convert.ToInt16(82);
            }
            else if (textinput == "NumPad1")
            {
                return Convert.ToInt16(79);
            }
            else if (textinput == "NumPad2")
            {
                return Convert.ToInt16(80);
            }
            else if (textinput == "NumPad3")
            {
                return Convert.ToInt16(81);
            }
            else if (textinput == "NumPad4")
            {
                return Convert.ToInt16(75);
            }
            else if (textinput == "NumPad5")
            {
                return Convert.ToInt16(76);
            }
            else if (textinput == "NumPad6")
            {
                return Convert.ToInt16(77);
            }
            else if (textinput == "NumPad7")
            {
                return Convert.ToInt16(71);
            }
            else if (textinput == "NumPad8")
            {
                return Convert.ToInt16(72);
            }
            else if (textinput == "NumPad9")
            {
                return Convert.ToInt16(73);
            }
            else if (textinput == "NumPadComma")
            {
                return Convert.ToInt16(179);
            }
            else if (textinput == "NumPadEnter")
            {
                return Convert.ToInt16(156);
            }
            else if (textinput == "NumPadEquals")
            {
                return Convert.ToInt16(141);
            }
            else if (textinput == "Oem102" || textinput == "Oem_102")
            {
                return Convert.ToInt16(86);
            }
            else if (textinput == "Pause")
            {
                return Convert.ToInt16(197);
            }
            else if (textinput == "Period")
            {
                return Convert.ToInt16(52);
            }
            else if (textinput == "PlayPause")
            {
                return Convert.ToInt16(162);
            }
            else if (textinput == "Power")
            {
                return Convert.ToInt16(222);
            }
            else if (textinput == "PrevTrack")
            {
                return Convert.ToInt16(144);
            }
            else if (textinput == "Prior")
            {
                return Convert.ToInt16(201);
            }
            else if (textinput == "Next")
            {
                return Convert.ToInt16(209);
            }
            else if (textinput == "Return")
            {
                return Convert.ToInt16(28);
            }
            else if (textinput == "LBracket")
            {
                return Convert.ToInt16(26);
            }
            else if (textinput == "RBracket")
            {
                return Convert.ToInt16(27);
            }
            else if (textinput == "LControl")
            {
                return Convert.ToInt16(29);
            }
            else if (textinput == "RControl")
            {
                return Convert.ToInt16(157);
            }
            else if (textinput == "LMenu")
            {
                return Convert.ToInt16(56);
            }
            else if (textinput == "RMenu")
            {
                return Convert.ToInt16(184);
            }
            else if (textinput == "LShift")
            {
                return Convert.ToInt16(42);
            }
            else if (textinput == "RShift")
            {
                return Convert.ToInt16(54);
            }
            else if (textinput == "Lwin")
            {
                return Convert.ToInt16(219);
            }
            else if (textinput == "Rwin")
            {
                return Convert.ToInt16(220);
            }
            else if (textinput == "Scroll")
            {
                return Convert.ToInt16(70);
            }
            else if (textinput == "Semicolon")
            {
                return Convert.ToInt16(39);
            }
            else if (textinput == "Shift")
            {
                return Convert.ToInt16(182);
            }
            else if (textinput == "Slash")
            {
                return Convert.ToInt16(53);
            }
            else if (textinput == "Sleep")
            {
                return Convert.ToInt16(223);
            }
            else if (textinput == "Space")
            {
                return Convert.ToInt16(57);
            }
            else if (textinput == "Stop")
            {
                return Convert.ToInt16(149);
            }
            else if (textinput == "Subtract")
            {
                return Convert.ToInt16(74);
            }
            else if (textinput == "SysRq")
            {
                return Convert.ToInt16(183);
            }
            else if (textinput == "Tab")
            {
                return Convert.ToInt16(15);
            }
            else if (textinput == "Underline")
            {
                return Convert.ToInt16(147);
            }
            else if (textinput == "Unlabeled")
            {
                return Convert.ToInt16(151);
            }
            else if (textinput == "VolumeDown")
            {
                return Convert.ToInt16(174);
            }
            else if (textinput == "VolumeUp")
            {
                return Convert.ToInt16(176);
            }
            else if (textinput == "Wake")
            {
                return Convert.ToInt16(227);
            }
            else if (textinput == "WebBack" || textinput == "BrowserBack")
            {
                return Convert.ToInt16(234);
            }
            else if (textinput == "WebFavorites" || textinput == "BrowserFavorites")
            {
                return Convert.ToInt16(230);
            }
            else if (textinput == "WebForward" || textinput == "BrowserForward")
            {
                return Convert.ToInt16(233);
            }
            else if (textinput == "WebHome" || textinput == "BrowserHome")
            {
                return Convert.ToInt16(178);
            }
            else if (textinput == "WebRefresh" || textinput == "BrowserRefresh")
            {
                return Convert.ToInt16(231);
            }
            else if (textinput == "WebSearch" || textinput == "BrowserSearch")
            {
                return Convert.ToInt16(229);
            }
            else if (textinput == "WebStop" || textinput == "BrowserStop")
            {
                return Convert.ToInt16(232);
            }
            else if (textinput == "Yen")
            {
                return Convert.ToInt16(125);
            }


            /*//All below has unknwn codes from simulated input method. Terminology has been adapted for now into what is now.
            else if (textinput == "Nonconvert")
            {
                return VirtualKeyCode.NONCONVERT;
            }
            else if (textinput == "Oemtilde")
            {
                return VirtualKeyCode.ESCAPE;
            }
            
            else if (textinput == "ShiftKey")
            {
                return VirtualKeyCode.SHIFT;
            }
            else if (textinput == "ControlKey")
            {
                return VirtualKeyCode.CONTROL;
            }
            
            else if (textinput == "Menu")
            {
                return VirtualKeyCode.MENU;
            }
            
            else if (textinput == "OemMinus")
            {
                return VirtualKeyCode.OEM_MINUS;
            }
            else if (textinput == "Oemplus")
            {
                return VirtualKeyCode.OEM_PLUS;
            }
            
            else if (textinput == "OemOpenBrackets")
            {
                return VirtualKeyCode.OEM_8;
            }
            else if (textinput == "Oem1")
            {
                return VirtualKeyCode.OEM_1;
            }
            else if (textinput == "Oem1")
            {
                return VirtualKeyCode.OEM_1;
            }
            else if (textinput == "Oem2")
            {
                return VirtualKeyCode.OEM_2;
            }
            else if (textinput == "Oem3")
            {
                return VirtualKeyCode.OEM_3;
            }
            else if (textinput == "Oem4")
            {
                return VirtualKeyCode.OEM_4;
            }
            else if (textinput == "Oem5")
            {
                return VirtualKeyCode.OEM_5;
            }
            else if (textinput == "Oem6")
            {
                return VirtualKeyCode.OEM_6;
            }
            else if (textinput == "Oem7")
            {
                return VirtualKeyCode.OEM_7;
            }
            else if (textinput == "Oem8")
            {
                return VirtualKeyCode.OEM_8;
            }
            
            
            else if (textinput == "Oemcomma")
            {
                return VirtualKeyCode.OEM_COMMA;
            }
            else if (textinput == "OemPeriod")
            {
                return VirtualKeyCode.OEM_PERIOD;
            }
            else if (textinput == "OemQuestion")
            {
                return VirtualKeyCode.OEM_1;
            }
            
            
            else if (textinput == "PageUp")
            {
                return VirtualKeyCode.VOLUME_UP;
            }
            else if (textinput == "PageDown")
            {
                return VirtualKeyCode.VOLUME_DOWN;
            }
            
            
            
            
            
            
            else if (textinput == "Clear")
            {
                return VirtualKeyCode.CLEAR;
            }
            else if (textinput == "OemClear")
            {
                return VirtualKeyCode.OEM_CLEAR;
            }
            
            else if (textinput == "Insert")
            {
                return VirtualKeyCode.INSERT;
            }
            else if (textinput == "BrowserHome")
            {
                return VirtualKeyCode.BROWSER_HOME;
            }
            else if (textinput == "LaunchMail")
            {
                return VirtualKeyCode.LAUNCH_MAIL;
            }
            else if (textinput == "MediaPlayPause")
            {
                return VirtualKeyCode.MEDIA_PLAY_PAUSE;
            }
            else if (textinput == "MediaStop")
            {
                return VirtualKeyCode.MEDIA_STOP;
            }
            else if (textinput == "MediaPreviousTrack")
            {
                return VirtualKeyCode.MEDIA_PREV_TRACK;
            }
            else if (textinput == "MediaNextTrack")
            {
                return VirtualKeyCode.MEDIA_NEXT_TRACK;
            }
            
            else if (textinput == "VolumeMute")
            {
                return VirtualKeyCode.VOLUME_MUTE;
            }
            else if (textinput == "Accept")
            {
                return VirtualKeyCode.ACCEPT;
            }
            else if (textinput == "Attn")
            {
                return VirtualKeyCode.ATTN;
            }
            else if (textinput == "BrowserBack")
            {
                return VirtualKeyCode.BROWSER_BACK;
            }
            else if (textinput == "BrowserFavorites")
            {
                return VirtualKeyCode.BROWSER_FAVORITES;
            }
            else if (textinput == "BrowserForward")
            {
                return VirtualKeyCode.BROWSER_FORWARD;
            }
            else if (textinput == "BrowserRefresh")
            {
                return VirtualKeyCode.BROWSER_REFRESH;
            }
            else if (textinput == "BrowserSearch")
            {
                return VirtualKeyCode.BROWSER_SEARCH;
            }
            else if (textinput == "BrowserStop")
            {
                return VirtualKeyCode.BROWSER_STOP;
            }
            else if (textinput == "Cancel")
            {
                return VirtualKeyCode.CANCEL;
            }
            
            else if (textinput == "Final")
            {
                return VirtualKeyCode.FINAL;
            }
            else if (textinput == "Hangul")
            {
                return VirtualKeyCode.HANGUL;
            }
            else if (textinput == "Hanja")
            {
                return VirtualKeyCode.HANJA;
            }
            else if (textinput == "HELP")
            {
                return VirtualKeyCode.HELP;
            }
            else if (textinput == "Junja")
            {
                return VirtualKeyCode.JUNJA;
            }
            
            else if (textinput == "LaunchApp1")
            {
                return VirtualKeyCode.LAUNCH_APP1;
            }
            else if (textinput == "LaunchApp2")
            {
                return VirtualKeyCode.LAUNCH_APP2;
            }
            else if (textinput == "LaunchMediaSelect")
            {
                return VirtualKeyCode.LAUNCH_MEDIA_SELECT;
            }
            else if (textinput == "LButton")
            {
                return VirtualKeyCode.LBUTTON;
            }
            
            
            
            
            else if (textinput == "MButton")
            {
                return VirtualKeyCode.MBUTTON;
            }
            else if (textinput == "ModeChange")
            {
                return VirtualKeyCode.MODECHANGE;
            }
            else if (textinput == "NoName")
            {
                return VirtualKeyCode.NONAME;
            }
            
            else if (textinput == "PA1")
            {
                return VirtualKeyCode.PA1;
            }
            else if (textinput == "Packet")
            {
                return VirtualKeyCode.PACKET;
            }
            
            else if (textinput == "Play")
            {
                return VirtualKeyCode.PLAY;
            }
            else if (textinput == "Print")
            {
                return VirtualKeyCode.PRINT;
            }
            
            else if (textinput == "ProcessKey")
            {
                return VirtualKeyCode.PROCESSKEY;
            }
            else if (textinput == "RButton")
            {
                return VirtualKeyCode.RBUTTON;
            }
            
            
            
            else if (textinput == "Separator")
            {
                return VirtualKeyCode.SEPARATOR;
            }
            
            else if (textinput == "Snapshot")
            {
                return VirtualKeyCode.SNAPSHOT;
            }
            
            else if (textinput == "TAB")
            {
                return VirtualKeyCode.TAB;
            }
            else if (textinput == "XButton1")
            {
                return VirtualKeyCode.XBUTTON1;
            }
            else if (textinput == "XButton2")
            {
                return VirtualKeyCode.XBUTTON2;
            }
            else if (textinput == "Zoom")
            {
                return VirtualKeyCode.ZOOM;
            }
            //////////////////////////*/
            return Convert.ToInt16(0);
        }
        public VirtualKeyCode ConvertTextToVirtualKeyCode(string textinput)
        {
            if (textinput == "A")
            {
                return VirtualKeyCode.VK_A;
            }
            else if (textinput == "B")
            {
                return VirtualKeyCode.VK_B;
            }
            else if (textinput == "C")
            {
                return VirtualKeyCode.VK_C;
            }
            else if (textinput == "D")
            {
                return VirtualKeyCode.VK_D;
            }
            else if (textinput == "E")
            {
                return VirtualKeyCode.VK_E;
            }
            else if (textinput == "F")
            {
                return VirtualKeyCode.VK_F;
            }
            else if (textinput == "G")
            {
                return VirtualKeyCode.VK_G;
            }
            else if (textinput == "H")
            {
                return VirtualKeyCode.VK_H;
            }
            else if (textinput == "I")
            {
                return VirtualKeyCode.VK_I;
            }
            else if (textinput == "J")
            {
                return VirtualKeyCode.VK_J;
            }
            else if (textinput == "K")
            {
                return VirtualKeyCode.VK_K;
            }
            else if (textinput == "L")
            {
                return VirtualKeyCode.VK_L;
            }
            else if (textinput == "M")
            {
                return VirtualKeyCode.VK_M;
            }
            else if (textinput == "N")
            {
                return VirtualKeyCode.VK_N;
            }
            else if (textinput == "O")
            {
                return VirtualKeyCode.VK_O;
            }
            else if (textinput == "P")
            {
                return VirtualKeyCode.VK_P;
            }
            else if (textinput == "Q")
            {
                return VirtualKeyCode.VK_Q;
            }
            else if (textinput == "R")
            {
                return VirtualKeyCode.VK_R;
            }
            else if (textinput == "S")
            {
                return VirtualKeyCode.VK_S;
            }
            else if (textinput == "T")
            {
                return VirtualKeyCode.VK_T;
            }
            else if (textinput == "U")
            {
                return VirtualKeyCode.VK_U;
            }
            else if (textinput == "V")
            {
                return VirtualKeyCode.VK_V;
            }
            else if (textinput == "W")
            {
                return VirtualKeyCode.VK_W;
            }
            else if (textinput == "X")
            {
                return VirtualKeyCode.VK_X;
            }
            else if (textinput == "Y")
            {
                return VirtualKeyCode.VK_Y;
            }
            else if (textinput == "Z")
            {
                return VirtualKeyCode.VK_Z;
            }
            else if (textinput == "D0")
            {
                return VirtualKeyCode.VK_0;
            }
            else if (textinput == "D1")
            {
                return VirtualKeyCode.VK_1;
            }
            else if (textinput == "D2")
            {
                return VirtualKeyCode.VK_2;
            }
            else if (textinput == "D3")
            {
                return VirtualKeyCode.VK_3;
            }
            else if (textinput == "D4")
            {
                return VirtualKeyCode.VK_4;
            }
            else if (textinput == "D5")
            {
                return VirtualKeyCode.VK_5;
            }
            else if (textinput == "D6")
            {
                return VirtualKeyCode.VK_6;
            }
            else if (textinput == "D7")
            {
                return VirtualKeyCode.VK_7;
            }
            else if (textinput == "D8")
            {
                return VirtualKeyCode.VK_8;
            }
            else if (textinput == "D9")
            {
                return VirtualKeyCode.VK_9;
            }
            else if (textinput == "F1")
            {
                return VirtualKeyCode.F1;
            }
            else if (textinput == "F2")
            {
                return VirtualKeyCode.F2;
            }
            else if (textinput == "F3")
            {
                return VirtualKeyCode.F3;
            }
            else if (textinput == "F4")
            {
                return VirtualKeyCode.F4;
            }
            else if (textinput == "F5")
            {
                return VirtualKeyCode.F5;
            }
            else if (textinput == "F6")
            {
                return VirtualKeyCode.F6;
            }
            else if (textinput == "F7")
            {
                return VirtualKeyCode.F7;
            }
            else if (textinput == "F8")
            {
                return VirtualKeyCode.F8;
            }
            else if (textinput == "F9")
            {
                return VirtualKeyCode.F9;
            }
            else if (textinput == "F10")
            {
                return VirtualKeyCode.F10;
            }
            else if (textinput == "F11")
            {
                return VirtualKeyCode.F11;
            }
            else if (textinput == "F12")
            {
                return VirtualKeyCode.F12;
            }
            else if (textinput == "NumPad0")
            {
                return VirtualKeyCode.NUMPAD0;
            }
            else if (textinput == "NumPad1")
            {
                return VirtualKeyCode.NUMPAD1;
            }
            else if (textinput == "NumPad2")
            {
                return VirtualKeyCode.NUMPAD2;
            }
            else if (textinput == "NumPad3")
            {
                return VirtualKeyCode.NUMPAD3;
            }
            else if (textinput == "NumPad4")
            {
                return VirtualKeyCode.NUMPAD4;
            }
            else if (textinput == "NumPad4")
            {
                return VirtualKeyCode.NUMPAD4;
            }
            else if (textinput == "NumPad5")
            {
                return VirtualKeyCode.NUMPAD5;
            }
            else if (textinput == "NumPad6")
            {
                return VirtualKeyCode.NUMPAD6;
            }
            else if (textinput == "NumPad7")
            {
                return VirtualKeyCode.NUMPAD7;
            }
            else if (textinput == "NumPad8")
            {
                return VirtualKeyCode.NUMPAD8;
            }
            else if (textinput == "NumPad9")
            {
                return VirtualKeyCode.NUMPAD9;
            }
            else if (textinput == "Up")
            {
                return VirtualKeyCode.UP;
            }
            else if (textinput == "Down")
            {
                return VirtualKeyCode.DOWN;
            }
            else if (textinput == "Left")
            {
                return VirtualKeyCode.LEFT;
            }
            else if (textinput == "Right")
            {
                return VirtualKeyCode.RIGHT;
            }
            else if (textinput == "Escape")
            {
                return VirtualKeyCode.ESCAPE;
            }
            else if (textinput == "Oemtilde")
            {
                return VirtualKeyCode.ESCAPE;
            }
            else if (textinput == "Capital")
            {
                return VirtualKeyCode.CAPITAL;
            }
            else if (textinput == "ShiftKey")
            {
                return VirtualKeyCode.SHIFT;
            }
            else if (textinput == "ControlKey")
            {
                return VirtualKeyCode.CONTROL;
            }
            else if (textinput == "Lwin")
            {
                return VirtualKeyCode.LWIN;
            }
            else if (textinput == "Rwin")
            {
                return VirtualKeyCode.RWIN;
            }
            else if (textinput == "Menu")
            {
                return VirtualKeyCode.MENU;
            }
            else if (textinput == "Apps")
            {
                return VirtualKeyCode.APPS;
            }
            else if (textinput == "OemMinus")
            {
                return VirtualKeyCode.OEM_MINUS;
            }
            else if (textinput == "Oemplus")
            {
                return VirtualKeyCode.OEM_PLUS;
            }
            else if (textinput == "Back")
            {
                return VirtualKeyCode.BACK;
            }
            else if (textinput == "OemOpenBrackets")
            {
                return VirtualKeyCode.OEM_8;
            }
            else if (textinput == "Oem1")
            {
                return VirtualKeyCode.OEM_1;
            }
            else if (textinput == "Oem1")
            {
                return VirtualKeyCode.OEM_1;
            }
            else if (textinput == "Oem2")
            {
                return VirtualKeyCode.OEM_2;
            }
            else if (textinput == "Oem3")
            {
                return VirtualKeyCode.OEM_3;
            }
            else if (textinput == "Oem4")
            {
                return VirtualKeyCode.OEM_4;
            }
            else if (textinput == "Oem5")
            {
                return VirtualKeyCode.OEM_5;
            }
            else if (textinput == "Oem6")
            {
                return VirtualKeyCode.OEM_6;
            }
            else if (textinput == "Oem7")
            {
                return VirtualKeyCode.OEM_7;
            }
            else if (textinput == "Oem8")
            {
                return VirtualKeyCode.OEM_8;
            }
            else if (textinput == "Oem102")
            {
                return VirtualKeyCode.OEM_102;
            }
            else if (textinput == "Return")
            {
                return VirtualKeyCode.RETURN;
            }
            else if (textinput == "Oemcomma")
            {
                return VirtualKeyCode.OEM_COMMA;
            }
            else if (textinput == "OemPeriod")
            {
                return VirtualKeyCode.OEM_PERIOD;
            }
            else if (textinput == "OemQuestion")
            {
                return VirtualKeyCode.OEM_1;
            }
            else if (textinput == "Scroll")
            {
                return VirtualKeyCode.SCROLL;
            }
            else if (textinput == "Home")
            {
                return VirtualKeyCode.HOME;
            }
            else if (textinput == "End")
            {
                return VirtualKeyCode.END;
            }
            else if (textinput == "Insert")
            {
                return VirtualKeyCode.INSERT;
            }
            else if (textinput == "PageUp")
            {
                return VirtualKeyCode.VOLUME_UP;
            }
            else if (textinput == "PageDown")
            {
                return VirtualKeyCode.VOLUME_DOWN;
            }
            else if (textinput == "Delete")
            {
                return VirtualKeyCode.DELETE;
            }
            else if (textinput == "NumLock")
            {
                return VirtualKeyCode.NUMLOCK;
            }
            else if (textinput == "Divide")
            {
                return VirtualKeyCode.DIVIDE;
            }
            else if (textinput == "Multiply")
            {
                return VirtualKeyCode.MULTIPLY;
            }
            else if (textinput == "Subtract")
            {
                return VirtualKeyCode.SUBTRACT;
            }
            else if (textinput == "Add")
            {
                return VirtualKeyCode.ADD;
            }
            else if (textinput == "Clear")
            {
                return VirtualKeyCode.CLEAR;
            }
            else if (textinput == "OemClear")
            {
                return VirtualKeyCode.OEM_CLEAR;
            }
            else if (textinput == "Next")
            {
                return VirtualKeyCode.NEXT;
            }
            else if (textinput == "Insert")
            {
                return VirtualKeyCode.INSERT;
            }
            else if (textinput == "BrowserHome")
            {
                return VirtualKeyCode.BROWSER_HOME;
            }
            else if (textinput == "LaunchMail")
            {
                return VirtualKeyCode.LAUNCH_MAIL;
            }
            else if (textinput == "MediaPlayPause")
            {
                return VirtualKeyCode.MEDIA_PLAY_PAUSE;
            }
            else if (textinput == "MediaStop")
            {
                return VirtualKeyCode.MEDIA_STOP;
            }
            else if (textinput == "MediaPreviousTrack")
            {
                return VirtualKeyCode.MEDIA_PREV_TRACK;
            }
            else if (textinput == "MediaNextTrack")
            {
                return VirtualKeyCode.MEDIA_NEXT_TRACK;
            }
            else if (textinput == "VolumeDown")
            {
                return VirtualKeyCode.VOLUME_DOWN;
            }
            else if (textinput == "VolumeUp")
            {
                return VirtualKeyCode.VOLUME_UP;
            }
            else if (textinput == "VolumeMute")
            {
                return VirtualKeyCode.VOLUME_MUTE;
            }
            else if (textinput == "Accept")
            {
                return VirtualKeyCode.ACCEPT;
            }
            else if (textinput == "Attn")
            {
                return VirtualKeyCode.ATTN;
            }
            else if (textinput == "BrowserBack")
            {
                return VirtualKeyCode.BROWSER_BACK;
            }
            else if (textinput == "BrowserFavorites")
            {
                return VirtualKeyCode.BROWSER_FAVORITES;
            }
            else if (textinput == "BrowserForward")
            {
                return VirtualKeyCode.BROWSER_FORWARD;
            }
            else if (textinput == "BrowserRefresh")
            {
                return VirtualKeyCode.BROWSER_REFRESH;
            }
            else if (textinput == "BrowserSearch")
            {
                return VirtualKeyCode.BROWSER_SEARCH;
            }
            else if (textinput == "BrowserStop")
            {
                return VirtualKeyCode.BROWSER_STOP;
            }
            else if (textinput == "Cancel")
            {
                return VirtualKeyCode.CANCEL;
            }
            else if (textinput == "Convert")
            {
                return VirtualKeyCode.CONVERT;
            }
            else if (textinput == "Final")
            {
                return VirtualKeyCode.FINAL;
            }
            else if (textinput == "Hangul")
            {
                return VirtualKeyCode.HANGUL;
            }
            else if (textinput == "Hanja")
            {
                return VirtualKeyCode.HANJA;
            }
            else if (textinput == "HELP")
            {
                return VirtualKeyCode.HELP;
            }
            else if (textinput == "Junja")
            {
                return VirtualKeyCode.JUNJA;
            }
            else if (textinput == "Kana")
            {
                return VirtualKeyCode.KANA;
            }
            else if (textinput == "Kanji")
            {
                return VirtualKeyCode.KANJI;
            }
            else if (textinput == "LaunchApp1")
            {
                return VirtualKeyCode.LAUNCH_APP1;
            }
            else if (textinput == "LaunchApp2")
            {
                return VirtualKeyCode.LAUNCH_APP2;
            }
            else if (textinput == "LaunchMediaSelect")
            {
                return VirtualKeyCode.LAUNCH_MEDIA_SELECT;
            }
            else if (textinput == "LButton")
            {
                return VirtualKeyCode.LBUTTON;
            }
            else if (textinput == "LControl")
            {
                return VirtualKeyCode.LCONTROL;
            }
            else if (textinput == "LMenu")
            {
                return VirtualKeyCode.LMENU;
            }
            else if (textinput == "LShift")
            {
                return VirtualKeyCode.LSHIFT;
            }
            else if (textinput == "Rwin")
            {
                return VirtualKeyCode.RWIN;
            }
            else if (textinput == "MButton")
            {
                return VirtualKeyCode.MBUTTON;
            }
            else if (textinput == "ModeChange")
            {
                return VirtualKeyCode.MODECHANGE;
            }
            else if (textinput == "NoName")
            {
                return VirtualKeyCode.NONAME;
            }
            else if (textinput == "Nonconvert")
            {
                return VirtualKeyCode.NONCONVERT;
            }
            else if (textinput == "PA1")
            {
                return VirtualKeyCode.PA1;
            }
            else if (textinput == "Packet")
            {
                return VirtualKeyCode.PACKET;
            }
            else if (textinput == "Pause")
            {
                return VirtualKeyCode.PAUSE;
            }
            else if (textinput == "Play")
            {
                return VirtualKeyCode.PLAY;
            }
            else if (textinput == "Print")
            {
                return VirtualKeyCode.PRINT;
            }
            else if (textinput == "Prior")
            {
                return VirtualKeyCode.PRIOR;
            }
            else if (textinput == "ProcessKey")
            {
                return VirtualKeyCode.PROCESSKEY;
            }
            else if (textinput == "RButton")
            {
                return VirtualKeyCode.RBUTTON;
            }
            else if (textinput == "RControl")
            {
                return VirtualKeyCode.RCONTROL;
            }
            else if (textinput == "RMenu")
            {
                return VirtualKeyCode.RMENU;
            }
            else if (textinput == "RShift")
            {
                return VirtualKeyCode.RSHIFT;
            }
            else if (textinput == "Separator")
            {
                return VirtualKeyCode.SEPARATOR;
            }
            else if (textinput == "Sleep")
            {
                return VirtualKeyCode.SLEEP;
            }
            else if (textinput == "Snapshot")
            {
                return VirtualKeyCode.SNAPSHOT;
            }
            else if (textinput == "Space")
            {
                return VirtualKeyCode.SPACE;
            }
            else if (textinput == "TAB")
            {
                return VirtualKeyCode.TAB;
            }
            else if (textinput == "XButton1")
            {
                return VirtualKeyCode.XBUTTON1;
            }
            else if (textinput == "XButton2")
            {
                return VirtualKeyCode.XBUTTON2;
            }
            else if (textinput == "Zoom")
            {
                return VirtualKeyCode.ZOOM;
            }
            return VirtualKeyCode.TAB;
        }

        void PlayCommands(List<string> multicommands,List<string> multicommandtypes, List<Int32> multicommandtimers, string userName)
        {

            int timingspacer = 0;
            int currentcommandcount = 0;
            truestop = false;
            //List<string> Tempcommandorderlist = new List<string> commandorderlist;
            //List<string> Tempmultiplecommands =  multiplecommands;
            //List<Int32> Tempmultiplecommandtimers = multiplecommandtimers;
            //List<string> Tempmultiplecommandtypes = multiplecommandtypes;

            //List<string> commandorderlist = new List<string> { };
            //List<string> multiplecommands = new List<string> { };
            //List<Int32> multiplecommandtimers = new List<Int32> { };
            //List<string> multiplecommandtypes = new List<string> { };
            //Thread newthread = null;

            if (chatdisabled == false || IsAMod(userName) == true)
            {


                foreach (string singlecommand in multicommands.ToList())
                {

                    if (truestop == true) { break; } //All you need to stop everyone's fun
                    //Thread.Sleep(Convert.ToInt32(InputUpDownDelay.Value * 1000));
                    //Thread.Sleep(timingspacer + Decimal.ToInt32(InputUpDownDelay.Value) * 1000);
                    //Console.WriteLine(ThreadCount);
                    Console.WriteLine("we Just waited: " + timingspacer.ToString() + " And will add " + multicommandtimers[currentcommandcount]);
                    timingspacer = multicommandtimers[currentcommandcount];
                    //Console.WriteLine("Command Type: " + multicommandtypes[currentcommandcount]);


                    if (multicommandtypes[currentcommandcount] == "Direct Press" && IsLurking(userName) == false) //--------------------------------------DIRECT IMPACT!! .. I mean Direct Press
                    {
                        Console.WriteLine("We are about to do a DIRECT PRESS");
                        Console.WriteLine("Ingoing Keypress Delay: " + timingspacer);
                        //Console.WriteLine(Convert.ToInt16(ConvertTextToVirtualKeyCode(singlecommand)));

                        if (heldkeys.Count > 0)
                        {
                            foreach (string i in heldkeys.ToArray())
                            {
                                if (i == singlecommand.ToUpper())
                                {
                                    heldkeys.Remove(i); //Error during stream where it tried to remove something with a bad index. Maybe fix with making sure the item is not null and correct index
                                    SendDirectInput("KeyUp", ConvertTextToShortKeyCode(singlecommand));
                                }
                                else
                                {
                                    SendDirectInput("KeyDown", ConvertTextToShortKeyCode(singlecommand));
                                    //Thread.Sleep(Convert.ToInt32(InputUpDownDelay.Value * 1000)); //Adds small delay set in the settings that gives the program a chance to pick up the command played
                                    Thread.Sleep(timingspacer);
                                    SendDirectInput("KeyUp", ConvertTextToShortKeyCode(singlecommand));
                                }
                            }
                        }
                        else if (heldkeys.Count <= 0)
                        {
                            SendDirectInput("KeyDown", ConvertTextToShortKeyCode(singlecommand));
                            Thread.Sleep(timingspacer);
                            //Thread.Sleep(Convert.ToInt32(InputUpDownDelay.Value * 1000)); //Adds small delay set in the settings that gives the program a chance to pick up the command played
                            SendDirectInput("KeyUp", ConvertTextToShortKeyCode(singlecommand));
                        }
                    }
                    else if (multicommandtypes[currentcommandcount] == "Direct Hold" && IsLurking(userName) == false) //---------------------------------------------------------------------Direct Hold
                    {
                        Console.WriteLine("We are about to do a DIRECT HOLD");
                        Console.WriteLine("Ingoing Keypress Delay: " + timingspacer);

                        if (heldkeys.Count > 0)
                        {
                            foreach (string i in heldkeys.ToArray())
                            {
                                if (i == singlecommand.ToUpper())
                                {
                                    heldkeys.Remove(i); //Has had null refference error System.NullReferenceException: 'Object reference not set to an instance of an object.
                                    SendDirectInput("KeyUp", ConvertTextToShortKeyCode(singlecommand));
                                    Thread.Sleep(timingspacer);
                                    //Thread.Sleep(Convert.ToInt32(InputUpDownDelay.Value * 1000)); //Adds small delay set in the settings that gives the program a chance to pick up the command played
                                }
                                else
                                {
                                    heldkeys.Add(singlecommand.ToUpper());
                                    SendDirectInput("KeyDown", ConvertTextToShortKeyCode(singlecommand));
                                    Thread.Sleep(timingspacer);
                                    //Thread.Sleep(Convert.ToInt32(InputUpDownDelay.Value * 1000)); //Adds small delay set in the settings that gives the program a chance to pick up the command played
                                }
                            }
                        }
                        else if (heldkeys.Count <= 0)
                        {
                            heldkeys.Add(singlecommand.ToUpper());
                            SendDirectInput("KeyDown", ConvertTextToShortKeyCode(singlecommand));
                            Thread.Sleep(timingspacer);
                            //Thread.Sleep(Convert.ToInt32(InputUpDownDelay.Value * 1000)); //Adds small delay set in the settings that gives the program a chance to pick up the command played
                        }
                    }
                    else if (multicommandtypes[currentcommandcount] == "Simulated Press" && IsLurking(userName) == false) //-----------------------------------------------------------------Simulated Press
                    {
                        Console.WriteLine("We are about to press a SIMULATED PRESS");
                        Console.WriteLine("Ingoing Keypress Delay: " + timingspacer);
                        //Console.WriteLine(Convert.ToInt16(ConvertTextToVirtualKeyCode(singlecommand)));
                        InputSimulator kb = new InputSimulator();

                        if (heldkeys.Count > 0)
                        {
                            foreach (string i in heldkeys.ToArray())
                            {
                                if (i == singlecommand.ToUpper())
                                {
                                    heldkeys.Remove(i); //Error during stream where it tried to remove something with a bad index. Maybe fix with making sure the item is not null and correct index
                                    kb.Keyboard.KeyUp(ConvertTextToVirtualKeyCode(singlecommand));
                                }
                                else
                                {
                                    kb.Keyboard.KeyDown(ConvertTextToVirtualKeyCode(singlecommand));
                                    Thread.Sleep(timingspacer);
                                    //Thread.Sleep(Convert.ToInt32(InputUpDownDelay.Value * 1000)); //Adds small delay set in the settings that gives the program a chance to pick up the command played
                                    kb.Keyboard.KeyUp(ConvertTextToVirtualKeyCode(singlecommand));
                                }
                            }
                        }
                        else if (heldkeys.Count <= 0)
                        {
                            kb.Keyboard.KeyDown(ConvertTextToVirtualKeyCode(singlecommand));
                            Thread.Sleep(timingspacer);
                            //Thread.Sleep(Convert.ToInt32(InputUpDownDelay.Value * 1000)); //Adds small delay set in the settings that gives the program a chance to pick up the command played
                            kb.Keyboard.KeyUp(ConvertTextToVirtualKeyCode(singlecommand));
                        }
                    }
                    else if (multicommandtypes[currentcommandcount] == "Simulated Hold" && IsLurking(userName) == false) //---------------------------------------------------------------Simulated Hold
                    {
                        Console.WriteLine("We are about to press a SIMULATED HOLD");
                        Console.WriteLine("Ingoing Keypress Delay: " + timingspacer);
                        InputSimulator kb = new InputSimulator();

                        if (heldkeys.Count > 0)
                        {
                            foreach (string i in heldkeys.ToArray())
                            {
                                if (i == singlecommand.ToUpper())
                                {
                                    heldkeys.Remove(i); //Has had null refference error System.NullReferenceException: 'Object reference not set to an instance of an object.
                                    kb.Keyboard.KeyUp(ConvertTextToVirtualKeyCode(singlecommand));
                                    Thread.Sleep(timingspacer);
                                    //Thread.Sleep(Convert.ToInt32(InputUpDownDelay.Value * 1000)); //Adds small delay set in the settings that gives the program a chance to pick up the command played
                                }
                                else
                                {
                                    heldkeys.Add(singlecommand.ToUpper());
                                    kb.Keyboard.KeyDown(ConvertTextToVirtualKeyCode(singlecommand));
                                    Thread.Sleep(timingspacer);
                                    //Thread.Sleep(Convert.ToInt32(InputUpDownDelay.Value * 1000)); //Adds small delay set in the settings that gives the program a chance to pick up the command played
                                }
                            }
                        }
                        else if (heldkeys.Count <= 0)
                        {
                            heldkeys.Add(singlecommand.ToUpper());
                            kb.Keyboard.KeyDown(ConvertTextToVirtualKeyCode(singlecommand));
                            Thread.Sleep(timingspacer);
                            //Thread.Sleep(Convert.ToInt32(InputUpDownDelay.Value * 1000)); //Adds small delay set in the settings that gives the program a chance to pick up the command played
                        }
                    }
                    else if (multicommandtypes[currentcommandcount] == "Stop" && IsLurking(userName) == false) //------------------------------------------------------------------------------Stop it, get some help
                    {
                        Console.WriteLine("We are about to perform a TRUE STOP");
                        InputSimulator kb = new InputSimulator();

                        if (heldkeys.Count > 0)
                        {
                            foreach (string i in heldkeys.ToArray())
                            {
                                heldkeys.Remove(i); //Has had null refference error System.NullReferenceException: 'Object reference not set to an instance of an object.
                            }
                        }
                        /*
                        if (multicommands.Count > 0)
                        {
                            foreach (string i in multicommands.ToArray())
                            {
                                multicommands.Remove(i); //Has had null refference error System.NullReferenceException: 'Object reference not set to an instance of an object.
                            }
                        }
                        */
                        if (commandorderlist.Count > 0)
                        {
                            foreach (string i in commandorderlist.ToArray())
                            {
                                commandorderlist.Remove(i); //Has had null refference error System.NullReferenceException: 'Object reference not set to an instance of an object.
                            }
                        }
                        foreach (Object i in MasterControlList.Items)
                        {
                            string[] itembreakdown = i.ToString().Split(Convert.ToChar("|"));
                            if (itembreakdown[2] != null)
                            {
                                kb.Keyboard.KeyUp(ConvertTextToVirtualKeyCode(itembreakdown[2]));
                                SendDirectInput("KeyUp", ConvertTextToShortKeyCode(itembreakdown[2]));
                            }
                        }
                        truestop = true;
                        currentcommandcount = 0;
                        //multicommandtimers.Clear();
                        break;
                    }
                    else if (multicommandtypes[currentcommandcount] == "Delay" && IsLurking(userName) == false) //------------------------------------------------------------------------------Let's Try some Delays
                    {
                        Thread.Sleep(timingspacer);
                        //Thread.Sleep(Convert.ToInt32(InputUpDownDelay.Value * 1000)); //Adds small delay set in the settings that gives the program a chance to pick up the command played
                    }
                    else if (multicommandtypes[currentcommandcount] == "Disable" && IsLurking(userName) == false) //------------------------------------------------------------------------------Let's Disable Chat
                    {

                        if (chatdisabled == true)
                        {
                            Console.WriteLine("Chat has been Enabled. Attempt again to Disable.");
                            chatdisabled = false;
                        }
                        else if (chatdisabled == false)
                        {
                            Console.WriteLine("Chat has been disabled. Attempt again to reEnable.");
                            chatdisabled = true;
                        }

                    }
                    else if (multicommandtypes[currentcommandcount] == "Lurk" && EnableLurkingCheckBox.Checked == true) //------------------------------------------------------------------------------Let's Lurk
                    {
                        if (IsLurking(userName) == false)
                        {
                            Console.WriteLine("Player wants to Lurk");
                            LurkList.Items.Add(userName);
                            if (Properties.Settings.Default.TwitchLurkList != null)
                            {
                                Properties.Settings.Default.TwitchLurkList.Clear();
                                foreach (string item in LurkList.Items)
                                {
                                    Properties.Settings.Default.TwitchLurkList.Add(userName);
                                }
                                Properties.Settings.Default.Save();
                            }
                        }
                        else
                        {
                            Console.WriteLine("Player is already Lurking");
                        }
                    }
                    else if (multicommandtypes[currentcommandcount] == "Play") //------------------------------------------------------------------------------Let's Play
                    {
                        if (IsLurking(userName) == true)
                        {
                            Console.WriteLine("Player wants to Play");
                            LurkList.Items.Remove(userName);
                            if (Properties.Settings.Default.TwitchLurkList != null)
                            {
                                Properties.Settings.Default.TwitchLurkList.Clear();
                                foreach (string item in LurkList.Items)
                                {
                                    Properties.Settings.Default.TwitchLurkList.Add(userName);
                                }
                                Properties.Settings.Default.Save();
                            }
                        }
                        else
                        {
                            Console.WriteLine("Player is already Playing");
                        }

                    }
                    else if (multicommandtypes[currentcommandcount].StartsWith("Reminder:") && IsLurking(userName) == false) //------------------------------------------------------------------------------Let's Try some Reminders
                    {
                        //Console.WriteLine("We are about to Remind the Chat about something");
                        foreach (Object i in RemindersListBox.Items)
                        {
                            string[] itembreakdown = i.ToString().Split(Convert.ToChar("|"));
                            //Console.WriteLine(itembreakdown[0]);
                            if (itembreakdown[0] != null & multicommandtypes[currentcommandcount].Substring(9) == itembreakdown[0])
                            {
                                if (itembreakdown[3].Length > 500)
                                {
                                    int maxLength = 500;
                                    for (int index = 0; index < itembreakdown[3].Length; index += maxLength)
                                    {
                                        twitchClient.SendPublicChatMessage(itembreakdown[3].Substring(index, Math.Min(maxLength, itembreakdown[3].Length - index)));
                                    }
                                    TwitchIRCTextbox.AppendText("REMINDER:" + itembreakdown[3] + Environment.NewLine);
                                    //I'll get around to splitting it up
                                    //irc.SendPublicChatMessage(itembreakdown[3]);
                                }
                                else
                                {
                                    twitchClient.SendPublicChatMessage(itembreakdown[3]);
                                    TwitchIRCTextbox.AppendText("REMINDER:" + itembreakdown[3] + Environment.NewLine);
                                    //irc.SendIrcMessage(itembreakdown[3]);
                                }
                            }
                        }
                    }

                    currentcommandcount += 1;
                    Thread.Sleep(Convert.ToInt32(InputUpDownDelay.Value * 1000)); //Adds small delay set in the settings that gives the program a chance to pick up the command played
                    //Thread.Sleep(Decimal.ToInt32(InputUpDownDelay.Value) * 1000);
                    //Thread.Sleep(Decimal.ToInt32(InputUpDownDelay.Value) * 1000);
                    //--------------------------Let's check for TRUE STOP------------------------------------------------------------------------------
                    /*if (truestop == true)
                    {
                        currentcommandcount = 0;
                        multicommandtimers.Clear();
                        truestop = false;
                        break;
                    }*/
                }
            }

            //if (AllowMultipleTriggersCheckbox.Checked == false)
            //{
            //    break;
            //}
        }

        /*
        public void SimulateKeyPress(string userinput, int keypressdelay, bool dinput)
        {
            //SendKeys.SendWait("{ENTER}");

            Console.WriteLine("Ingoing Keypress Delay: " + keypressdelay);
            Console.WriteLine(Convert.ToInt16(ConvertTextToVirtualKeyCode(userinput)));
            InputSimulator kb = new InputSimulator();
            if (heldkeys.Count > 0)
            {
                foreach (string i in heldkeys.ToArray())
                {
                    if (i == userinput.ToUpper())
                    {
                        heldkeys.Remove(i); //Error during stream where it tried to remove something with a bad index. Maybe fix with making sure the item is not null and correct index
                        if (dinput == true) { SendDirectInput("KeyUp", ConvertTextToShortKeyCode(userinput)); } else { kb.Keyboard.KeyUp(ConvertTextToVirtualKeyCode(userinput)); }
                        //Thread.Sleep(Decimal.ToInt32(InputUpDownDelay.Value * 1000));

                    }
                    else
                    {
                        if (dinput == true) { SendDirectInput("KeyDown", ConvertTextToShortKeyCode(userinput)); } else { kb.Keyboard.KeyDown(ConvertTextToVirtualKeyCode(userinput)); }
                        Thread.Sleep(keypressdelay);
                        if (dinput == true) { SendDirectInput("KeyUp", ConvertTextToShortKeyCode(userinput)); } else { kb.Keyboard.KeyUp(ConvertTextToVirtualKeyCode(userinput)); }
                       
                        //Thread.Sleep(Decimal.ToInt32(InputUpDownDelay.Value * 1000));
                    }
                }
            }
            else if (heldkeys.Count <= 0)
            {
                Console.WriteLine("Input: " + userinput);
                if (dinput == true) { SendDirectInput("KeyDown", ConvertTextToShortKeyCode(userinput)); } else { kb.Keyboard.KeyDown(ConvertTextToVirtualKeyCode(userinput)); }
                Thread.Sleep(keypressdelay);
                if (dinput == true) { SendDirectInput("KeyUp", ConvertTextToShortKeyCode(userinput)); } else { kb.Keyboard.KeyUp(ConvertTextToVirtualKeyCode(userinput)); }
                //Thread.Sleep(Decimal.ToInt32(InputUpDownDelay.Value * 1000));
            }
        }

        public void SimulateKeyHold(string userinput, int keypressdelay, bool dinput)
        {
            InputSimulator kb = new InputSimulator();
            if (heldkeys.Count > 0)
            {
                foreach (string i in heldkeys.ToArray())
                {
                    if (i == userinput.ToUpper())
                    {
                        heldkeys.Remove(i); //Has had null refference error System.NullReferenceException: 'Object reference not set to an instance of an object.
                        //kb.Keyboard.KeyUp(ConvertTextToVirtualKeyCode(userinput));
                        //SendDirectInput("KeyUp", ConvertTextToShortKeyCode(userinput));
                        if (dinput == true) { SendDirectInput("KeyUp", ConvertTextToShortKeyCode(userinput)); } else { kb.Keyboard.KeyUp(ConvertTextToVirtualKeyCode(userinput)); }
                        Thread.Sleep(keypressdelay);
                        //Thread.Sleep(Decimal.ToInt32(InputUpDownDelay.Value * 1000));

                    }
                    else
                    {
                        heldkeys.Add(userinput.ToUpper());
                        //kb.Keyboard.KeyDown(ConvertTextToVirtualKeyCode(userinput));
                        //SendDirectInput("KeyDown", ConvertTextToShortKeyCode(userinput));
                        if (dinput == true) { SendDirectInput("KeyDown", ConvertTextToShortKeyCode(userinput)); } else { kb.Keyboard.KeyDown(ConvertTextToVirtualKeyCode(userinput)); }
                        Thread.Sleep(keypressdelay);
                        //Thread.Sleep(Decimal.ToInt32(InputUpDownDelay.Value * 1000));
                    }
                }
            }
            else if (heldkeys.Count <= 0)
            {
                Console.WriteLine("Input: " + userinput);
                heldkeys.Add(userinput.ToUpper());
                //kb.Keyboard.KeyDown(ConvertTextToVirtualKeyCode(userinput));
                //SendDirectInput("KeyDown", ConvertTextToShortKeyCode(userinput));
                if (dinput == true) { SendDirectInput("KeyDown", ConvertTextToShortKeyCode(userinput)); } else { kb.Keyboard.KeyDown(ConvertTextToVirtualKeyCode(userinput)); }
                Thread.Sleep(keypressdelay);
                //Thread.Sleep(Decimal.ToInt32(InputUpDownDelay.Value * 1000));
            }
            /*if (heldkeys.Count > 0) {
            foreach (string i in heldkeys.ToArray())
            {
                if (i == userinput.ToUpper())
                {
                    heldkeys.Remove(i);
                    kb.Keyboard.KeyUp(ConvertTextToVirtualKeyCode(userinput));
                    Thread.Sleep(Decimal.ToInt32(InputUpDownDelay.Value * 1000));

                }
                else
                {
                    heldkeys.Add(userinput.ToUpper());
                    kb.Keyboard.KeyDown(ConvertTextToVirtualKeyCode(userinput));
                    Thread.Sleep(Decimal.ToInt32(InputUpDownDelay.Value * 1000));
                }
            }
            } else if (heldkeys.Count <= 0)
            {
                heldkeys.Add(userinput.ToUpper());
                kb.Keyboard.KeyDown(ConvertTextToVirtualKeyCode(userinput));
                //Thread.Sleep(Decimal.ToInt32(InputUpDownDelay.Value * 1000));
            } 
        }

        public void StopAllIncomingCommands()
        {
            InputSimulator kb = new InputSimulator();
            if (heldkeys.Count > 0)
            {
                foreach (string i in heldkeys.ToArray())
                {
                    heldkeys.Remove(i); //Has had null refference error System.NullReferenceException: 'Object reference not set to an instance of an object.
                }
            }
            if (multiplecommands.Count > 0)
            {
                foreach (string i in multiplecommands.ToArray())
                {
                    multiplecommands.Remove(i); //Has had null refference error System.NullReferenceException: 'Object reference not set to an instance of an object.
                }
            }
            if (commandorderlist.Count > 0)
            {
                foreach (string i in commandorderlist.ToArray())
                {
                    commandorderlist.Remove(i); //Has had null refference error System.NullReferenceException: 'Object reference not set to an instance of an object.
                }
            }
            foreach (Object i in MasterControlList.Items)
            {
                string[] itembreakdown = i.ToString().Split(Convert.ToChar("|"));
                if (itembreakdown[2] != null)
                {
                    kb.Keyboard.KeyUp(ConvertTextToVirtualKeyCode(itembreakdown[2]));
                    SendDirectInput("KeyUp", ConvertTextToShortKeyCode(itembreakdown[2]));
                }
            }
        }
    */

        public void UpdateControlTypeBox()
        {
            if (RemindersListBox != null)
            {
                CommandTypeBox.Items.Clear();
                CommandTypeBox.Items.Add("Direct Press");
                CommandTypeBox.Items.Add("Direct Hold");
                CommandTypeBox.Items.Add("Simulated Press");
                CommandTypeBox.Items.Add("Simulated Hold");
                CommandTypeBox.Items.Add("Stop");
                CommandTypeBox.Items.Add("Delay");
                CommandTypeBox.Items.Add("Disable");
                if (EnableLurkingCheckBox.Checked == true) {
                CommandTypeBox.Items.Add("Lurk");
                CommandTypeBox.Items.Add("Play");
                }
                //--Bleh I hate doing this manully (Resets values that are ment to be there then adds additional triggers after)
                foreach (string item in RemindersListBox.Items)
                {
                    string[] itembreakdown = item.ToString().Split(Convert.ToChar("|"));
                    if (itembreakdown[1] != null & itembreakdown[1] == "Trigger")
                    {
                        CommandTypeBox.Items.Add("Reminder:" + itembreakdown[0]);
                    }
                }
                Properties.Settings.Default.Save();
            }
        }
        public void UpdateSelectedReminder()
        {
            if (RemindersListBox.SelectedItem != null)
            {
                int selectedindex = RemindersListBox.SelectedIndex;
                RemindersListBox.Items.RemoveAt(RemindersListBox.SelectedIndex);
                RemindersListBox.Items.Insert(selectedindex, ReminderNameBox.Text + "|" + ReminderTypeBox.Text + "|" + ReminderTimerBox.Text + "|" + ReminderTextBox.Text);
                RemindersListBox.SelectedIndex = selectedindex;

                Properties.Settings.Default.RemindersList.Clear();
                foreach (string item in RemindersListBox.Items)
                {
                    Properties.Settings.Default.RemindersList.Add(item);
                }
                Properties.Settings.Default.Save();
            }
        }

        public void UpdateSelectedControl()
        {
            if (MasterControlList.SelectedItem != null)
            {
                int selectedindex = MasterControlList.SelectedIndex;
                MasterControlList.Items.RemoveAt(MasterControlList.SelectedIndex);
                MasterControlList.Items.Insert(selectedindex, CommandName.Text + "|" + CommandBox.Text + "|" + OutputBox.Text + "|" + Convert.ToString(CommandEnabledCheckbox.Checked) + "|" + Convert.ToString(ModOnlyCheckBox.Checked) + "|" + Convert.ToString(CommandCaseSensitiveBox.Checked) + "|" + Convert.ToString(CommandTypeBox.Text) + "|" + Convert.ToString(KeyPressDelay.Value));
                MasterControlList.SelectedIndex = selectedindex;

                Properties.Settings.Default.CommandList.Clear();
                foreach (string item in MasterControlList.Items)
                {
                    Properties.Settings.Default.CommandList.Add(item);
                }
                Properties.Settings.Default.Save();
            }
        }
        private void TwitchChatNavigateButton_Click(object sender, EventArgs e)
        {
        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }
        public bool IsAMod(string userName)
        {
            foreach (Object m in ModList.Items)
            {
                if (userName.ToUpper() == m.ToString().ToUpper())
                {
                    Console.WriteLine("You passed the mod test!");
                    return true;
                }
            }
            return false;
        }
        public bool IsLurking(string userName)
        {
            foreach (Object m in LurkList.Items)
            {
                if (userName.ToUpper() == m.ToString().ToUpper())
                {
                    Console.WriteLine("User is currently lurking!");
                    return true;
                }
            }
            return false;
        }

        public ITwitchClient CreateTwitchClient(string hostname, int port, string username, string oauth, string channel)
        {
            string selectedTwitchClient = TwitchChatClientChoice.Items[TwitchChatClientChoice.SelectedIndex].ToString();
            if (selectedTwitchClient.Equals("TwitchLib.Client"))
            {
                return new TwitchLibClient(hostname, port, username, oauth, channel);
            }
            else 
            {
                return new IrcClient(hostname, port, username, oauth, channel);
            }
        }

        public IMessageParser CreateMessageParser()
        {
            string selectedTwitchClient = TwitchChatClientChoice.Items[TwitchChatClientChoice.SelectedIndex].ToString();
            if (selectedTwitchClient.Equals("TwitchLib.Client"))
            {
                return new TwitchLibClientMessageParser();
            } else
            {
                return new IrcMessageParser();
            }
        }

        public void ConnectButton_Click(object sender, EventArgs e)
        {

            if (PressedConnect == false)
            {
                // Initialize and connect to Twitch chat
                twitchClient = CreateTwitchClient("irc.twitch.tv", 6667,
                TwitchUsernameTextbox.Text, TwitchOAuthTextbox.Text, TwitchChannelNameTextbox.Text);
                messageParser = CreateMessageParser();

            // Ping to the server to make sure this bot stays connected to the chat
            // Server will respond back to this bot with a PONG (without quotes):
            // Example: ":tmi.twitch.tv PONG tmi.twitch.tv :irc.twitch.tv"
            ping = new PingSender(twitchClient);
            ping.Start();
                //autoconnect = new AutoReconnect(irc, Convert.ToInt32(ReconnectTimeBox.Value));
                //autoconnect.Start();
                

                TwitchIRCTextbox.Clear();
            PressedConnect = true;
            ConnectButton.Text = "Connected | Press to Disconnect";
              /*  new Thread(() =>
                {
                    while (PressedConnect)
                    {
                        Thread.Sleep(Convert.ToInt32(ReconnectTimeBox.Value));
                        Console.WriteLine("Last IRC message: " + irc.ReadMessage());
                        if (irc.IsConnected() == false)
                        {
                            irc.SendIrcMessage("**Disconnected From IRC**");
                            irc = null; 
                            timeschatted = 0;

                            Thread.Sleep(Convert.ToInt32(ReconnectTimeBox.Value));

                            irc = new IrcClient("irc.twitch.tv", 6667,
                            TwitchUsernameTextbox.Text, TwitchOAuthTextbox.Text, TwitchChannelNameTextbox.Text);

                            if (BackgroundWorker.IsBusy != true)
                            {
                                BackgroundWorker.RunWorkerAsync();
                                Console.WriteLine("Running Background Worker");
                            }
                        }
                    }

                }).Start();
              */
                //Run Background Worker
                if (BackgroundWorker.IsBusy != true)
            {
                BackgroundWorker.RunWorkerAsync();
                    Console.WriteLine("Running Background Worker");
                    //Lets start some Reminder Timers
                    foreach (Object i in RemindersListBox.Items)
                    {
                        string[] itembreakdown = i.ToString().Split(Convert.ToChar("|"));
                        Console.WriteLine(itembreakdown[0]);
                        if (itembreakdown[1] == "Timer")
                        {
                            //Thread newtimerthread =
                            //new Thread(() =>
                            //{

                            //});
                            //newtimerthread.Start();
                            //DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
                            //System.Timers.Timer myTimer = new System.Timers.Timer(Convert.ToInt32(itembreakdown[2])*1000);
                            //System.Timers.Timer myTimer2 = new System.Windows.Forms.Timer(1000);
                            var myTimer = new System.Timers.Timer(Convert.ToInt32(itembreakdown[2]) * 1000);
                            myTimer.Elapsed += (source, c) => ReminderTimerTick(sender, c, itembreakdown[3], "Timer");//ReminderTimerTick;
                            myTimer.AutoReset = true;
                            myTimer.Enabled = true;
                            remindertimers.Add(myTimer);
                        } else if (itembreakdown[1] == "Active Timer")
                        {
                            var myTimer = new System.Timers.Timer(Convert.ToInt32(itembreakdown[2]) * 1000);
                            myTimer.Elapsed += (source, c) => ReminderTimerTick(sender, c, itembreakdown[3], "Active Timer");//ReminderTimerTick;
                            myTimer.AutoReset = true;
                            myTimer.Enabled = true;
                            remindertimers.Add(myTimer);
                        } else if (itembreakdown[1] == "Inactive Timer")
                        {
                            var myTimer = new System.Timers.Timer(Convert.ToInt32(itembreakdown[2]) * 1000);
                            myTimer.Elapsed += (source, c) => ReminderTimerTick(sender, c, itembreakdown[3], "Inactive Timer");//ReminderTimerTick;
                            myTimer.AutoReset = true;
                            myTimer.Enabled = true;
                            remindertimers.Add(myTimer);
                        }
                    }
                    //Delete the activity timer now cause this is an auto reconnect, and we're about to make a new timer anyway
                    if (autoreconnecting == true)
                    {
                        activechattick = 0;
                        activitytimer.Stop();
                        activitytimer.Enabled = false;
                        activitytimer = null;
                        autoreconnecting = false; //may need to change where this happens
                    }
                    //Activity timer to determine if chat is active or not, other timers will act based on if chat is active or not, not this timer
                    var acttimer = new System.Timers.Timer(1000);
                    acttimer.Elapsed += (source, c) => ActivityTimerTick(sender, c);
                    acttimer.AutoReset = true;
                    acttimer.Enabled = true;
                    activitytimer = acttimer;
                }
                
            } else if (PressedConnect == true)
            {
                //if (irc != null)
                //{
                    PressedConnect = false;
                twitchClient.SendIrcMessage("Disconnected from IRC");
                twitchClient.Disconnect();
                twitchClient = null; //This also makes the BackgroundWorker Stop.
                            //BackgroundWorker.CancelAsync();
                            //BackgroundWorker.DoWork();
                
                    ConnectButton.Text = "Connect";
                    timeschatted = 0;
                    TwitchIRCTextbox.Clear();
                //Remove Reminder Timers
                foreach (System.Timers.Timer i in remindertimers)
                {
                    System.Timers.Timer myTimer = i;
                    myTimer.Dispose();
                    myTimer = null;
                }
                remindertimers.Clear();
                //keeps auto reconnect timer from stopping
                if (autoreconnecting == false)
                {
                    activechattick = 0;
                    activitytimer.Stop();
                    activitytimer.Enabled = false;
                    activitytimer = null;
                }
                
                //}

            }
            
        }

        public void ReminderTimerTick(Object source, ElapsedEventArgs c, string remindertext, string remindertype)
        {
            if (remindertype == "Timer")
            {
                if (remindertext.Length > 500)
                {
                    int maxLength = 500;
                    for (int index = 0; index < remindertext.Length; index += maxLength)
                    {
                        twitchClient.SendPublicChatMessage(remindertext.Substring(index, Math.Min(maxLength, remindertext.Length - index)));
                    }
                    TwitchIRCTextbox.AppendText("REMINDER:" + remindertext + Environment.NewLine);
                }
                else
                {
                    twitchClient.SendPublicChatMessage(remindertext);
                    TwitchIRCTextbox.AppendText("REMINDER:" + remindertext + Environment.NewLine);
                }
                
            } else if (remindertype == "Active Timer")
            {
                Console.WriteLine("Using Active Chat, Status is:" + activechat);
                if (activechat == true)
                {
                    if (remindertext.Length > 500)
                    {
                        int maxLength = 500;
                        for (int index = 0; index < remindertext.Length; index += maxLength)
                        {
                            twitchClient.SendPublicChatMessage(remindertext.Substring(index, Math.Min(maxLength, remindertext.Length - index)));
                        }
                        TwitchIRCTextbox.AppendText("REMINDER:" + remindertext + Environment.NewLine);
                    }
                    else
                    {
                        twitchClient.SendPublicChatMessage(remindertext);
                        TwitchIRCTextbox.AppendText("REMINDER:" + remindertext + Environment.NewLine);
                    }
                }
            
            }
            else if (remindertype == "Inactive Timer")
            {
                Console.WriteLine("Using Inactive Chat, Status is:" + activechat);
                if (activechat == false)
                {
                    if (remindertext.Length > 500)
                    {
                        int maxLength = 500;
                        for (int index = 0; index < remindertext.Length; index += maxLength)
                        {
                            twitchClient.SendPublicChatMessage(remindertext.Substring(index, Math.Min(maxLength, remindertext.Length - index)));
                        }
                        TwitchIRCTextbox.AppendText("REMINDER:" + remindertext + Environment.NewLine);
                    }
                    else
                    {
                        twitchClient.SendPublicChatMessage(remindertext);
                        TwitchIRCTextbox.AppendText("REMINDER:" + remindertext + Environment.NewLine);
                    }
                }
            }


        }

        public void ActivityTimerTick(Object source, ElapsedEventArgs c)
        {
            activechattick += 1;
            //Console.WriteLine(activechattick + " " + Convert.ToInt32(ReminderActivityTimer.Value));
            if (activechattick >= Convert.ToInt32(ReminderActivityTimer.Value)) {
                //irc.SendPublicChatMessage("***Chat No Longer Active***");
                //TwitchIRCTextbox.AppendText("REMINDER:" + remindertext + Environment.NewLine);
                
                activechat = false;
                //activitytimer.Enabled = false;
                //activitytimer.Stop();
                //activitytimer.Dispose();
                //activitytimer = null;
                //activitytimer = new System.Timers.Timer(Convert.ToInt32(ReminderActivityTimer.Value) * 1000);
            }else if (activechattick >= Convert.ToInt32(ReconnectTimeBox.Value) && AutoReconnectCheckBox.Checked == true && autoreconnecting == false)
            {
                Console.WriteLine("Chat has been inactive for a while, let's reset the bot!");
                //ConnectButton_Click(object sender, EventArgs e);
                autoreconnecting = true;
                ConnectButton.PerformClick();
                Thread.Sleep(Convert.ToInt32(ReconnectDelayTimeBox.Value*1000));
                ConnectButton.PerformClick();
            }


        }

        private void SendTwitchMessage_Click(object sender, EventArgs e)
        {
            if (twitchClient != null)
            {
                twitchClient.SendPublicChatMessage(TwitchMessageText.Text);
                TwitchIRCTextbox.Text = TwitchIRCTextbox.Text + ("YOU SAID: " + TwitchMessageText.Text + Environment.NewLine);
                TwitchMessageText.Text = "";
            }
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;

            while (twitchClient != null)
            {
                //Thread.Sleep(1000);
                timeschatted += 1;
                //TwitchIRCTextbox.Text = TwitchIRCTextbox.Text + (timeschatted.ToString() + Environment.NewLine); // Print raw irc messages

                string message = twitchClient.ReadMessage();
                if (timeschatted < 11)
                {
                    TwitchIRCTextbox.Text = TwitchIRCTextbox.Text + (message + Environment.NewLine); // Print raw irc messages
                }

                if (message != null)
                {
                    //At some point in a stream the message returned 'null' and broke the bot Needs fix, Maybe "try' instead

                    messageParser.Parse(message);
                    if (messageParser.IsValid())
                    {
                        string userName = messageParser.GetUserName();
                        message = messageParser.GetChatMessage();

                        /* Gives that big block of text of the commands lol
                        if (message.Equals(ShowAllCommandsKeywordTextbox.Text))
                        {
                            string wholecommandlist = "List of Active Commands: ";
                            foreach (Object i in MasterControlList.Items)
                            {
                                string[] itembreakdown = i.ToString().Split(Convert.ToChar("|"));
                                if (Convert.ToBoolean(itembreakdown[3]) == true)
                                {
                                    wholecommandlist = wholecommandlist + itembreakdown[1] + ",";
                                }
                            }
                            irc.SendPublicChatMessage(wholecommandlist);
                        } */
                        //If you wanna lurk, we'll put ya down for it
                        /*if (message.Equals(LurkCommandTextBox.Text) && EnableLurkingCheckBox.Checked == true)
                        {
                            Console.WriteLine("Lurk Command seen");
                            bool isinlurklist = false;
                            int removeindex = 0;
                            foreach (var l in LurkList.Items)
                            {
                                if (userName.ToUpper() == l.ToString().ToUpper())
                                {
                                    isinlurklist = true;
                                    Console.WriteLine("Lurker exsists in the list");
                                }
                                if (isinlurklist == false)
                                {
                                    removeindex++;
                                }
                            }
                            if (isinlurklist == true)
                            {
                                Console.WriteLine("Found in the list, Removing them");
                                LurkList.Items.RemoveAt(removeindex);
                                if (Properties.Settings.Default.TwitchLurkList != null)
                                {
                                    Properties.Settings.Default.TwitchLurkList.Clear();
                                    foreach (string item in LurkList.Items)
                                    {
                                        Properties.Settings.Default.TwitchLurkList.Add(item);
                                    }
                                    Properties.Settings.Default.Save();
                                }
                                //Properties.Settings.Default.TwitchLurkList.RemoveAt(removeindex);
                                //Properties.Settings.Default.Save();
                                string playmessage = PlayingMessageTextBox.Text.Replace("{user}", userName);
                                irc.SendPublicChatMessage(playmessage);
                            } else {
                                Console.WriteLine("Was not found in the list, adding them");
                                var newindex = LurkList.Items.Add(userName);

                                if (Properties.Settings.Default.TwitchLurkList != null)
                                {
                                    Properties.Settings.Default.TwitchLurkList.Clear();
                                    foreach (string item in LurkList.Items)
                                    {
                                        Console.WriteLine("New Lurker? " + item);
                                        Properties.Settings.Default.TwitchLurkList.Add(item);
                                    }
                                    Properties.Settings.Default.Save();
                                }
                                //LurkList.Items.Add(userName);
                                //Properties.Settings.Default.TwitchLurkList.Add(userName);
                                //Properties.Settings.Default.Save();
                                string lurkmessage = LurkingMessageTextBox.Text.Replace("{user}", userName);
                                irc.SendPublicChatMessage(lurkmessage);
                            }
                        }
                        */
                        //Check if they are a lurker :v
                        /*bool islurking = false;
                        if (EnableLurkingCheckBox.Checked == true)
                        {
                            foreach (var l in LurkList.Items)
                            {
                                if (userName.ToUpper() == l.ToString().ToUpper())
                                {
                                    islurking = true;
                                }
                            }
                        } */
                        //if (islurking == false)
                        //{

                        
                        //Console.WriteLine(message); // Print parsed irc message (debugging only)
                        //This clears the Multi Command List and Repopulates it accordingly ********************************************************
                        multiplecommands.Clear();
                        multiplecommandtimers.Clear();
                        multiplecommandtypes.Clear();
                        //Clear Active Reminder Timer if Chatted
                        activechat = true;
                        activechattick = 0;

                            if (UseMultipleCommandsSeparator.Checked == true)
                        {
                            if (message.Contains(MultipleCommandSeparatorTextBox.Text))
                            {
                                string[] messagesplit = message.Split(Convert.ToChar(MultipleCommandSeparatorTextBox.Text));
                                //string[] messagesplit2 = 
                                Console.WriteLine(Regex.Matches(message, "([-+]?/s?)").Cast<Match>().Select(m => m.Value));
                                foreach (var i in messagesplit)
                                {
                                    foreach (var c in MasterControlList.Items)
                                    {
                                        string[] itembreakdown = c.ToString().Split(Convert.ToChar("|"));

                                        if (itembreakdown[5] != null & Convert.ToBoolean(itembreakdown[5]) == true)
                                        {
                                            if (i == itembreakdown[1])
                                            {
                                                if (Convert.ToBoolean(itembreakdown[4]) == true) //If its a mod command we will check if their user matches the mod list
                                                {
                                                    foreach (Object m in ModList.Items)
                                                    {
                                                        if (userName.ToUpper() == m.ToString().ToUpper())
                                                        {
                                                            Console.WriteLine("You passed the mod test!");
                                                            multiplecommands.Add(itembreakdown[2]);
                                                            multiplecommandtimers.Add(Convert.ToInt32(Convert.ToDecimal(itembreakdown[7]) * 1000));
                                                            multiplecommandtypes.Add(itembreakdown[6]);
                                                            //commandorderlist.Add(itembreakdown[2]);
                                                            Console.WriteLine(i + " Matches " + itembreakdown[1]);
                                                        }
                                                    }
                                                } else
                                                {
                                                    multiplecommands.Add(itembreakdown[2]);
                                                    multiplecommandtimers.Add(Convert.ToInt32(Convert.ToDecimal(itembreakdown[7]) * 1000));
                                                    multiplecommandtypes.Add(itembreakdown[6]);
                                                    //commandorderlist.Add(itembreakdown[2]);
                                                    Console.WriteLine(i + " Matches " + itembreakdown[1]);
                                                }
                                            }
                                        }
                                        else if (itembreakdown[5] != null & Convert.ToBoolean(itembreakdown[5]) == false)
                                        {
                                            if (i.ToUpper() == itembreakdown[1].ToUpper())
                                            {
                                                if (Convert.ToBoolean(itembreakdown[4]) == true) //If its a mod command we will check if their user matches the mod list
                                                {
                                                    foreach (Object m in ModList.Items)
                                                    {
                                                        if (userName.ToUpper() == m.ToString().ToUpper())
                                                        {
                                                            Console.WriteLine("You passed the mod test!");
                                                            multiplecommands.Add(itembreakdown[2]);
                                                            multiplecommandtimers.Add(Convert.ToInt32(Convert.ToDecimal(itembreakdown[7]) * 1000));
                                                            multiplecommandtypes.Add(itembreakdown[6]);
                                                            //commandorderlist.Add(itembreakdown[2]);
                                                            Console.WriteLine(i + " Matches " + itembreakdown[1]);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    multiplecommands.Add(itembreakdown[2]);
                                                    multiplecommandtimers.Add(Convert.ToInt32(Convert.ToDecimal(itembreakdown[7]) * 1000));
                                                    multiplecommandtypes.Add(itembreakdown[6]);
                                                    //commandorderlist.Add(itembreakdown[2]);
                                                    Console.WriteLine(i + " Matches " + itembreakdown[1]);
                                                }
                                            }
                                        }


                                    }

                                }
                                
                            } else if (message != null)
                            {
                                foreach (var c in MasterControlList.Items)
                                {
                                    string[] itembreakdown = c.ToString().Split(Convert.ToChar("|"));

                                    if (itembreakdown[5] != null & Convert.ToBoolean(itembreakdown[5]) == true)
                                    {
                                        if (message == itembreakdown[1])
                                        {
                                            if (Convert.ToBoolean(itembreakdown[4]) == true) //If its a mod command we will check if their user matches the mod list
                                            {
                                                foreach (Object m in ModList.Items)
                                                {
                                                    if (userName.ToUpper() == m.ToString().ToUpper())
                                                    {
                                                        Console.WriteLine("You passed the mod test!");
                                                        multiplecommands.Add(itembreakdown[2]);
                                                        multiplecommandtimers.Add(Convert.ToInt32(Convert.ToDecimal(itembreakdown[7]) * 1000));
                                                        multiplecommandtypes.Add(itembreakdown[6]);
                                                        //commandorderlist.Add(itembreakdown[2]);
                                                        Console.WriteLine(message + " Matches " + itembreakdown[1]);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                multiplecommands.Add(itembreakdown[2]);
                                                multiplecommandtimers.Add(Convert.ToInt32(Convert.ToDecimal(itembreakdown[7]) * 1000));
                                                multiplecommandtypes.Add(itembreakdown[6]);
                                                //commandorderlist.Add(itembreakdown[2]);
                                                Console.WriteLine(message + " Matches " + itembreakdown[1]);
                                            }
                                        }
                                    }
                                    else if (itembreakdown[5] != null & Convert.ToBoolean(itembreakdown[5]) == false)
                                    {
                                        if (message.ToUpper() == itembreakdown[1].ToUpper())
                                        {
                                            if (Convert.ToBoolean(itembreakdown[4]) == true) //If its a mod command we will check if their user matches the mod list
                                            {
                                                foreach (Object m in ModList.Items)
                                                {
                                                    if (userName.ToUpper() == m.ToString().ToUpper())
                                                    {
                                                        Console.WriteLine("You passed the mod test!");
                                                        multiplecommands.Add(itembreakdown[2]);
                                                        multiplecommandtimers.Add(Convert.ToInt32(Convert.ToDecimal(itembreakdown[7]) * 1000));
                                                        multiplecommandtypes.Add(itembreakdown[6]);
                                                        //commandorderlist.Add(itembreakdown[2]);
                                                        Console.WriteLine(message + " Matches " + itembreakdown[1]);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                multiplecommands.Add(itembreakdown[2]);
                                                multiplecommandtimers.Add(Convert.ToInt32(Convert.ToDecimal(itembreakdown[7]) * 1000));
                                                multiplecommandtypes.Add(itembreakdown[6]);
                                                //commandorderlist.Add(itembreakdown[2]);
                                                Console.WriteLine(message + " Matches " + itembreakdown[1]);
                                            }
                                        }
                                    }


                                }
                            }
                        }
                        TwitchIRCTextbox.AppendText(userName + ": " + message + Environment.NewLine); // Print raw irc messages
                        commandorderlist.Add(message);
                        
                            

                        
                        //Check All Controls in List
                        //  foreach (string item in MasterControlList.Items) do
                        //  {
                        //  Console.WriteLine(item.ToString());
                        //}

                        //Omni-Command Check? (Checks for if you want commands to overlap or not, then plays the commands given) ***********************************************************************
                        if (UseMultipleCommandsSeparator.Checked == true & multiplecommands.Count() > 0)
                        {
                            //We're going to make clones of the lists needed so we don't run into errors in the new thread.
                            List<string> localmultiplecommands = new List<string>(multiplecommands);
                            List<string> localmultiplecommandtypes = new List<string>(multiplecommandtypes);
                            List<Int32> localmultiplecommandtimers = new List<Int32>(multiplecommandtimers);

                            if (AllowOverlappingTriggers.Checked == true)
                            {
                                
                                new Thread(() =>
                                {
                                    PlayCommands(localmultiplecommands, localmultiplecommandtypes, localmultiplecommandtimers, userName);

                                }).Start();
                            } else
                            {
                                Thread newinputthread = 
                                new Thread(() => 
                                { 
                                    PlayCommands(localmultiplecommands, localmultiplecommandtypes, localmultiplecommandtimers, userName);
                                });
                                newinputthread.Start();
                                newinputthread.Join();
                            }

                        }
                        //Checks if multi-commands are enabled and populated

                        /*
                        if (UseMultipleCommandsSeparator.Checked == true & multiplecommands.Count() > 0) //Checks if multi-commands are enabled and populated
                        {
                            int timingspacer = 0;
                            int currentcommandcount = 0;
                            //Thread newthread = null;

                            foreach (string singlecommand in multiplecommands)
                            {
                                 
                                Thread.Sleep(timingspacer + Decimal.ToInt32(InputUpDownDelay.Value) * 1000);
                                
                                Console.WriteLine("we Just waited: " + timingspacer.ToString() + " And will add " + multiplecommandtimers[currentcommandcount]);
                                timingspacer = multiplecommandtimers[currentcommandcount];
                                Console.WriteLine("Command Type: " + multiplecommandtypes[currentcommandcount]);
                                if (currentcommandcount+1 < multiplecommands.Count())
                                {
                                    nextcommand = multiplecommands[currentcommandcount + 1];
                                } else
                                {
                                    nextcommand = null;
                                }
                                if (multiplecommandtypes[currentcommandcount] == "Direct Press")
                                {
                                    Console.WriteLine("We are about to do a DIRECT PRESS");
                                    //Thread newinputthread = 
                                    new Thread(() =>
                                    {
                                        SimulateKeyPress(singlecommand, timingspacer, true);

                                    }).Start();
                                    //newinputthread.Start();
                                    //newinputthread.Join();
                                }
                                else if (multiplecommandtypes[currentcommandcount] == "Direct Hold")
                                {
                                    Console.WriteLine("We are about to press a DIRECT HOLD");
                                    new Thread(() =>
                                    {
                                        SimulateKeyHold(singlecommand, timingspacer, true);
                                    }).Start();
                                }
                                else if (multiplecommandtypes[currentcommandcount] == "Direct Stop")
                                {
                                    Console.WriteLine("We are about to press a DIRECT STOP");
                                    StopAllIncomingCommands();
                                    break;
                                }
                                else if (multiplecommandtypes[currentcommandcount] == "Simulated Press")
                                {
                                    Console.WriteLine("We are about to press a SIMULATED PRESS");
                                    new Thread(() =>
                                    {
                                        SimulateKeyPress(singlecommand, timingspacer, false);
                                    }).Start();
                                }
                                else if (multiplecommandtypes[currentcommandcount] == "Simulated Hold")
                                {
                                    Console.WriteLine("We are about to press a SIMULATED HOLD");
                                    new Thread(() =>
                                    {
                                        SimulateKeyHold(singlecommand, timingspacer, false);
                                    }).Start();
                                }
                                else if (multiplecommandtypes[currentcommandcount] == "Simulated Stop")
                                {
                                    Console.WriteLine("We are about to press a SIMULATED STOP");
                                    StopAllIncomingCommands();
                                    break;
                                }
                                currentcommandcount += 1;
                                Thread.Sleep(Decimal.ToInt32(InputUpDownDelay.Value) * 1000);
                            }

                            //if (AllowMultipleTriggersCheckbox.Checked == false)
                            //{
                            //    break;
                            //}
                        } */
                        if (commandorderlist.Count > 0)
                        {
                            commandorderlist.RemoveAt(0);
                        }
                    }
                }

                //} // end lurk check
            }
            Console.WriteLine("IRC is now Null");
            //e.Cancel = true;
            //if (BackgroundWorker.CancellationPending == true)
            //{
            //    Console.WriteLine("IRC is null, closing BackgroundWorker");
            //    e.Cancel = true;
            //}
                       
        }

        private void setGameWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processlist = Process.GetProcesses();
            WindowListPanel1.Visible = true;
            WindowListPanel1.BringToFront();
            foreach (Process process in processlist)
            {
                if (!String.IsNullOrEmpty(process.MainWindowTitle))
                {
                    ActiveWindowsBox.Items.Add(process.ProcessName + " - " + process.MainWindowTitle);
                    //ActiveWindowsBox.Items.Add("Process: {0} ID: {1} Window title: {2}" + process.ProcessName + process.Id + process.MainWindowTitle);
                    //Console.WriteLine("Process: {0} ID: {1} Window title: {2}", process.ProcessName, process.Id, process.MainWindowTitle);

                }
            }

        }

        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        private void ActiveWindowsBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            WindowListPanel1.Visible = false;
            WindowListPanel1.SendToBack();
            foreach (Process process in processlist)
            {
                if (!String.IsNullOrEmpty(process.MainWindowTitle))
                {
                    if ((process.ProcessName + " - " + process.MainWindowTitle) == ActiveWindowsBox.SelectedItem.ToString())
                    {
                        Process chosenprocess = Process.GetProcessesByName(process.ProcessName)[0];
                        selectedgamewindow = chosenprocess.MainWindowHandle;
                        SetForegroundWindow(selectedgamewindow);
                        ActiveWindowsBox.Items.Clear();
                        return;
                    }

                }
            }
        }

        private void ControlPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void MasterControlList_SelectedIndexChanged(object sender, EventArgs e)
        {
            //int intIndexParseSign = MasterControlList.Text.IndexOf('!');
            //CommandName.Text = intIndexParseSign = MasterControlList.IndexOf(" |");
            if (MasterControlList.SelectedItem != null)
            {
                string[] selectedcontrols = MasterControlList.SelectedItem.ToString().Split(Convert.ToChar("|"));
                CommandName.Text = selectedcontrols[0];
                CommandBox.Text = selectedcontrols[1];
                OutputBox.Text = selectedcontrols[2];
                CommandEnabledCheckbox.Checked = Convert.ToBoolean(selectedcontrols[3]);
                ModOnlyCheckBox.Checked = Convert.ToBoolean(selectedcontrols[4]);
                CommandCaseSensitiveBox.Checked = Convert.ToBoolean(selectedcontrols[5]);
                //PressOrHoldCheckBox.Checked = Convert.ToBoolean(selectedcontrols[5]);
                CommandTypeBox.Text = selectedcontrols[6];
                KeyPressDelay.Value = Convert.ToDecimal(selectedcontrols[7]);
            }
        }

        private void CommandEnabledCheckbox_OnClick(object sender, EventArgs e)
        {
            UpdateSelectedControl();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SettingsPanel1.Visible == true)
            {
                SettingsPanel1.Visible = false;
                SettingsPanel1.SendToBack();
            }
            else if (SettingsPanel1.Visible == false)
            {
                SettingsPanel1.Visible = true;
                SettingsPanel1.BringToFront();
            }
        }

        private void AddControlButton_Click(object sender, EventArgs e)
        {
            //Update Control Text
            var newindex = MasterControlList.Items.Add("Press A|a|Space|True|False|False|Direct Press|0.2");
            MasterControlList.SelectedIndex = newindex;
            UpdateSelectedControl();
        }

        private void DeleteControlButton_Click(object sender, EventArgs e)
        {
            MasterControlList.Items.Remove(MasterControlList.SelectedItem);
        }

        private void OutputBox_KeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
            Console.WriteLine(e.KeyCode);
            OutputBox.Text = e.KeyCode.ToString();
            UpdateSelectedControl();
        }

        private void PressOrHoldCheckBox_Click(object sender, EventArgs e)
        {
            UpdateSelectedControl();
        }

        private void CommandCaseSensitiveBox_Click(object sender, EventArgs e)
        {
            UpdateSelectedControl();
        }

        private void KeyPressDelay_ValueChanged(object sender, EventArgs e)
        {
            UpdateSelectedControl();
        }

        private void CommandBox_KeyUp(object sender, KeyEventArgs e)
        {
            UpdateSelectedControl();
        }

        private void CommandName_KeyUp(object sender, KeyEventArgs e)
        {
            UpdateSelectedControl();
        }

        private void CommandName_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                CommandBox.Focus();
            }
        }

        private void CommandBox_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                OutputBox.Focus();
            }
        }

        private void saveControlProfileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog ChooseDirectoryDialog = new SaveFileDialog();
            ChooseDirectoryDialog.Title = "Choose Your Save Location";
            ChooseDirectoryDialog.DefaultExt = "txt";
            ChooseDirectoryDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            //ChooseDirectoryDialog.ShowDialog();
            
            if (ChooseDirectoryDialog.ShowDialog() == DialogResult.OK)
            {
                StreamWriter SaveFile1 = new System.IO.StreamWriter(ChooseDirectoryDialog.FileName);
                foreach (Object item in MasterControlList.Items)
                {
                    SaveFile1.WriteLine(item.ToString());
                }
                SaveFile1.Close();
            }
            else
            {
                Console.WriteLine("No File Saved");
            }
        }

        private void loadControlProfileToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //StreamReader ReadFile1 = new System.IO.StreamReader("E:/file.txt");
            OpenFileDialog ChooseFileDialog = new OpenFileDialog();
            ChooseFileDialog.Title = "Choose Your Control Scheme";
            ChooseFileDialog.DefaultExt = "txt";
            ChooseFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            //ChooseFileDialog.ShowDialog();
            if (ChooseFileDialog.ShowDialog() == DialogResult.OK)
            {
                string[] codelist = File.ReadAllLines(ChooseFileDialog.FileName);
                MasterControlList.Items.Clear();
                foreach (string item in codelist)
                {
                    MasterControlList.Items.Add(item);
                }
            }
            else
            {
                Console.WriteLine("No File Chosen");
            }
            MasterControlList.SelectedIndex = 0;
            UpdateSelectedControl();
            //ReadFile1.Close();
        }

        private void TwitchIRCTextbox_TextChanged(object sender, EventArgs e)
        {

        }

        private void PressOrHoldCheckBox_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void CommandTypeBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateSelectedControl();
        }

        private void TwitchUsernameTextbox_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.TwitchUsername = TwitchUsernameTextbox.Text;
            Properties.Settings.Default.Save();
        }

        private void TwitchChannelNameTextbox_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.TwitchChannelName = TwitchChannelNameTextbox.Text;
            Properties.Settings.Default.Save();
        }

        private void TwitchOAuthTextbox_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.TwitchOAuthCode = TwitchOAuthTextbox.Text;
            Properties.Settings.Default.Save();
        }

        private void TwitchMessageText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                if (twitchClient != null)
                {
                    twitchClient.SendPublicChatMessage(TwitchMessageText.Text);
                    TwitchIRCTextbox.Text = TwitchIRCTextbox.Text + ("YOU SAID: " + TwitchMessageText.Text + Environment.NewLine);
                    TwitchMessageText.Text = "";
                }
            }

        }

        private void CheckIRCChatInfo_Tick(object sender, EventArgs e)
        {

        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            //splitContainer1.SplitterDistance = (Form1.ActiveForm.Size.Width / 2);
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Console.WriteLine("BackgroundWorker Complete!");
        }

        private void AddModButton_Click(object sender, EventArgs e)
        {
            if (AddModTextBox.Text != "")
            {
                var newindex = ModList.Items.Add(AddModTextBox.Text);
                ModList.SelectedIndex = newindex;

                if (Properties.Settings.Default.TwitchModList != null)
                {
                    Properties.Settings.Default.TwitchModList.Clear();
                    foreach (string item in ModList.Items)
                    {
                        Console.WriteLine("New Mod? " + item);
                        Properties.Settings.Default.TwitchModList.Add(item);
                    }
                    Properties.Settings.Default.Save();
                }
            }
        }

        private void ModOnlyCheckBox_Click(object sender, EventArgs e)
        {
            UpdateSelectedControl();
        }

        private void DeleteModButton_Click(object sender, EventArgs e)
        {
            ModList.Items.Remove(ModList.SelectedItem);
            if (Properties.Settings.Default.TwitchModList != null)
            {
                Properties.Settings.Default.TwitchModList.Clear();
                foreach (string item in ModList.Items)
                {
                    Properties.Settings.Default.TwitchModList.Add(item);
                }
                Properties.Settings.Default.Save();
            }
            
        }

        private void AddLurkerButton_Click(object sender, EventArgs e)
        {
            if (AddLurkerTextBox.Text != "")
            {
                var newindex = LurkList.Items.Add(AddLurkerTextBox.Text);
                LurkList.SelectedIndex = newindex;

                if (Properties.Settings.Default.TwitchLurkList != null)
                {
                    Properties.Settings.Default.TwitchLurkList.Clear();
                    foreach (string item in LurkList.Items)
                    {
                        Console.WriteLine("New Lurker? " + item);
                        Properties.Settings.Default.TwitchLurkList.Add(item);
                    }
                    Properties.Settings.Default.Save();
                }
            }
        }

        private void DeleteLurkerButton_Click(object sender, EventArgs e)
        {
            LurkList.Items.Remove(LurkList.SelectedItem);
            if (Properties.Settings.Default.TwitchLurkList != null)
            {
                Properties.Settings.Default.TwitchLurkList.Clear();
                foreach (string item in LurkList.Items)
                {
                    Properties.Settings.Default.TwitchLurkList.Add(item);
                }
                Properties.Settings.Default.Save();
            }
        }

        private void macrosToolStripMenuItem_Click(object sender, EventArgs e)
        {
                MacroPanel1.Visible = true;
                MacroPanel1.BringToFront();
        }

        private void controlsToolStripMenuItem_Click(object sender, EventArgs e)
        {
                ControlPanel1.Visible = true;
                ControlPanel1.BringToFront();
        }

        private void SettingsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SettingsComboBox.SelectedItem.ToString() == "Behavior")
            {
                SettingsSubPanel1.Visible = true;
                SettingsSubPanel1.BringToFront();
            } else {
                
                //SettingsSubPanel1.Visible = false;
                SettingsSubPanel1.SendToBack();
            }
            if (SettingsComboBox.SelectedItem.ToString() == "Controls")
            {
                ControlsSubPanel1.Visible = true;
                ControlsSubPanel1.BringToFront();
            }
            else
            {
                ControlsSubPanel1.Visible = false;
                ControlsSubPanel1.SendToBack();
            }
            if (SettingsComboBox.SelectedItem.ToString() == "Macros")
            {
                MacrosSubPanel1.Visible = true;
                MacrosSubPanel1.BringToFront();
            }
            else
            {
                MacrosSubPanel1.Visible = false;
                MacrosSubPanel1.SendToBack();
            }
            if (SettingsComboBox.SelectedItem.ToString() == "Reminders")
            {
                RemindersSubPanel1.Visible = true;
                RemindersSubPanel1.BringToFront();
            }
            else
            {
                RemindersSubPanel1.Visible = false;
                RemindersSubPanel1.SendToBack();
            }
        }

        private void remindersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemindersPanel1.Visible = true;
            RemindersPanel1.BringToFront();
        }

        private void rafflesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RafflesPanel1.Visible = true;
            RafflesPanel1.BringToFront();
        }

        private void RemindersListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //int intIndexParseSign = MasterControlList.Text.IndexOf('!');
            //CommandName.Text = intIndexParseSign = MasterControlList.IndexOf(" |");
            if (RemindersListBox.SelectedItem != null)
            {
                string[] selectedreminders = RemindersListBox.SelectedItem.ToString().Split(Convert.ToChar("|"));
                ReminderNameBox.Text = selectedreminders[0];
                ReminderTypeBox.Text = selectedreminders[1];
                ReminderTimerBox.Text = selectedreminders[2];
                ReminderTextBox.Text = selectedreminders[3];
            }
        }

        private void AddReminderButton_Click(object sender, EventArgs e)
        {
            var newindex = RemindersListBox.Items.Add("Reminder|Trigger|600|Just wanted to remind you guys about our discord! https://discord.gg/dUB7E9UrMS");
            RemindersListBox.SelectedIndex = newindex;
            UpdateSelectedReminder();
            UpdateControlTypeBox();
        }

        private void RemoveReminderButton_Click(object sender, EventArgs e)
        {
            RemindersListBox.Items.Remove(RemindersListBox.SelectedItem);
            UpdateControlTypeBox();
        }

        private void ReminderNameBox_TextChanged(object sender, EventArgs e)
        {
            UpdateSelectedReminder();
        }

        private void ReminderTypeBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateSelectedReminder();
        }

        private void ReminderTimerBox_ValueChanged(object sender, EventArgs e)
        {
            UpdateSelectedReminder();
        }

        private void ReminderTextBox_TextChanged(object sender, EventArgs e)
        {
            UpdateSelectedReminder();
        }

        private void saveReminderProfileToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            SaveFileDialog ChooseDirectoryDialog = new SaveFileDialog();
            ChooseDirectoryDialog.Title = "Choose Your Save Location";
            ChooseDirectoryDialog.DefaultExt = "txt";
            ChooseDirectoryDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            //ChooseDirectoryDialog.ShowDialog();
            if (ChooseDirectoryDialog.ShowDialog() == DialogResult.OK)
            {
                StreamWriter SaveFile1 = new System.IO.StreamWriter(ChooseDirectoryDialog.FileName);
                foreach (Object item in RemindersListBox.Items)
                {
                    SaveFile1.WriteLine(item.ToString());
                }
                SaveFile1.Close();
            }
            else
            {
                Console.WriteLine("No File Saved");
            }
            UpdateSelectedReminder();
        }

        private void loadReminderProfileToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            //StreamReader ReadFile1 = new System.IO.StreamReader("E:/file.txt");
            OpenFileDialog ChooseFileDialog = new OpenFileDialog();
            ChooseFileDialog.Title = "Choose Your Control Scheme";
            ChooseFileDialog.DefaultExt = "txt";
            ChooseFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            //ChooseFileDialog.ShowDialog();
            
            
            if (ChooseFileDialog.ShowDialog() == DialogResult.OK) {
                string[] codelist = File.ReadAllLines(ChooseFileDialog.FileName);
                RemindersListBox.Items.Clear();
                foreach (string item in codelist)
                {
                    RemindersListBox.Items.Add(item);
                }
            } else
            {
                Console.WriteLine("No File Chosen");
            }
            RemindersListBox.SelectedIndex = 0;
            UpdateControlTypeBox();
            UpdateSelectedReminder();
            //ReadFile1.Close();
        }

        private void ReminderActivityTimer_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.ReminderActivityTimer = ReminderActivityTimer.Value;
            Properties.Settings.Default.Save();
        }

        private void OAuthCodeButton_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://twitchapps.com/tmi/"); //It's where I get my code anyway... There are other places to aquire it.
        }

        private void InputUpDownDelay_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.ApptoAppDelay = InputUpDownDelay.Value;
            Properties.Settings.Default.Save();
        }

        private void ReconnectTimeBox_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.ReconnectActivityTimer = ReconnectTimeBox.Value;
            Properties.Settings.Default.Save();
        }

        private void ReconnectDelayTimeBox_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.ReconnectDelayTimer = ReconnectDelayTimeBox.Value;
            Properties.Settings.Default.Save();
        }

        private void AutoReconnectCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.ReconnectCheckBox = AutoReconnectCheckBox.Checked;
            Properties.Settings.Default.Save();
        }

        private void EnableLurkingCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.EnableLurkingCheckBox = EnableLurkingCheckBox.Checked;
            Properties.Settings.Default.Save();
        }

        private void AllowMultipleTriggersCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.AllowMultipleTriggersCheckBox = AllowMultipleTriggersCheckbox.Checked;
            Properties.Settings.Default.Save();
        }

        private void AllowOverlappingTriggers_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.AllowOverlapCheckBox = AllowOverlappingTriggers.Checked;
            Properties.Settings.Default.Save();
        }

        private void TwitchChatClient_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.TwitchChatClient = TwitchChatClientChoice.Items[TwitchChatClientChoice.SelectedIndex].ToString();
            Properties.Settings.Default.Save();
        }
    }

    //This class is for Direct Input for some games
    public class MasterDirectInput
    {
        [DllImport("user32.dll")]
        static extern UInt32 SendInput(UInt32 nInputs, [MarshalAs(UnmanagedType.LPArray, SizeConst = 1)] INPUT[] pInputs, Int32 cbSize);

        [StructLayout(LayoutKind.Sequential)]
        struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public int mouseData;
            public int dwFlags;
            public int time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct KEYBDINPUT
        {
            public short wVk;
            public short wScan;
            public int dwFlags;
            public int time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct HARDWAREINPUT
        {
            public int uMsg;
            public short wParamL;
            public short wParamH;
        }

        [StructLayout(LayoutKind.Explicit)]
        struct INPUT
        {
            [FieldOffset(0)]
            public int type;
            [FieldOffset(4)]
            public MOUSEINPUT mi;
            [FieldOffset(4)]
            public KEYBDINPUT ki;
            [FieldOffset(4)]
            public HARDWAREINPUT hi;
        }

        const int KEYEVENTF_EXTENDEDKEY = 0x0001;
        const int KEYEVENTF_KEYUP = 0x0002;
        const int KEYEVENTF_UNICODE = 0x0004;
        const int KEYEVENTF_SCANCODE = 0x0008;


        public void Send_Key(short Keycode, int KeyUporDown)
        {
            INPUT[] InputData = new INPUT[1];

            InputData[0].type = 1;
            InputData[0].ki.wScan = Keycode;
            InputData[0].ki.dwFlags = KeyUporDown;
            InputData[0].ki.time = 0;
            InputData[0].ki.dwExtraInfo = IntPtr.Zero;

            Console.WriteLine(InputData[0].ki.wScan.ToString() + Marshal.SizeOf(typeof(INPUT)));
            SendInput(1, InputData, Marshal.SizeOf(typeof(INPUT)));
        }

    }
}
