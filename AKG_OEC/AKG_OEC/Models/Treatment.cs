using System;
using System.Collections.Generic;

namespace AKG_OEC.Models
{
    public partial class Treatment
    {
        public Treatment()
        {
            TreatmentFertilizer = new HashSet<TreatmentFertilizer>();
        }

        public int TreatmentId { get; set; }
        public string Name { get; set; }
        public int PlotId { get; set; }
        public float? Moisture { get; set; }
        public float? Yield { get; set; }
        public float? Weight { get; set; }

        public Plot Plot { get; set; }
        public ICollection<TreatmentFertilizer> TreatmentFertilizer { get; set; }
    }
}
