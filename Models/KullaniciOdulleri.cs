using MongoDB.Bson;
namespace LiderTablosuAPI.Models
{
    public class KullaniciOdulleri
    {
        public ObjectId _id { get; set; }
        public ObjectId user_id { get; set; }
        public string odul { get; set; }
    }
}