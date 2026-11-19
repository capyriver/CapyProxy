using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using System.Diagnostics;
using System.Management;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Markup;
using System.Windows.Threading;
using Microsoft.Win32;
using System.Reflection;
using Forms=System.Windows.Forms;

class Studio {
 static readonly string binary=@"C:\gost\gost.exe", local="http://127.0.0.1:8080";
 static readonly string profile=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"CapyProxy","profile");
 const string runKey=@"Software\Microsoft\Windows\CurrentVersion\Run";
 static string self { get{return Assembly.GetExecutingAssembly().Location;} }
 Window w; Forms.NotifyIcon tray; string proto, savedHost,savedPort,savedProto; bool busy,checking,active,init=true; int generation;
 readonly DispatcherTimer timer=new DispatcherTimer();
 T C<T>(string n) where T:FrameworkElement {return (T)w.FindName(n);}
 void Click(string n,Action action){C<Button>(n).Click+=(s,e)=>{try{action();}catch(Exception ex){Msg(ex.Message);}};}
 void Msg(string m){C<TextBlock>("Notice").Text=m;var log=C<TextBox>("Events");log.Text=(DateTime.Now.ToString("HH:mm:ss")+"   "+m+"\n"+log.Text).TrimEnd();if(log.Text.Length>24000)log.Text=log.Text.Substring(0,24000);}
 void Text(string n,string val){C<TextBlock>(n).Text=val;}
 static Brush B(string color){return (Brush)new BrushConverter().ConvertFromString(color);}
 void Paint(string n,bool selected){var b=C<Button>(n);b.Background=B(selected?"#373A42":"#18191D");b.Foreground=B(selected?"#FFFFFF":"#BABCC5");}
 void Page(string id){foreach(string p in new[]{"Home","Profile","Log"}){C<StackPanel>(p).Visibility=p==id?Visibility.Visible:Visibility.Collapsed;Paint("Nav"+p,p==id);}Text("Heading",id=="Home"?"Обзор":id=="Profile"?"Профиль":"Журнал");}
 void Protocol(string p){proto=p;Paint("Socks",p=="socks5");Paint("Http",p=="http");}
 string Box(string name){return C<TextBox>(name).Text;}
 string Password{get{return C<CheckBox>("Reveal").IsChecked==true?Box("Plain"):C<PasswordBox>("Secret").Password;}}
 void Load(){string[] v={"","","","","socks5"};if(File.Exists(profile))v=Encoding.UTF8.GetString(ProtectedData.Unprotect(File.ReadAllBytes(profile),null,DataProtectionScope.CurrentUser)).Split('\n');if(v.Length!=5)throw new Exception("Не удалось прочитать профиль.");savedHost=v[0];savedPort=v[1];savedProto=v[4];C<TextBox>("Host").Text=v[0];C<TextBox>("Port").Text=v[1];C<TextBox>("User").Text=v[2];C<PasswordBox>("Secret").Password=v[3];Protocol(v[4]);Server();}
 void Server(){Text("ServerInfo",string.IsNullOrWhiteSpace(savedHost)?"Заполните профиль":savedProto.ToUpperInvariant()+"   "+savedHost+":"+savedPort);}
 void Persist(){var h=Box("Host").Trim();int port;if(Uri.CheckHostName(h)==UriHostNameType.Unknown||!int.TryParse(Box("Port"),out port)||port<1||port>65535)throw new Exception("Проверьте адрес сервера и порт.");var values=new[]{h,port.ToString(),Box("User"),Password,proto};Directory.CreateDirectory(Path.GetDirectoryName(profile));var temp=profile+"."+Guid.NewGuid().ToString("N");File.WriteAllBytes(temp,ProtectedData.Protect(Encoding.UTF8.GetBytes(string.Join("\n",values)),null,DataProtectionScope.CurrentUser));if(File.Exists(profile))File.Replace(temp,profile,null);else File.Move(temp,profile);savedHost=h;savedPort=values[1];savedProto=proto;Server();}
 static int[] Owned(){using(var q=new ManagementObjectSearcher("SELECT ProcessId,ExecutablePath,CommandLine FROM Win32_Process WHERE Name='gost.exe'"))using(var rows=q.Get()){return rows.Cast<ManagementObject>().Where(m=>string.Equals(Convert.ToString(m["ExecutablePath"]),binary,StringComparison.OrdinalIgnoreCase)&&(" "+Convert.ToString(m["CommandLine"])+" ").Contains(" -L "+local+" ")).Select(m=>Convert.ToInt32(m["ProcessId"])).ToArray();}}
 void Render(bool running){active=running;Text("State",running?"Прокси включён":"Прокси выключен");C<Button>("Toggle").Content=running?"Отключить":"Подключить";tray.Text=running?"CapyProxy · включён":"CapyProxy · выключен";C<Button>("CheckIp").IsEnabled=running&&!checking;if(!running)Text("Ip","—");}
 async Task Change(bool start){if(busy)return;busy=true;generation++;C<Button>("Toggle").IsEnabled=false;try{if(start&&!File.Exists(profile)){Page("Profile");throw new Exception("Сначала сохраните профиль подключения.");}var raw=start?Encoding.UTF8.GetString(ProtectedData.Unprotect(File.ReadAllBytes(profile),null,DataProtectionScope.CurrentUser)).Split('\n'):null;await Task.Run(()=>{var ids=Owned();if(!start){foreach(int id in ids){using(var p=Process.GetProcessById(id)){p.Kill();p.WaitForExit(3500);}}return;}if(ids.Length>0)return;if(!File.Exists(binary))throw new Exception("Модуль GOST не найден в C:\\gost.");var auth=raw[2].Length==0?"":Uri.EscapeDataString(raw[2])+":"+Uri.EscapeDataString(raw[3])+"@";string h=raw[0];if(h.Contains(":")&&!h.StartsWith("["))h="["+h+"]";using(var p=Process.Start(new ProcessStartInfo(binary,"-L "+local+" -F "+raw[4]+"://"+auth+h+":"+raw[1]){UseShellExecute=false,CreateNoWindow=true})){if(p.WaitForExit(550))throw new Exception("GOST завершился. Проверьте, свободен ли порт 8080.");}});Render(await Task.Run(()=>Owned().Length>0));Msg(active?"Локальный прокси запущен.":"Соединение отключено.");}catch(Exception ex){Msg(ex.Message);}finally{busy=false;C<Button>("Toggle").IsEnabled=true;}}
 async void Verify(){if(checking||busy)return;int g=generation;checking=true;C<Button>("CheckIp").IsEnabled=false;Text("Ip","Проверяем…");try{var watch=Stopwatch.StartNew();var ip=await Task.Run(()=>{var q=(HttpWebRequest)WebRequest.Create("https://api.ipify.org");q.Proxy=new WebProxy(local,false);q.Timeout=12000;q.ReadWriteTimeout=12000;using(var r=q.GetResponse())using(var s=new StreamReader(r.GetResponseStream())){var result=s.ReadToEnd().Trim();IPAddress a;if(!IPAddress.TryParse(result,out a))throw new Exception();return result;}});if(g==generation){Text("Ip",ip);Msg("Проверка через прокси прошла за "+watch.ElapsedMilliseconds+" мс.");}}catch{if(g==generation){Text("Ip","Недоступен");Msg("Проверочный запрос через прокси не прошёл.");}}finally{checking=false;C<Button>("CheckIp").IsEnabled=active;}}
 void SetAuto(bool value){if(init)return;try{if(value&&!File.Exists(profile))Persist();using(var k=Registry.CurrentUser.CreateSubKey(runKey)){if(value)k.SetValue("CapyProxy","\""+self+"\" /auto");else k.DeleteValue("CapyProxy",false);}Msg(value?"Автоподключение включено.":"Автоподключение выключено.");}catch(Exception e){init=true;C<CheckBox>("Auto").IsChecked=!value;init=false;Msg(e.Message);}}
 void Restore(){w.Show();w.WindowState=WindowState.Normal;w.Activate();}
 static BitmapImage Asset(string n){var b=new BitmapImage();b.BeginInit();b.CacheOption=BitmapCacheOption.OnLoad;using(var s=Assembly.GetExecutingAssembly().GetManifestResourceStream(n)){b.StreamSource=s;b.EndInit();}b.Freeze();return b;}
 void Preview(string path){w.UpdateLayout();var root=(FrameworkElement)w.Content;var dpi=VisualTreeHelper.GetDpi(root);var bmp=new RenderTargetBitmap((int)Math.Ceiling(root.ActualWidth*dpi.DpiScaleX),(int)Math.Ceiling(root.ActualHeight*dpi.DpiScaleY),dpi.PixelsPerInchX,dpi.PixelsPerInchY,PixelFormats.Pbgra32);bmp.Render(root);var enc=new PngBitmapEncoder();enc.Frames.Add(BitmapFrame.Create(bmp));using(var f=File.Create(path))enc.Save(f);}
 async void Poll(){if(busy)return;int g=generation;try{bool on=await Task.Run(()=>Owned().Length>0);if(g==generation&&!busy)Render(on);}catch{Text("Notice","Не удалось прочитать состояние GOST.");}}
 public Studio(string[] args){ServicePointManager.SecurityProtocol=SecurityProtocolType.Tls12;var app=new Application();using(var s=Assembly.GetExecutingAssembly().GetManifestResourceStream("ui"))w=(Window)XamlReader.Load(s);
 w.MaxHeight=SystemParameters.WorkArea.Height;w.MaxWidth=SystemParameters.WorkArea.Width;
 w.Icon=Asset("ghost");C<Image>("Ghost").Source=w.Icon;tray=new Forms.NotifyIcon{Icon=System.Drawing.Icon.ExtractAssociatedIcon(self),Visible=true,Text="CapyProxy"};tray.DoubleClick+=(s,e)=>Restore();var menu=new Forms.ContextMenuStrip();menu.Items.Add("Открыть CapyProxy",null,(s,e)=>Restore());menu.Items.Add("Проверить IP",null,(s,e)=>{Restore();Page("Home");Verify();});menu.Items.Add("Закрыть панель",null,(s,e)=>w.Close());tray.ContextMenuStrip=menu;
 foreach(var p in new[]{"Home","Profile","Log"}){string page=p;Click("Nav"+page,()=>Page(page));}Click("TrayBtn",()=>w.Hide());Click("CheckIp",Verify);Click("Copy",()=>{Clipboard.SetText(local);Msg("Адрес скопирован.");});Click("Clear",()=>{C<TextBox>("Events").Clear();Text("Notice","Журнал очищен.");});Click("Save",()=>{Persist();Msg(active?"Профиль сохранён. Переподключитесь, чтобы применить изменения.":"Профиль сохранён.");});Click("Http",()=>Protocol("http"));Click("Socks",()=>Protocol("socks5"));Click("Toggle",async ()=>await Change(!active));
 var reveal=C<CheckBox>("Reveal");var secret=C<PasswordBox>("Secret");var plain=C<TextBox>("Plain");reveal.Checked+=(s,e)=>{plain.Text=secret.Password;plain.Visibility=Visibility.Visible;secret.Visibility=Visibility.Collapsed;};reveal.Unchecked+=(s,e)=>{secret.Password=plain.Text;plain.Clear();secret.Visibility=Visibility.Visible;plain.Visibility=Visibility.Collapsed;};
 var auto=C<CheckBox>("Auto");using(var k=Registry.CurrentUser.OpenSubKey(runKey))auto.IsChecked=k!=null&&k.GetValue("CapyProxy")!=null;auto.Checked+=(s,e)=>SetAuto(true);auto.Unchecked+=(s,e)=>SetAuto(false);init=false;
 Load();Page("Home");Render(Owned().Length>0);timer.Interval=TimeSpan.FromSeconds(5);timer.Tick+=(s,e)=>Poll();timer.Start();
 w.ContentRendered+=async (s,e)=>{if(args.Contains("/auto")){await Change(true);if(active)w.Hide();}var preview=Environment.GetEnvironmentVariable("CAPY_PREVIEW");if(!string.IsNullOrEmpty(preview))Preview(preview);};w.StateChanged+=(s,e)=>{if(w.WindowState==WindowState.Minimized)w.Hide();};w.Closed+=(s,e)=>{timer.Stop();tray.Dispose();app.Shutdown();};app.Run(w);}
 [STAThread]public static void Main(string[] args){try{new Studio(args);}catch(Exception ex){MessageBox.Show(ex.GetBaseException().Message,"CapyProxy");}}
}

