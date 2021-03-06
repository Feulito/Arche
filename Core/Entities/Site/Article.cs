using IOC.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Core.Entities.Site
{
    /// <summary>
    /// Article présentant une actualité/Nouveauté
    /// </summary>
    public class Article : AbstractEntity
    {
        public string Title { get; set; }
        public string HeaderUrl { get; set; }
        public string Content { get; set; }
        public string AuteurId { get; set; }

        [JsonIgnore]
        [ForeignKey("AuteurId")]
        [ProxyResolved("AuteurId")]
        public virtual User Auteur { get; set; }
    }
}
