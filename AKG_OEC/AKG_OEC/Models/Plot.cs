using System;
using System.Collections.Generic;

namespace AKG_OEC.Models
{
    public partial class Plot
    {
        public Plot()
        {
            Treatment = new HashSet<Treatment>();
        }

        public int PlotId { get; set; }
        public int? FarmId { get; set; }
        public int? VarietyId { get; set; }
        public DateTime? DatePlanted { get; set; }
        public DateTime? DateHarvested { get; set; }
        public int? PlantingRate { get; set; }
        public bool PlantingRateByPounds { get; set; }
        public int? RowWidth { get; set; }
        public int? PatternRepeats { get; set; }
        public float? OrganicMatter { get; set; }
        public float? BicarbP { get; set; }
        public float? Potassium { get; set; }
        public float? Magnesium { get; set; }
        public float? Calcium { get; set; }
        public float? PHsoil { get; set; }
        public float? PHbuffer { get; set; }
        public float? Cec { get; set; }
        public float? PercentBaseSaturationK { get; set; }
        public float? PercentBaseSaturationMg { get; set; }
        public float? PercentBaseSaturationCa { get; set; }
        public float? PercentBaseSaturationH { get; set; }
        public string Comments { get; set; }
        
        public Farm Farm { get; set; }
        public Variety Variety { get; set; }
        public ICollection<Treatment> Treatment { get; set; }
    }
}
