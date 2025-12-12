using Es.Riam.Gnoss.Web.MVC.Models;
using System;
using System.Collections.Generic;

namespace Gnoss.Web.Open.Models.FichaRecurso
{
    public class VersionViewModel
    {
        public int Number { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string UrlPreview { get; set; }
        public string UrlLoadActionRestoreVersion { get; set; }
        public string UrlLoadActionDeleteVersion { get; set; }
        public DateTime PublishDate { get; set; }
        public string Publisher { get; set; }
        public Guid VersionId { get; set; }
        public Guid? StatusId { get; set; }
        public bool IsImprovement { get; set; }
        public EstadoVersion VersionStatus { get; set; }
        public bool IsLastVersion { get; set; }
        public bool IsSemantic {get; set; }
        public bool IsServerFile { get; set; }
        public Guid ImprovementId { get; set; }
        public List<VersionViewModel> ImprovementSubversions { get; set; }
    }
}
