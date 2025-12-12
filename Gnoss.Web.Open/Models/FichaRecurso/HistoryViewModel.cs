using Es.Riam.Gnoss.Web.MVC.Models;
using System;
using System.Collections.Generic;

namespace Gnoss.Web.Open.Models.FichaRecurso
{
    public class HistoryViewModel
    {
        public bool IsActiveImprovement { get; set; }
        public bool HasActiveImprovement { get; set; }
        public Guid? ImprovementStatus { get; set; }
        public Guid? LastVersionStatus { get; set; }
        public bool HasWorkflow {  get; set; }
        public List<VersionViewModel> Versions { get; set; }
    }
}
