using System.ComponentModel.DataAnnotations.Schema;

namespace DimmedAPI.Entidades
{
    public class EntryRequestFiles
    {
        public int Id { get; set; }
        public int RequestId { get; set; }
        public string FileName { get; set; }
        public string FileNameHash { get; set; }
        public string FileExt { get; set; }
        public string FileUrl { get; set; }
        [NotMapped]
        public string FileBase64 { get; set; }
        [NotMapped]
        public string Name { get; set; }
    }
}
