# 🎬 Auto-Video-Downloader-FFmpeg-UI

[![Download](https://img.shields.io/badge/Download-Latest_Version-blue?style=for-the-badge&logo=windows)](https://github.com/burakozdelen/Auto-Video-Downloader-FFmpeg-UI/releases/download/v1.0/Video.Downloader.v1.0.zip)

**Select Language / Dil Seçiniz:**
[🇺🇸 English](#-english) | [🇹🇷 Türkçe](#-türkçe)

---

## 🇺🇸 English

**Video Downloader** is an advanced Windows desktop application that analyzes your browser's network traffic to automatically detect, capture, and download fragmented video streams (HLS/m3u8) and subtitles.

This tool simplifies complex video downloading processes and utilizes the power of **FFmpeg** in the background to merge audio, video, and subtitles into a single file.

### 📖 User Guide & Tips

To get the best performance from the application, please read the following steps carefully.

#### 1. Initial Setup & Certificates
* **Certificate Approval:** When you launch 'Start Search' mode for the first time, the app will request to install a "Root Certificate" to read HTTPS traffic. You must **Approve/Yes** this request; otherwise, links cannot be captured.
* **Automatic Tool Installation:** During the download phase, if `FFmpeg` and `N_m3u8DL-RE` tools are missing from your computer, the app will download them automatically.
    * ⚠️ **IMPORTANT:** After these tools are automatically downloaded and the "Success" message appears, **you must click the Download button AGAIN** to start the actual process.

#### 2. Capturing the Right Link (Crucial!)
* **Close Other Tabs:** When searching for a video, keep **only the video tab open** in your browser. Close other tabs. Otherwise, background requests from other sites will clutter the capture list.
* **Find the Master URL:** You generally need to select the "Master URL" from the list.
    * Links containing the word "master" are usually the correct ones.
    * If other links end with numbers like 1080, 720, but one does NOT, that one is likely the "Master" link.
    * The extension does not always have to be `.m3u8`; it could be `.txt` or contain complex characters.

#### 3. Download Options
* You can start only **ONE** video download process at a time.
* However, you can select all resolutions, audio tracks, and subtitles *within* that single video to merge them.
* **Subtitles:** If you see multiple files for the same language (e.g., 3 files named 'eng.vtt'), **SELECT ALL OF THEM**. Some might be empty or broken; you can verify and delete them after downloading.

### ⚠️ Critical Warnings & Troubleshooting

#### 🛑 If the App Crashes (Proxy Error)
This app runs a "Proxy Server" in the background. If the app closes unexpectedly or is force-closed via Task Manager, Windows Proxy settings may remain active, **cutting off your internet access.**

**Solution:**
1.  Go to **Windows Settings > Network & Internet > Proxy**.
2.  Turn the "Use a proxy server" option to **OFF**.

#### 📺 Playback Recommendation (VLC Player)
We strongly recommend using **[VLC Media Player](https://www.videolan.org/)** to watch the downloaded videos.
> Standard players (like Windows Media Player) often fail to detect subtitles or switch between audio tracks (dubbing) correctly.

#### 🚫 Unsupported Platforms (DRM)
* **DRM Protection:** Platforms like YouTube, Netflix, Disney+, Amazon Prime use encrypted (DRM) streams. Downloading from these platforms is **NOT SUPPORTED**.
* **Live/Dynamic Streams:** Some streams use second-by-second encryption. If a download finishes instantly but you only get 3-5 seconds of footage, this is not a bug; it is due to the stream's protection method.

### 📢 Credits & Libraries

This project uses the following amazing open-source projects to perform complex media operations. All credits and copyrights belong to their respective developers.

* **FFmpeg:** The industry standard for video processing, converting, and merging.
    🔗 [https://ffmpeg.org/](https://ffmpeg.org/) (LGPL/GPL Licensed)

* **N_m3u8DL-RE:** A powerful CLI tool for downloading modern and complex HLS/DASH streams.
    🔗 [https://github.com/nilaoda/N_m3u8DL-RE](https://github.com/nilaoda/N_m3u8DL-RE) (MIT Licensed)

* **Titanium Web Proxy:** A powerful HTTP(S) proxy server for C#.
    🔗 [https://github.com/justcoding121/Titanium-Web-Proxy](https://github.com/justcoding121/Titanium-Web-Proxy)

### ⚖️ Disclaimer

This project is intended solely for educational and personal use; the developer assumes no liability for any misuse or copyright infringement.

---

## 🇹🇷 Türkçe

**Video Downloader**, tarayıcınızdaki video trafiğini analiz ederek yakalayan, parçalı akışları (HLS/m3u8) ve altyazıları otomatik olarak tespit edip indirmenizi sağlayan gelişmiş bir Windows masaüstü uygulamasıdır.

Bu araç, karmaşık video indirme işlemlerini sizin için basitleştirir ve arka planda FFmpeg gücünü kullanır.

### 📖 Kullanım Kılavuzu ve İpuçları

Uygulamadan en iyi verimi almak için lütfen aşağıdaki adımları dikkatlice okuyun.

#### 1. İlk Kurulum ve Sertifika
* **Sertifika Onayı:** 'Start Search' modunu ilk kez başlattığınızda, uygulama HTTPS trafiğini okuyabilmek için bir "Kök Sertifika" (Root Certificate) yüklemek isteyecektir. Buna **Evet/Onay** vermelisiniz, aksi takdirde linkler yakalanamaz.
* **Otomatik Araç Kurulumu:** İndirme aşamasında, eğer bilgisayarınızda `FFmpeg` ve `N_m3u8DL-RE` araçları eksikse, uygulama bunları otomatik olarak indirir.
    * ⚠️ **ÖNEMLİ:** Bu araçlar otomatik indirilip "Success" mesajı göründükten sonra, işlemi başlatmak için **Download butonuna TEKRAR basmanız gerekmektedir.**

#### 2. Doğru Linki Yakalamak (Çok Önemli!)
* **Sekmeleri Kapatın:** Video ararken tarayıcınızda **sadece video sekmesi açık olsun**. Diğer sekmeleri kapatın. Aksi takdirde arka plandaki gereksiz istekler listeyi doldurur.
* **Master URL'yi Bulun:** Listede genellikle "Master URL"yi seçmeniz gerekir.
    * İçinde "master" kelimesi geçen linkler genelde doğru linktir.
    * Diğer linkler 1080, 720 gibi sayılarla bitiyorsa ama bir tanesi bitmiyorsa, o "Master" linktir.
    * Uzantısı her zaman `.m3u8` olmak zorunda değildir, `.txt` veya karışık karakterler olabilir.

#### 3. İndirme Seçenekleri
* Aynı anda sadece **TEK BİR** video indirme işlemi başlatabilirsiniz.
* Ancak, o tek videonun içindeki tüm çözünürlükleri, ses dosyalarını ve altyazıları aynı anda seçebilirsiniz.
* **Altyazılar:** Aynı dilde birden fazla dosya görürseniz (örn: 3 tane 'eng.vtt'), **HEPSİNİ SEÇİN**. Bazıları boş veya bozuk olabilir, indirdikten sonra kontrol edip silebilirsiniz.

### ⚠️ Kritik Uyarılar ve Sorun Giderme

#### 🛑 Uygulama Çökerse (Proxy Hatası)
Bu uygulama arka planda bir "Proxy Sunucusu" çalıştırır. Eğer uygulama beklenmedik bir şekilde kapanırsa veya Görev Yöneticisi ile zorla kapatılırsa, Windows Proxy ayarları açık kalabilir ve **internetiniz kesilebilir.**

**Çözüm:**
1.  **Windows Ayarları > Ağ ve İnternet > Proxy** yolunu izleyin.
2.  "Bir proxy sunucusu kullan" (Use a proxy server) seçeneğini **KAPALI** (OFF) konuma getirin.

#### 📺 Oynatma Önerisi (VLC Player)
İndirilen videoları izlemek için kesinlikle **[VLC Media Player](https://www.videolan.org/)** kullanmanızı öneririz.
> Standart Windows Media Player, altyazıları algılamada veya çift ses dosyaları (dublaj) arasında geçiş yapmada yetersiz kalabilir.

#### 🚫 Desteklenmeyen Platformlar (DRM)
* **DRM Koruması:** YouTube, Netflix, Disney+, Amazon Prime gibi platformlar şifreli (DRM) yayınlar kullanır. Bu araç ile bu platformlardan indirme **YAPILAMAZ**.
* **Canlı/Dinamik Akışlar:** Bazı yayınlar saniyelik şifreleme kullanır. İndirme anında bitiyor ama sadece 3-5 saniyelik görüntü geliyorsa, bu bir hata değil, yayının koruma yöntemidir.

### 📢 Teşekkür ve Kütüphaneler (Credits)

Bu proje, karmaşık medya işlemlerini gerçekleştirmek için aşağıdaki harika açık kaynaklı projeleri kullanmaktadır. Tüm krediler ve telif hakları ilgili geliştiricilere aittir.

* **FFmpeg:** Video işleme, dönüştürme ve birleştirme işlemleri için endüstri standardı.
    🔗 [https://ffmpeg.org/](https://ffmpeg.org/) (LGPL/GPL Licensed)

* **N_m3u8DL-RE:** Modern ve karmaşık HLS/DASH akışlarını indirmek için kullanılan güçlü CLI aracı.
    🔗 [https://github.com/nilaoda/N_m3u8DL-RE](https://github.com/nilaoda/N_m3u8DL-RE) (MIT Licensed)

* **Titanium Web Proxy:** C# için güçlü HTTP(S) proxy sunucusu.
    🔗 [https://github.com/justcoding121/Titanium-Web-Proxy](https://github.com/justcoding121/Titanium-Web-Proxy)

### ⚖️ Yasal Uyarı

Bu proje yalnızca eğitim ve kişisel kullanım amacıyla geliştirilmiştir; geliştirici, yazılımın kötüye kullanımından veya telif hakkı ihlallerinden sorumlu tutulamaz.
