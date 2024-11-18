using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace TranVanTai.DuongTuanDuy.Models
{
    public class Mail
    {
        [DisplayName("Nguoi gui:")]
        public string From {  get; set; }

        [DisplayName("Nguoi ngan:")]
        public string To { get; set; }

        [DisplayName("Tieu de:")]
        public string Subject { get; set; }

        [DisplayName("Noi dung:")]
        public string Notes { get; set; }

        [DisplayName("File dinh kem:")]
        public string Attachment { get; set; }


    }
}