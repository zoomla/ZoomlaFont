using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public abstract class Base
    {
        public object GetValue(string fieldName)
        {
            return GetType().GetProperty(fieldName).GetValue(this);
        }
    }
    public class APPConfigInfo : Base
    {
        //public string ImgSavePath { get; set; }
        //public bool Dev { get; set; }
    }
}
