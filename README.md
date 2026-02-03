# 📱 MAUI Firebase Auth & Push Notification Demo

Bu proje, **.NET MAUI** platformu üzerinde modern bir mobil uygulama deneyimi sunmak amacıyla geliştirilmiştir. Uygulama, Firebase ekosistemi ile tam entegre çalışarak **Kimlik Doğrulama (Auth)**, **Firestore Veri Yönetimi** ve **Bulut Bildirimleri (FCM V1)** süreçlerini uçtan uca yönetmektedir.

---

## 📋 Proje Gereksinim Uyumluluk Tablosu

Aşağıdaki tablo, case dökümanında belirtilen zorunlu gereksinimlerin karşılanma durumunu göstermektedir:

| Madde | Gereksinim | Durum | Teknik Detay |
| :--- | :--- | :---: | :--- |
| 1 | **Landing Tasarımı** | ✅ | CarouselView ve Responsive modern UI bileşenleri kullanıldı. |
| 2 | **Firebase Auth** | ✅ | Register, Login, Logout ve "Beni Hatırla" özellikleri aktif. |
| 3 | **İzinler ve Token** | ✅ | Android 13+ izin akışı ve FCM Token gösterimi sağlandı. |
| 4 | **Bildirim Gönderimi** | ✅ | FCM V1 & OAuth2 ile Topic/Token bazlı gönderim paneli kuruldu. |
| 5 | **Bildirim Alma** | ✅ | Ön plan (Foreground) ve Arka plan (Background) desteği mevcut. |
| 6 | **Dökümantasyon** | ✅ | Tüm kurulum ve mimari detaylar bu dosyada belgelenmiştir. |

---

## 📸 Ekran Görüntüleri

### 1. Kimlik Doğrulama ve Kayıt Akışı
| Hoşgeldiniz (Landing) | Giriş Yap (Login) | Kayıt Ol (Register) |
| :---: | :---: | :---: |
| <img src="screenshots/landing.jpeg" width="200" /> | <img src="screenshots/login.jpeg" width="200" /> | <img src="screenshots/register.jpeg" width="200" /> |

### 2. Dashboard (3 Sekmeli Yönetim Paneli)
| Sekme 1: Bildirim Gönder | Sekme 2: Gelen Kutusu | Sekme 3: Cihaz Token |
| :---: | :---: | :---: |
| <img src="screenshots/dashboardtab1.jpeg" width="200" /> | <img src="screenshots/dashboardtab2.jpeg" width="200" /> | <img src="screenshots/dashboardtab3.jpeg" width="200" /> |

---

## 💻 Kritik Teknik Çözümler

### 1. Güvenli Bildirim Gönderimi (FCM V1 & OAuth2)
Uygulama, eski Legacy API yerine Google'ın güncel **FCM V1** protokolünü kullanır. Uygulama içinden bildirim tetiklemek için `Google.Apis.Auth` kütüphanesi ile Service Account üzerinden geçici **Access Token** üretilir:

```csharp
// OAuth2 Access Token Üretimi (FCM V1 Yetkilendirme)
var credential = GoogleCredential.FromJson(jsonContent)
                 .CreateScoped("https://www.googleapis.com/auth/firebase.messaging");
var accessToken = await credential.UnderlyingCredential.GetAccessTokenForRequestAsync();

// FCM V1 Payload Gönderimi
using (var client = new HttpClient())
{
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
    var response = await client.PostAsync(fcmV1Url, new StringContent(jsonPayload, Encoding.UTF8, "application/json"));
}

```
### 2. Dinamik Bildirim Listeleme (Inbox)
Gelen bildirimler, uygulama açıkken (Foreground) NotificationReceived eventi ile yakalanır ve arayüzdeki ObservableCollection listesine anlık yansıtılır:
```csharp
CrossFirebaseCloudMessaging.Current.NotificationReceived += (s, e) => {
    MainThread.BeginInvokeOnMainThread(() => {
        Notifications.Insert(0, new ReceivedNotification {
            Title = e.Notification.Title,
            Body = e.Notification.Body,
            ReceivedTime = DateTime.Now.ToString("HH:mm")
        });
    });
};

```

### 🛠️ Kurulum ve Yapılandırma (Case Maddeleri)
### 1. Firebase Kurulumu
Android: google-services.json dosyası Platforms/Android/ altına eklenmiş ve Build Action GoogleServicesJson olarak ayarlanmıştır.
Yetkilendirme: service-account.json dosyası Resources/Raw/ altına eklenerek FCM V1 gönderimi için gerekli izinler tanımlanmıştır.
### 2. Authentication Akışı
Kayıt & Giriş: Kullanıcı kayıt olduğunda Firebase Auth üzerinde hesap oluşturulur ve kullanıcı profil verileri Firestore'da yedeklenir.
Oturum Yönetimi: Preferences kullanılarak "Beni Hatırla" özelliği entegre edilmiştir. Uygulama açılışında geçerli bir oturum varsa otomatik giriş (Auto-login) yapılır.
### 3. Bildirim Gönderme Mantığı (Topic / Token)
Dashboard üzerindeki yönetim panelinden (Tab 1) iki farklı hedefleme yapılabilir:
Topic (Konu): "news" gibi belirli bir konuya abone olan tüm cihazlara toplu mesaj gönderimi.
Token (Cihaz): Sadece belirli bir cihazın eşsiz FCM Token'ına özel, manuel hedefli mesaj gönderimi.
### 4. Kullanılan Paketler
Plugin.Firebase (Auth, Firestore, Messaging)
Google.Apis.Auth (OAuth2/FCM V1 Yetkilendirme)
CommunityToolkit.Maui (Modern UI/UX Bileşenleri)
Xamarin.AndroidX.Lifecycle (Sürüm çakışmalarını gidermek için 2.8.3.1 sürümüyle stabilize edildi)

### ⚠️ Bilinen Eksikler ve İyileştirmeler
### [!IMPORTANT]
Güvenlik Notu: Service Account anahtarı test kolaylığı için uygulama içindedir; gerçek prodüksiyon senaryolarında bu işlem bir Backend API üzerinden yönetilmelidir.
Splash Screen: Android 12+ üzerindeki sistem önbelleği nedeniyle, bazı cihazlarda görsel varsayılan sistem renginde kalabilmektedir.
Geliştirme Önerisi: Bildirimlerin SQLite ile cihazda kalıcı olarak saklanması ve bildirim geçmişi silme özelliği eklenebilir.
### Geliştirici: Sultannur KAYA
### Teslim Tarihi: 03.02.2026
### Platform: .NET MAUI v8.0