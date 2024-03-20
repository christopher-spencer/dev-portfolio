using System;
using System.Collections.Generic;
//FIXME do we need a check in place so that only one image in SideProject can be set to MainImage w/ isMainImage?
namespace Capstone.Models
{
    public class Image
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public bool IsMainImage { get; set; }
    }
}