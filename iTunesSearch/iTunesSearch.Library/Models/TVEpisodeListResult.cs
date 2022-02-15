using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace iTunesSearch.Library.Models
{
    [DataContract]
    public class TVEpisodeListResult
    {
        [DataMember(Name = "resultCount")]
        public int Count { get; set; }

        [DataMember(Name = "results")]
        public List<TVEpisode> Episodes { get; set; }
    }
}
