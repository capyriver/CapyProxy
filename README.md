# CapyProxy

**A small Windows desktop control panel for an upstream proxy.** CapyProxy starts and stops a local HTTP endpoint, keeps the upstream profile on the current Windows account, and can check the public IP seen by a request routed through that endpoint.

> The application is a client-side controller. It does not provide proxy servers, VPN service, accounts, credentials, or guaranteed access to any website. You must supply a proxy endpoint that you are authorized to use.

## What it does

- Configure an upstream HTTP or SOCKS5 endpoint, including optional username and password.
- Expose the local endpoint at `http://127.0.0.1:8080` for compatible applications.
- Save the profile encrypted with Windows DPAPI for the current user account.
- Start or stop the bridge from the main panel; minimize the window to the notification area.
- Optionally start the connection when the current user signs in to Windows.
- Check the public IP returned by `api.ipify.org` through the local endpoint.
- Keep a small in-app session event log.

The current application interface is in Russian. This README is available in English, 简体中文, Русский and Қазақша.

## Requirements

- Windows 10 or Windows 11.
- .NET Framework 4.x compiler and WPF assemblies available on the machine (the build script uses the Windows Framework compiler).
- GOST 3.x installed at `C:\gost\gost.exe`. GOST is a separate third-party program and is not bundled here.
- A working upstream HTTP or SOCKS5 proxy that permits your use.

## Build from source

Open PowerShell in the repository directory and run:

```powershell
Set-ExecutionPolicy -Scope Process Bypass
./Build.ps1
```

The output is written to `dist\CapyProxy.exe`. The executable is intentionally ignored by Git; this repository publishes source and artwork, not a prebuilt binary.

## First run

1. Install GOST separately from its official project and place `gost.exe` at `C:\gost\gost.exe`.
2. Build and launch CapyProxy.
3. Open **Профиль**, enter your own upstream server, port, protocol and optional credentials, then save.
4. Select **Подключить**. Compatible applications can use `http://127.0.0.1:8080` as their HTTP proxy.
5. Select **Проверить IP** to check the public address reported through the bridge.

The app does not switch a VPN on or off and does not change the Windows-wide proxy setting. The auto-start option registers CapyProxy for the current Windows user only.

## Security and privacy

- The repository contains no proxy endpoint, username, password, API key, token, or user profile.
- A fresh profile is blank. Credentials are entered by each user and saved locally using Windows DPAPI scoped to that Windows account.
- Do not commit personal proxy settings, exported profiles, logs, or compiled executables.
- The bridge listens on loopback (`127.0.0.1`), not on all network interfaces.
- The IP check sends a request to `api.ipify.org` through the configured local proxy. That service sees the request and the proxy's egress address.
- The current implementation passes the upstream URL to the GOST process when launching it. On a shared machine, other local users or administrators may be able to inspect process command lines. Avoid using credentials on an untrusted shared Windows account.
- CapyProxy is not a VPN, anonymity guarantee, or anti-ban tool. Follow the proxy provider's terms and the destination service's rules.

## Project layout

```text
Build.ps1       Windows build script
src/Studio.cs   Application logic
src/Studio.xaml WPF interface
assets/         Ghost artwork and Windows icon
```

## Third-party software

GOST is not included. Obtain it from the official [go-gost/gost project](https://github.com/go-gost/gost) and review its current license and release notes before use. CapyProxy is an independent project and is not affiliated with the GOST maintainers.

## License

CapyProxy source in this repository is provided under the MIT License; see [LICENSE](LICENSE). Third-party software and artwork may have separate terms.

---

## 简体中文

**CapyProxy 是一个用于管理上游代理的 Windows 桌面小工具。** 它可以启动或停止本地 HTTP 代理入口，在当前 Windows 账户下保存上游配置，并检查通过该入口发出的请求所显示的公网 IP。

> 本程序只是客户端控制面板，不提供代理服务器、VPN、账户、凭据，也不保证能够访问任何网站。请自行提供有权使用的代理服务。

### 功能

- 配置上游 HTTP 或 SOCKS5 代理，可选填写用户名和密码。
- 为兼容的应用提供本地入口 `http://127.0.0.1:8080`。
- 使用当前 Windows 用户的 DPAPI 加密保存配置。
- 在主窗口中连接或断开；最小化后驻留系统托盘。
- 可选在当前 Windows 用户登录时自动启动代理。
- 通过本地入口请求 `api.ipify.org` 检查公网 IP。
- 在应用内查看本次运行的事件记录。

当前应用界面为俄语。本 README 提供 English、简体中文、Русский 和 Қазақша版本。

### 系统要求

- Windows 10 或 Windows 11。
- 系统可用的 .NET Framework 4.x 编译器及 WPF 程序集；构建脚本使用 Windows 自带的 Framework 编译器。
- GOST 3.x 单独安装在 `C:\gost\gost.exe`。本仓库不包含 GOST。
- 一个允许你使用的 HTTP 或 SOCKS5 上游代理。

### 从源码构建

在仓库目录打开 PowerShell 并运行：

```powershell
Set-ExecutionPolicy -Scope Process Bypass
./Build.ps1
```

程序输出到 `dist\CapyProxy.exe`。Git 会忽略该可执行文件；本仓库发布源码和美术资源，不发布预编译程序。

### 首次使用

1. 从 GOST 官方项目单独获取并安装 GOST，将 `gost.exe` 放到 `C:\gost\gost.exe`。
2. 构建并启动 CapyProxy。
3. 打开 **Профиль**，填写自己的代理地址、端口、协议及可选凭据，然后保存。
4. 点击 **Подключить**。兼容应用可将 `http://127.0.0.1:8080` 设为 HTTP 代理。
5. 点击 **Проверить IP**，查看通过代理入口返回的公网地址。

本程序不会开关 VPN，也不会更改 Windows 全局代理设置。自动启动选项只为当前 Windows 用户注册启动项。

### 安全与隐私

- 仓库不含代理地址、用户名、密码、API 密钥、令牌或用户配置文件。
- 新配置为空。每位用户自行输入凭据；配置使用 Windows DPAPI 并绑定当前 Windows 账户保存在本机。
- 请勿提交个人代理设置、导出的配置、日志或编译后的程序。
- 本地代理仅监听回环地址 `127.0.0.1`，不会监听所有网络接口。
- IP 检查会通过配置的本地代理向 `api.ipify.org` 发送请求；该服务可以看到请求和代理出口地址。
- 当前实现启动 GOST 时会把上游地址传给进程。共享电脑上的其他本地用户或管理员可能查看进程命令行；不要在不可信的共享 Windows 账户中使用凭据。
- CapyProxy 不是 VPN，不保证匿名或避免封禁。请遵守代理服务商条款和目标服务规则。

### 目录结构

```text
Build.ps1       Windows 构建脚本
src/Studio.cs   应用逻辑
src/Studio.xaml WPF 界面
assets/         幽灵图案和 Windows 图标
```

### 第三方软件与许可

本仓库不包含 GOST。请从 [go-gost/gost 官方项目](https://github.com/go-gost/gost) 获取，并在使用前查看当前许可证和发行说明。CapyProxy 是独立项目，与 GOST 维护者无关联。

本仓库中的 CapyProxy 源码采用 MIT 许可证，详见 [LICENSE](LICENSE)。第三方软件和美术资源可能适用其他条款。

---

## Русский

**CapyProxy — небольшая настольная панель Windows для управления вышестоящим прокси.** Она запускает и останавливает локальную HTTP-точку, сохраняет профиль для текущей учётной записи Windows и проверяет внешний IP запроса, прошедшего через эту точку.

> Приложение — только клиентская панель. Оно не предоставляет прокси-сервер, VPN, аккаунты или учётные данные и не гарантирует доступ к сайтам. Укажите прокси, которым вы вправе пользоваться.

### Возможности

- Настройка вышестоящего HTTP- или SOCKS5-прокси с необязательными логином и паролем.
- Локальный адрес `http://127.0.0.1:8080` для совместимых приложений.
- Шифрование профиля средствами Windows DPAPI для текущей учётной записи.
- Подключение и отключение из главного окна; сворачивание в системный трей.
- Необязательный автозапуск прокси при входе текущего пользователя Windows.
- Проверка внешнего IP запросом к `api.ipify.org` через локальную точку.
- Журнал событий текущего сеанса внутри приложения.

Сейчас интерфейс приложения доступен на русском языке. README также переведён на English, 简体中文 и Қазақша.

### Требования

- Windows 10 или Windows 11.
- Доступные в системе компилятор .NET Framework 4.x и сборки WPF; скрипт сборки использует Framework Compiler из Windows.
- GOST 3.x, установленный отдельно по пути `C:\gost\gost.exe`. В репозитории GOST отсутствует.
- Рабочий вышестоящий HTTP- или SOCKS5-прокси, которым разрешено пользоваться.

### Сборка из исходников

Откройте PowerShell в каталоге репозитория и выполните:

```powershell
Set-ExecutionPolicy -Scope Process Bypass
./Build.ps1
```

Результат появится в `dist\CapyProxy.exe`. Исполняемый файл исключён из Git: репозиторий публикует исходники и графику, но не готовый бинарный файл.

### Первый запуск

1. Отдельно установите GOST из официального проекта и поместите `gost.exe` в `C:\gost\gost.exe`.
2. Соберите и запустите CapyProxy.
3. Откройте **Профиль**, укажите собственные сервер, порт, протокол и при необходимости учётные данные, затем сохраните настройки.
4. Нажмите **Подключить**. Совместимые приложения могут использовать `http://127.0.0.1:8080` как HTTP-прокси.
5. Нажмите **Проверить IP**, чтобы проверить внешний адрес через локальную точку.

Приложение не включает и не выключает VPN и не меняет системную настройку прокси Windows. Автозапуск регистрируется только для текущего пользователя Windows.

### Безопасность и конфиденциальность

- В репозитории нет прокси-адреса, логина, пароля, API-ключа, токена или пользовательского профиля.
- Новый профиль пустой. Пользователь вводит свои данные самостоятельно; настройки шифруются Windows DPAPI и хранятся локально для текущей учётной записи Windows.
- Не добавляйте в репозиторий личные прокси-настройки, экспортированные профили, журналы и собранные исполняемые файлы.
- Локальная точка слушает только loopback-адрес `127.0.0.1`, а не все сетевые интерфейсы.
- Проверка IP отправляет запрос к `api.ipify.org` через настроенный локальный прокси. Сервис видит запрос и адрес выхода прокси.
- Текущая реализация передаёт адрес прокси процессу GOST при запуске. Другие локальные пользователи или администраторы общего компьютера могут просматривать командную строку процесса. Не вводите учётные данные на недоверенной общей учётной записи Windows.
- CapyProxy не является VPN, не гарантирует анонимность и не помогает гарантированно избежать блокировок. Соблюдайте условия провайдера прокси и правила сайтов.

### Структура проекта

```text
Build.ps1       Скрипт сборки для Windows
src/Studio.cs   Логика приложения
src/Studio.xaml Интерфейс WPF
assets/         Изображение призрака и значок Windows
```

### Стороннее ПО и лицензия

GOST в репозиторий не включён. Получайте его из [официального проекта go-gost/gost](https://github.com/go-gost/gost) и перед использованием проверяйте актуальные лицензию и примечания к выпуску. CapyProxy — независимый проект, не связанный с сопровождающими GOST.

Исходный код CapyProxy в этом репозитории распространяется по лицензии MIT; подробности в файле [LICENSE](LICENSE). Для стороннего ПО и графики могут действовать отдельные условия.

---

## Қазақша

**CapyProxy — Windows жүйесіндегі сыртқы проксиді басқаруға арналған шағын жұмыс үстелі панелі.** Ол жергілікті HTTP нүктесін іске қосып және тоқтатып, прокси профилін ағымдағы Windows тіркелгісі үшін сақтайды әрі осы нүкте арқылы жіберілген сұраудың сыртқы IP мекенжайын тексере алады.

> Бұл бағдарлама тек клиенттік басқару панелі. Ол прокси серверін, VPN қызметін, тіркелгілерді немесе логин мен құпиясөзді ұсынбайды және сайттарға қолжетімділікке кепілдік бермейді. Өзіңіз пайдалануға құқығыңыз бар проксиді енгізіңіз.

### Мүмкіндіктері

- HTTP немесе SOCKS5 сыртқы проксиін және қажет болса логин мен құпиясөзді баптау.
- Үйлесімді қолданбалар үшін `http://127.0.0.1:8080` жергілікті мекенжайын ұсыну.
- Профильді Windows DPAPI арқылы ағымдағы пайдаланушы тіркелгісіне арнап шифрлап сақтау.
- Негізгі терезеден қосу және ажырату; терезені жүйелік трейге жинау.
- Ағымдағы Windows пайдаланушысы кіргенде автоматты қосылуды таңдау.
- Жергілікті нүкте арқылы `api.ipify.org` сұрауымен сыртқы IP мекенжайын тексеру.
- Қолданба ішінде ағымдағы сеанс оқиғаларының журналын жүргізу.

Қолданба интерфейсі әзірге орыс тілінде. README мәтіні English, 简体中文, Русский және Қазақша тілдерінде берілген.

### Талаптар

- Windows 10 немесе Windows 11.
- Жүйеде қолжетімді .NET Framework 4.x компиляторы және WPF жинақтары; құрастыру скрипті Windows Framework Compiler құралын пайдаланады.
- `C:\gost\gost.exe` жолында бөлек орнатылған GOST 3.x. Бұл репозиторийде GOST жоқ.
- Пайдалануға рұқсатыңыз бар жұмыс істейтін HTTP немесе SOCKS5 проксиі.

### Бастапқы кодтан құрастыру

Репозиторий қалтасындағы PowerShell терезесін ашып, орындаңыз:

```powershell
Set-ExecutionPolicy -Scope Process Bypass
./Build.ps1
```

Нәтиже `dist\CapyProxy.exe` жолына жазылады. Git бұл файлды елемейді: репозиторий бастапқы код пен графиканы ұсынады, дайын орындалатын файлды емес.

### Алғашқы іске қосу

1. GOST-ты ресми жобадан бөлек орнатып, `gost.exe` файлын `C:\gost\gost.exe` жолына орналастырыңыз.
2. CapyProxy қолданбасын құрастырып, іске қосыңыз.
3. **Профиль** бөлімін ашып, өз серверіңіздің мекенжайын, портын, протоколын және қажет болса тіркелгі деректерін енгізіп, сақтаңыз.
4. **Подключить** батырмасын басыңыз. Үйлесімді қолданбалар HTTP прокси ретінде `http://127.0.0.1:8080` мекенжайын қолдана алады.
5. Прокси арқылы көрінетін сыртқы мекенжайды тексеру үшін **Проверить IP** батырмасын басыңыз.

Қолданба VPN қызметін қоспайды және өшірмейді, Windows жүйесіндегі ортақ прокси параметрін өзгертпейді. Автоматты іске қосу тек ағымдағы Windows пайдаланушысына орнатылады.

### Қауіпсіздік және құпиялылық

- Репозиторийде прокси мекенжайы, логин, құпиясөз, API кілті, токен немесе пайдаланушы профилі жоқ.
- Жаңа профиль бос болады. Әр пайдаланушы өз деректерін өзі енгізеді; баптаулар Windows DPAPI арқылы шифрланып, осы компьютерде ағымдағы Windows тіркелгісіне байланыстырылып сақталады.
- Жеке прокси параметрлерін, экспортталған профильдерді, журналдарды немесе құрастырылған файлдарды репозиторийге қоспаңыз.
- Жергілікті нүкте тек `127.0.0.1` loopback мекенжайында тыңдайды, барлық желі интерфейсінде емес.
- IP тексеруі бапталған жергілікті прокси арқылы `api.ipify.org` қызметіне сұрау жібереді. Бұл қызмет сұрауды және проксидің сыртқы IP мекенжайын көреді.
- Қазіргі нұсқа іске қосылған кезде сыртқы прокси мекенжайын GOST процесіне береді. Ортақ компьютердегі басқа жергілікті пайдаланушылар немесе әкімшілер процестің командалық жолын көруі мүмкін. Сенімсіз ортақ Windows тіркелгісінде тіркелгі деректерін енгізбеңіз.
- CapyProxy VPN емес, анонимдікке не бұғаттаудан айналып өтуге кепілдік бермейді. Прокси провайдерінің шарттарын және сайт ережелерін сақтаңыз.

### Жоба құрылымы

```text
Build.ps1       Windows құрастыру скрипті
src/Studio.cs   Қолданба логикасы
src/Studio.xaml WPF интерфейсі
assets/         Елестің суреті мен Windows белгішесі
```

### Үшінші тарап бағдарламасы және лицензия

GOST репозиторийге кірмейді. Оны [go-gost/gost ресми жобасынан](https://github.com/go-gost/gost) алып, қолданар алдында ағымдағы лицензиясы мен шығарылым жазбаларын тексеріңіз. CapyProxy — GOST әзірлеушілеріне қатысы жоқ тәуелсіз жоба.

Осы репозиторийдегі CapyProxy бастапқы коды MIT лицензиясымен таратылады; толық мәтіні [LICENSE](LICENSE) файлында. Үшінші тарап бағдарламалары мен графикасына бөлек шарттар қолданылуы мүмкін.

