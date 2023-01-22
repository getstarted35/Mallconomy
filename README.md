# Mallconomy
Mallconomy: Points - C#, MongoDB, API

-Giriş-

Bu proje, belirli bir ay için liderlik tablosunu ve ödülleri hesaplamak için oluşturulmuş bir API'dir.
API kullanılarak oluşturulmuştur. Projedeki tüm yapıların türkçe metin kullanarak yapılması tamamen benim
ingilizce bilgimin yetersizliği ve ileriye dönük projeyi incelerken daha iyi anlamamı sağlamak için gerçeleştirilmiştir.
Projenin deşifre edilmesi, inceleyen kişilerin daha iyi anlaması amaçlanmıştır.

--İşlemler--

1-Projemiz üzerinden MongoDB kullanacağimiz için gerekli paketleri NuGet aracılığı ile
yükleyerek işleme başlandı (MongoDB.Bson and MongoDB.Driver)

2-MongoDB Database'ine bağlanmak için appsettings.json dosyasına, açtığımız
Database bilgilerimi ekleyerek, Task üzerinde verilen json dosyasını aktarıldı

3-Startup class'ı için MongoDBConnection işlemlerinin congifürasyon işlemleri gerçekleştirildi

4-Program.cs kısmında ÖdülHesaplayici ve gereken tüm  kısımlar yerleştirildi

5-Json dosyasında verilen listelenmesi istenen class'ların Propertyleri dahil edildi.

---Classların Görevleri---

MongoDBConnection: Bu sınıf, MongoDB'ye bağlanmaktan sorumludur.
veritabanı ve koleksiyonlardan veri eklemek ve almak için kullanılmıştır

OdulHesaplayici: Bu sınıf, ödülleri ve sıralamaları hesaplamaktan sorumludur.
Kullanıcı noktası verilerine dayalı olarak. Lider Tablosu oluşturmak için yöntemler içerir,

LiderTablosuController: Bu sınıf, HTTP isteklerini işlemekten,
Route ve Tabloyu, listelemek ve almaktan sorumludur.

----Sonuç----

NOT: API'yi kullanmak için, kullanıcının önce MongoDBConnection sınıfını başlatması, MongoDB bağlantı dizesini kendi veritabanı ve parolasını girerek 
sağlanması gereklidir. Tablo Oluşturmak, Aynı Ay içerisinde tek tabloya ulaşılması. Sadece True durumundaki kullanıcıların yarışmaya dahil olması gibi
pek çok özellik açmış olduğum class'larda mevcuttur. Muhtemelen Interface kullanmadığım için projeyi çalıştırdığımızda Swagger açılmamaktadır. Yapmış
olduğumuz projenin sonucunu Console üzerinden ve Bağlı olduğumuz veritabını üzerinden listelemekteyiz. 

Json Link: https://cdn.mallconomy.com/testcase/points.json
