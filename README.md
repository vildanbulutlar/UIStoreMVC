# UIStoreMVC 🛍️

ASP.NET Core MVC kullanılarak geliştirilmiş,
**Onion Architecture** ve **Domain-Driven Design (DDD)** prensiplerine uygun
bir e-ticaret uygulamasıdır.

Bu proje; yalnızca çalışan bir uygulama üretmeyi değil,
**kurumsal projelere yakın, sürdürülebilir ve ölçeklenebilir**
bir mimari kurmayı hedeflemektedir.

---

## 🚀 Proje Özeti

- MVC tabanlı web uygulaması
- Area yapısı ile modüler UI organizasyonu
- Admin & kullanıcı senaryoları
- Üyelik, dashboard ve yönetim akışları
- Veritabanı tabanlı alışveriş sepeti
- Gerçek e-ticaret senaryosu kurgusu

---

## 🧅 Onion Architecture & DDD Yaklaşımı

Projede mimari tercih bilinçli olarak
**Onion Architecture** ve **Domain-Driven Design (DDD)** prensiplerine göre yapılmıştır.

Amaç; iş kurallarını merkeze alan,
UI ve altyapıdan bağımsız,
test edilebilir ve sürdürülebilir bir yapı kurmaktır.

### 🧠 Domain Merkezli Tasarım

Uygulamanın kalbi **Domain katmanıdır**.

- Entity’ler yalnızca veri taşımaz, **davranış içerir**
- İş kuralları UI veya Infrastructure katmanına dağılmaz
- Domain katmanı başka hiçbir katmana bağımlı değildir

Ürün, kategori, sipariş ve sepet gibi kavramlar
iş kurallarıyla birlikte Domain içerisinde ele alınmıştır.

---

## 🧩 Katmanlı Mimari Yapı

### Domain
- Entity’ler
- İş kuralları
- Domain davranışları
- Repository arayüzleri

### Application
- Use case’ler
- Servisler
- DTO’lar
- Validation ve iş akışları

### Infrastructure
- Entity Framework Core
- Repository implementasyonları
- Veritabanı erişimi
- Persistence ve teknik detaylar

### Presentation (MVC)
- Controller’lar
- View’lar
- ViewComponent’ler
- Area bazlı UI organizasyonu

Bağımlılıklar **içe doğru** ilerler.
Dış katmanlar iç katmanlara bağımlıdır; tersi mümkün değildir.

---

## 🧩 MVC Areas ile Modüler Yapı

Projede ASP.NET Core MVC **Areas** yapısı aktif olarak kullanılmıştır.

Bu sayede uygulama;
- Sorumluluklarına göre ayrılmış
- Controller karmaşasından arındırılmış
- Büyümeye açık bir yapı kazanmıştır

### Kullanılan Alanlar (Areas)

- **Admin Area**
  - Dashboard ekranları
  - Ürün, kategori ve sipariş yönetimi
  - Üyelik başvurularının yönetimi
  - Yetkiye göre erişim kontrolü

- **Public / User Area**
  - Ürün listeleme ve detay sayfaları
  - Alışveriş sepeti
  - Sipariş oluşturma akışları
  - Kullanıcıya özel işlemler

---

## 👤 Üyelik & Rol Tabanlı Senaryolar

Projede üyelikler yalnızca
login / register seviyesinde ele alınmamıştır.

- Kullanıcıya bağlı shopping cart
- Kullanıcıya özel sipariş geçmişi
- Rol bazlı erişim senaryoları
- Admin tarafından yönetilen başvuru ve onay süreçleri

Üyelik yapısı;
Domain kuralları korunarak,
Application servisleri üzerinden
UI katmanına yansıtılmıştır.

---

## 📊 Admin Dashboard Yaklaşımı

Admin paneli sadece CRUD ekranlarından oluşmaz.

Bu projede dashboard mantığı:
- Yöneticiye özel Area yapısı
- Gerçek veri üzerinden özet bilgiler
- Sipariş ve içerik yönetimi
- Genişletilebilir ve okunabilir tasarım

prensipleriyle ele alınmıştır.

---

## 🔧 Kullanılan Teknolojiler

- ASP.NET Core MVC
- Entity Framework Core
- Onion Architecture
- Domain-Driven Design (DDD)
- Repository Pattern
- Unit of Work
- AutoMapper
- FluentValidation
- SQL Server
- LibMan (jQuery, Validation)
- Bootstrap

---

## 🎯 Projenin Amacı

Bu proje;
- Katmanlı mimariyi gerçek bir senaryoda uygulamak
- Domain odaklı düşünme pratiği kazanmak
- Kurumsal projelere yakın bir yapı kurmak
- GitHub portföyünde referans bir çalışma sunmak

amacıyla geliştirilmiştir.

---

## 📌 Not

Bu proje eğitim ve portföy amaçlıdır.
Geliştirilmeye ve genişletilmeye açıktır.
