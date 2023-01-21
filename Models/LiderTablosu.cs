using MongoDB.Bson;
namespace LiderTablosuAPI.Models
{
    public class LiderTablosu
    {
        public ObjectId _id { get; set; }
        public string user_id { get; set; }
        public int puan { get; set; }
        public int _odul { get; set; }
    }
}