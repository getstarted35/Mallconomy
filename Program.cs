using System;
using LiderTablosuAPI.Controllers;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace LiderTablosuAPI
{
    public class Program
    {
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        WebHost.CreateDefaultBuilder(args).UseStartup<Startup>();
        public static void Main(string[] args)
        {
            AppContext.SetSwitch("System.Diagnostics.Trace.UseGlobalLock", false);
            var host = CreateWebHostBuilder(args)
            .CaptureStartupErrors(true) // Başlatma hatalarını göstermek için
            .Build();
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    //Gerekli servislerin yazılma işlemleri
                    var odulHesaplayici = services.GetRequiredService<OdulHesaplayici>();
                    var mongoDBConnection = services.GetRequiredService<MongoDBConnection>();
                    var _liderTablosuController = services.GetRequiredService<LiderTablosuController>();

                    //Ay formatı MMYYYY
                    DateTime gecerliTarih = DateTime.Now;
                    var suAnkiAy = gecerliTarih.ToString("MMyyyy");

                    Console.WriteLine("\n\nPlease Wait...");

                    //Mevcut ay içinde lider tablosu oluşturma ve ödülleri dağıtma
                    odulHesaplayici.HesaplaOduller(suAnkiAy);
                    //Lider tablosunu göster yazdır ve toplam puanligilerini göster
                    var liderTablosu = odulHesaplayici.LiderTablosuListesi(suAnkiAy);
                    Console.WriteLine("\n\nLiderTablosu:");
                    foreach (var oge in liderTablosu)
                    {
                        Console.WriteLine(oge.user_id + " - " + oge._odul + " - " + oge.puan);
                    }

                    //user_id göre lider tablosu gösterme işlemi
                    var kullanicininLiderTablosu = _liderTablosuController.LiderTablosuListesi(suAnkiAy, "628bd088f833c9b49877a580");
                    Console.WriteLine("\n\nLiderTablosu for user 628bd088f833c9b49877a580 :");
                    foreach (var oge in kullanicininLiderTablosu)
                    {
                        Console.WriteLine(oge.user_id + " - " + oge._odul + " - " + oge.puan);
                    }

                    //Tüm Ödüller
                    var odulleri = odulHesaplayici.TumOdulleriListele();
                    Console.WriteLine("\n\nOdulleri:");
                    foreach (var oge in odulleri)
                    {
                        Console.WriteLine(oge.user_id + " - " + oge.odul);
                    }

                    //Ödülleri kullanıcı kimliğine göre filtreleme işlemi
                    var kullaniciOdulleri = odulHesaplayici.KullaniciOdulleriniListele("628b514c1ba3abddabf15fe3");
                    Console.WriteLine("\n\nOdulleri for user 628b514c1ba3abddabf15fe3:");
                    int count = 1;
                    foreach (var oge in kullaniciOdulleri)
                    {
                        Console.WriteLine(count + ". " + oge.odul + "\n\n");
                        count++;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            host.Run();
        }
    }
}