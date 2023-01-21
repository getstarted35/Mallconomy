using System.Collections.Generic;
using LiderTablosuAPI.Models;
using MongoDB.Driver;

namespace LiderTablosuAPI.Controllers
{
    public class MongoDBConnection
    {
        // The MongoClient sınıfı MongoDB cluster ile bağlantı kurmamızı sağlayan bölüm
        private readonly MongoClient _kullanici;
        // IMongoDatabase interface Veritabanı ile etkileşim sağlar
        private readonly IMongoDatabase _veritabani;

        // Database ismi veritabanı bağlantı kısmının gerçekleştiği blok
        public MongoDBConnection(string connectionString, string databaseName)
        {
            _kullanici = new MongoClient(connectionString);
            _veritabani = _kullanici.GetDatabase(databaseName);
        }

        // Veritabanında ki Kullanıcı puanlarını alma yöntemi
        public IEnumerable<Puanlar> PuanlariGetir()
        {
            var puanlarKoleksiyonu = _veritabani.GetCollection<Puanlar>("puanlar");
            var puanlar = puanlarKoleksiyonu.Find(puan => puan.approved == true).ToEnumerable();
            return puanlar;
        }

        public IMongoDatabase veritabani
        {
            get { return _veritabani; }
        }

        // properties for accessing the puanlar, liderTablosu and kullaniciOdulleri collections in the veritabani
        public IMongoCollection<Puanlar> Puanlar => _veritabani.GetCollection<Puanlar>("puanlar");

        public IMongoCollection<LiderTablosu> LiderTablosu => _veritabani.GetCollection<LiderTablosu>("liderTablosu");
        public IMongoCollection<KullaniciOdulleri> KullaniciOdulleri => _veritabani.GetCollection<KullaniciOdulleri>("kullaniciOdulleri");
    }
}