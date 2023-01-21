using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using LiderTablosuAPI.Models;
namespace LiderTablosuAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LiderTablosuController : ControllerBase
    {
        private readonly OdulHesaplayici _odulHesaplayici;
        private readonly MongoDBConnection _mongoDBConnection;

        public LiderTablosuController(OdulHesaplayici odulHesaplayici, MongoDBConnection mongoDBConnection)
        {
            _odulHesaplayici = odulHesaplayici;
            _mongoDBConnection = mongoDBConnection;
        }

        [HttpPost("olustur")]
        public IEnumerable<LiderTablosu> LiderTablosuOlustur(string ay)
        {
            try
            {
                var puanlar = _mongoDBConnection.PuanlariGetir();
                var siralamalar = puanlar.OrderByDescending(puan => puan.point)
                                   .Select((puan, gosterge) => new { puan.user_id, puan.point, _odul = gosterge + 1 });
                return _odulHesaplayici.LiderTablosuOlustur(ay, siralamalar);
            }

            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return null;
            }
        }

        [HttpGet("liste")]
        public IEnumerable<LiderTablosu> LiderTablosuListesi(string ay, string userId)
        {

            // Lider tablosunu getir
            var liderTablosu = _odulHesaplayici.LiderTablosuListesi(ay);
            // Kullanıcı kimliğine göre filtreleme işlemi
            liderTablosu = liderTablosu.Where(lb => lb.user_id == userId);

            return liderTablosu;
        }

        [HttpGet("odulleri")]
        public IEnumerable<KullaniciOdulleri> KullaniciOdulleriniListele(string userId)
        {
            try
            {
                // Kullanıcı ödüllerini getir
                var kullaniciOdulleri = _odulHesaplayici.KullaniciOdulleriniListele(userId);
                return kullaniciOdulleri;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return null;
            }
        }
    }
}