# DovizKuru

USD, EUR ve XAU kurunu takip edebileceğiniz bir uygulama. 

# Gereksinimler

- Windows 10.0.17763.0 veya üstü
- 
# Kurulum

* Release klasörü içerisindeki DovizKuru.exe dosyasını çalıştırabilirsiniz.
* [Visual Studio 2022](https://visualstudio.microsoft.com/tr/downloads/) - IDE ile açıp derleyebilirsiniz.

# Kullanım

* Uygulama açıldığında USD, EUR ve XAU kurunu çeker ve ekrana yazdırır. 
* Uygulama açıkken 15 saniyede bir günceller.
* Sağ üst köşedeki saat ikonuna tıklayarak Alarm penceresini açabilirsiniz.
* Alarm penceresinde kur, değer ve alarm tipi seçerek alarm kurabilirsiniz.
* Alarmlarınızı Alarm penceresinden görebilir, düzenleyebilir ve silebilirsiniz.
* Çalışan bir alarm otomatik olarak kapatılır, ancak silinmez. İsterseniz Alarm penceresinden silebilirsiniz veya tekrar çalıştırabilirsiniz.
* Tüm alarm sonuçları tek bir bildirim halinde gösterilir.

# Kütüphaneler

* [HtmlAgilityPack](https://html-agility-pack.net/) - Web sayfasından veri çekmek için kullanıldı.
* [Newtonsoft.Json](https://www.newtonsoft.com/json) - JSON işlemleri için kullanıldı.
* [Microsoft.Toolkit.Uwp.Notifications](https://www.nuget.org/packages/Microsoft.Toolkit.Uwp.Notifications/) - Bildirim için kullanıldı.
* [Mahapps.Metro](https://mahapps.com/) - Uygulama arayüzü için kullanıldı.
* [Mahapps.Metro.IconPacks](https://www.nuget.org/packages/MahApps.Metro.IconPacks) - Uygulama arayüzü için kullanıldı.
* [CommunityToolkit.MVVM](CommunityToolkit.Mvvm) - MVVM mimarisi için kullanıldı.
* [Microsoft.Extensions.DependencyInjection](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection/8.0.0-preview.4.23259.5) - Dependency Injection için kullanıldı.
* [Microsoft.Web.WebView2](https://developer.microsoft.com/en-us/microsoft-edge/webview2/) - WebView için kullanıldı.