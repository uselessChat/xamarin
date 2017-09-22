using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Storage
{
   public interface IImages
    {
        string Location(String fileName);
        string MergeByUrl(string urlFront, string urlBack);
    }
}
