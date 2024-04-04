using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCommon
{
    public class MongoDatabaseSettings
    {
        public string ConnectionURI { get; set; }
        public string DatabaseName { get; set; }
        public bool CreateIndexes { get; set; }
        public bool InitData { get; set; }
    }
}
