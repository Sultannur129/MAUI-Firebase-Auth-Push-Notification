[English](#english) | [Türkçe](#türkçe)


## Türkçe

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
 #### Android: 
google-services.json dosyası Platforms/Android/ altına eklenmiş ve Build Action GoogleServicesJson olarak ayarlanmıştır.
#### Yetkilendirme: 
service-account.json dosyası Resources/Raw/ altına eklenerek FCM V1 gönderimi için gerekli izinler tanımlanmıştır.
### 2. Authentication Akışı
#### Kayıt & Giriş: 
Kullanıcı kayıt olduğunda Firebase Auth üzerinde hesap oluşturulur ve kullanıcı profil verileri Firestore'da yedeklenir.
#### Oturum Yönetimi: 
Preferences kullanılarak "Beni Hatırla" özelliği entegre edilmiştir. Uygulama açılışında geçerli bir oturum varsa otomatik giriş (Auto-login) yapılır.
### 3. Bildirim Gönderme Mantığı (Topic / Token)
Dashboard üzerindeki yönetim panelinden (Tab 1) iki farklı hedefleme yapılabilir:
#### Topic (Konu): 
"news" gibi belirli bir konuya abone olan tüm cihazlara toplu mesaj gönderimi.
#### Token (Cihaz): 
Sadece belirli bir cihazın eşsiz FCM Token'ına özel, manuel hedefli mesaj gönderimi.
### 4. Kullanılan Paketler
Plugin.Firebase (Auth, Firestore, Messaging)
Google.Apis.Auth (OAuth2/FCM V1 Yetkilendirme)
CommunityToolkit.Maui (Modern UI/UX Bileşenleri)
Xamarin.AndroidX.Lifecycle (Sürüm çakışmalarını gidermek için 2.8.3.1 sürümüyle stabilize edildi)

### ⚠️ Bilinen Eksikler ve İyileştirmeler
#### Güvenlik Notu: 
Service Account anahtarı test kolaylığı için uygulama içindedir; gerçek prodüksiyon senaryolarında bu işlem bir Backend API üzerinden yönetilmelidir. Apk keystore ile imzalanmıştır. SHA1 ve SHA256 fingerprint keyleri Firebase Console'da güvenlik amacıyla entegre edilmiştir.
#### Geliştirme Önerisi: 
Bildirimlerin SQLite ile cihazda kalıcı olarak saklanması ve bildirim geçmişi silme özelliği eklenebilir.
#### Geliştirici: Sultannur KAYA
#### Teslim Tarihi: 03.02.2026
#### Platform: .NET MAUI v8.0



## English

# 📱 MAUI Firebase Auth & Push Notification Demo

This project is developed to provide a modern mobile application experience using the **.NET MAUI** platform. It features a full integration with the Firebase ecosystem, managing **Identity Authentication (Auth)**, **Firestore Data Management**, and **Cloud Messaging (FCM V1)** processes end-to-end.

---

## 📋 Project Requirement Compliance

The following table demonstrates how the mandatory requirements specified in the case document are met:

| Item | Requirement | Status | Technical Detail |
| :--- | :--- | :---: | :--- |
| 1 | **Landing Design** | ✅ | Modern UI with CarouselView and Responsive components. |
| 2 | **Firebase Auth** | ✅ | Register, Login, Logout, and "Remember Me" features. |
| 3 | **Permissions & Token** | ✅ | Android 13+ permission flow and FCM Token display. |
| 4 | **Sending Notifications** | ✅ | Topic/Token based sender panel using FCM V1 & OAuth2. |
| 5 | **Receiving Notifications** | ✅ | Supported for both Foreground and Background states. |
| 6 | **Documentation** | ✅ | Full architectural and setup details documented here. |

---

## 📸 Screenshots

### 1. Authentication and Registration Flow
| Welcome (Landing) | Sign In (Login) | Sign Up (Register) |
| :---: | :---: | :---: |
| <img src="screenshots/landing.jpeg" width="200" /> | <img src="screenshots/login.jpeg" width="200" /> | <img src="screenshots/register.jpeg" width="200" /> |

### 2. Dashboard (3-Tab Management Panel)
| Tab 1: Send Notification | Tab 2: Notification Inbox | Tab 3: Device Token |
| :---: | :---: | :---: |
| <img src="screenshots/dashboardtab1.jpeg" width="200" /> | <img src="screenshots/dashboardtab2.jpeg" width="200" /> | <img src="screenshots/dashboardtab3.jpeg" width="200" /> |

---

## 💻 Critical Technical Solutions

### 1. Secure Notification Delivery (FCM V1 & OAuth2)
The application utilizes Google's current **FCM V1** protocol instead of the Legacy API. To trigger notifications from within the app, a temporary **Access Token** is generated via a Service Account using the `Google.Apis.Auth` library:

```csharp
// OAuth2 Access Token Generation (FCM V1 Authorization)
var credential = GoogleCredential.FromJson(jsonContent)
                 .CreateScoped("https://www.googleapis.com/auth/firebase.messaging");
var accessToken = await credential.UnderlyingCredential.GetAccessTokenForRequestAsync();

// FCM V1 Payload Delivery
using (var client = new HttpClient())
{
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
    var response = await client.PostAsync(fcmV1Url, new StringContent(jsonPayload, Encoding.UTF8, "application/json"));
}
```
### 2. Dynamic Notification Listing (Inbox)
Incoming notifications are captured via the NotificationReceived event while the app is in the foreground and immediately reflected in the UI's ObservableCollection:
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

## 🛠️ Setup and Configuration
### 1. Firebase Configuration
#### Android: 
The google-services.json file is placed under Platforms/Android/ with the Build Action set to GoogleServicesJson.
#### Authorization: 
The service-account.json file is located in Resources/Raw/ to provide the necessary credentials for FCM V1 delivery.
### 2. Authentication Flow
#### Registration & Login: 
User accounts are created via Firebase Auth, and profile metadata is synchronized with Firestore.
#### Session Management: 
"Remember Me" functionality is integrated using Preferences. The application performs an Auto-login if a valid session exists upon startup.
### 3. Notification Logic (Topic / Token)
The Management Panel (Tab 1) allows targeting through two methods:
#### Topic Based: 
Broadly targeting all devices subscribed to a specific topic (e.g., "news").
#### Token Based: 
Precisely targeting a single device using its unique FCM Token.
### 4. Utilized Packages
Plugin.Firebase (Auth, Firestore, Messaging)
Google.Apis.Auth (OAuth2/FCM V1 Authorization)
CommunityToolkit.Maui (Modern UI/UX Components)
Xamarin.AndroidX.Lifecycle (Stabilized at version 2.8.3.1 to resolve version conflicts)

## ⚠️ Known Issues and Future Improvements

### Security Note: 
The Service Account key is embedded within the app for testing convenience. In a production environment, notification triggers should be managed via a secure Backend API. The APK is signed with a Keystore, and SHA1/SHA256 fingerprints have been integrated into the Firebase Console for security.

#### Future Roadmap: 
Integration of SQLite for persistent local notification storage and a feature to clear notification history could be added.

### Developer: Sultannur KAYA
### Submission Date: 03.02.2026
### Platform: .NET MAUI v8.0