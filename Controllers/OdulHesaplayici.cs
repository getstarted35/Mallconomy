using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using LiderTablosuAPI.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace LiderTablosuAPI.Controllers
{
    public class OdulHesaplayici
    {
        private readonly MongoDBConnection _dbConnection;

        private const string Birinci = "Birinci";
        private const string Ikinci = "Ikinci";
        private const string Ucuncu = "Ucuncu";
        private const string Top100 = "Top100 = $25";
        private const int ConsolationPrizeAmount = 12500;
        private string ConsolationPrize = "Top1000 = $" + ConsolationPrizeAmount / 1000;

        public OdulHesaplayici(MongoDBConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        // odulleri Hesaplama ve dağıtma yöntemi
        public void HesaplaOduller(string ay)
        {
            Regex ayBicim = new Regex(@"^(0[1-9]|1[012])[0-9]{4}$");
            if (!ayBicim.IsMatch(ay))
            {
                throw new ArgumentException("Olması gereken Ay formatı MMYYYY");
            }
            _dbConnection.Puanlar.Indexes.CreateOne(new CreateIndexModel<Puanlar>(Builders<Puanlar>.IndexKeys.Descending(x => x.point)));

            // Ay içinde onaylanmış kullanıcı verilerinin alınması
            var puanlar = _dbConnection.Puanlar.Find(Builders<Puanlar>.Filter.Eq(p => p.approved, true)).ToList();

            // Sıralamaların Puanlara göre hesaplanması
            var siralamalar = puanlar.OrderByDescending(puan => puan.point)
            .Select((puan, gosterge) => new { puan.user_id, puan.point, _odul = gosterge + 1 });

            // Mevcut olan ödüllerin silinme işlemi
            _dbConnection.KullaniciOdulleri.DeleteMany(Builders<KullaniciOdulleri>.Filter.Empty);
            int loading = 1;

            // Sıralamaya göre ödüllerin dağıtılması
            var kullaniciOdulleri = new List<KullaniciOdulleri>();
            foreach (var siralama in siralamalar)
            {
                var kullaniciOdulu = new KullaniciOdulleri
                {
                    user_id = ObjectId.Parse(siralama.user_id),
                };
                switch (siralama._odul)
                {
                    case 1:
                        kullaniciOdulu.odul = Birinci;
                        break;
                    case 2:
                        kullaniciOdulu.odul = Ikinci;
                        break;
                    case 3:
                        kullaniciOdulu.odul = Ucuncu;
                        break;
                    case int n when (n <= 100):
                        kullaniciOdulu.odul = Top100;
                        break;
                    case int n when (n <= 1000):
                        kullaniciOdulu.odul = ConsolationPrize;
                        break;
                    default:
                        kullaniciOdulu.odul = "Odul Yok";
                        break;
                }
                kullaniciOdulleri.Add(kullaniciOdulu);
                loading++;
            }

            if (kullaniciOdulleri.Count() > 0)
            {
                _dbConnection.KullaniciOdulleri.InsertMany(kullaniciOdulleri);
            }
            else 
            {
                Console.WriteLine("Kullanıcı ödülleri listesi boş");
            }

            // liderTablosu Oluşturma
            LiderTablosuOlustur(ay, siralamalar);
        }

        // Lider tablosu oluşturma yöntemi
        public IEnumerable<LiderTablosu> LiderTablosuOlustur(string ay, IEnumerable<dynamic> siralamalar)
        {
            // Ay içinde oluşturulan lider tablosu Koleksiyonu işlemi
            var liderTablosuKoleksiyonu = _dbConnection.veritabani.GetCollection<LiderTablosu>("LiderTablosu_" + ay);

            //Geçerli ay içinde lider tablosu zaten var mı yok mu kontrol sağlanması
            var liderTablosuBulunmak = liderTablosuKoleksiyonu.CountDocuments(Builders<LiderTablosu>.Filter.Empty) > 0;
            if (!liderTablosuBulunmak)
            {
                // Lider tablosu yoksa yeni bir lider tablosu koleksiyonu oluşturuyoruz
                var listesiLiderTablosu = siralamalar.Select(siralama => new LiderTablosu
                {
                    user_id = siralama.user_id,
                    puan = siralama.point,
                    _odul = siralama._odul
                });
                liderTablosuKoleksiyonu.InsertMany(listesiLiderTablosu);
                return listesiLiderTablosu;
            }
            else
            {
                Console.Write("Talep edilen ay'a ait Lider tablosu mevcut");
                return liderTablosuKoleksiyonu.Find(Builders<LiderTablosu>.Filter.Empty).ToList();
            }
        }

        public IEnumerable<KullaniciOdulleri> TumOdulleriListele()
        {
            // Tüm ödülleri al
            var TumOdulleri = _dbConnection.KullaniciOdulleri.Find(Builders<KullaniciOdulleri>.Filter.Empty).ToList();
            return TumOdulleri;
        }


        // Kullanıcı Ödülleri listeleme yöntemi
        public IEnumerable<KullaniciOdulleri> KullaniciOdulleriniListele(string userId)
        {
            // Belirlenen kullanıcılar için ödüllerin alınması
            var kullaniciOdulleri = _dbConnection.KullaniciOdulleri.Find(odul => odul.user_id == ObjectId.Parse(userId)).ToList();
            return kullaniciOdulleri;
        }

        // Lider tablosu listeleme yöntemi
        public IEnumerable<LiderTablosu> LiderTablosuListesi(string ay)
        {
            // Giriş doğrulama
            Regex ayBicim = new Regex(@"^(0[1-9]|1[012])[0-9]{4}$");
            if (!ayBicim.IsMatch(ay))
            {
                throw new ArgumentException("Ay'ın yazım formatı MMYYYY");
            }
            try
            {
                // Belirtilen ay içinde lider tablosu
                var liderTablosu = _dbConnection.veritabani.GetCollection<LiderTablosu>("LiderTablosu_" + ay)
                .Find(Builders<LiderTablosu>.Filter.Empty).ToList();
                if (!liderTablosu.Any())
                {
                    Console.WriteLine("Listelenecek Lider Tablosu Bulunamadı");
                }

                return liderTablosu;
            }
            catch (Exception ex)
            {
                // İstisna durumunda mesaj bildirimi
                Console.WriteLine(ex.Message);

                // Lider tablosu oluşturulurken bir hata olursa null döndür
                return null;
            }
        }
    }
}


