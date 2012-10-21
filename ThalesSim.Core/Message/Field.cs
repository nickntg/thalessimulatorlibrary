/*
 This program is free software; you can redistribute it and/or modify
 it under the terms of the GNU General Public License as published by
 the Free Software Foundation; either version 2 of the License, or
 (at your option) any later version.

 This program is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 GNU General Public License for more details.

 You should have received a copy of the GNU General Public License
 along with this program; if not, write to the Free Software
 Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ThalesSim.Core.Utility;

namespace ThalesSim.Core.Message
{
    public class Field
    {
        public string Name { get; set; }

        public int Length { get; set; }

        public string DynamicLength { get; set; }

        public string ParseUntil { get; set; }

        public FieldType Type { get; set; }

        public string DependentField { get; set; }

        public List<string> DependentValues { get; set; }

        public bool ExclusiveDependency { get; set; }

        public List<string> ValidValues { get; set; }

        public List<string> OptionValues { get; set; }

        public string RejectionCode { get; set; }

        public bool Skip { get; set; }

        public string Repetitions { get; set; }

        public bool StaticRepetitions { get; set; }

        public bool SkipUntil { get; set; }

        public bool AllowNotFoundValid { get; set; }

        public Field()
        {
            SkipUntil = false;
            StaticRepetitions = false;
            ExclusiveDependency = true;
            ParseUntil = string.Empty;
            DependentValues = new List<string>();
            ValidValues = new List<string>();
            OptionValues = new List<string>();
        }

        public Field (DataRow dr) : this()
        {
            Name = Convert.ToString(dr["Name"]);
            if (dr.IsNotNull("DynamicFieldLength"))
            {
                DynamicLength = Convert.ToString(dr["DynamicFieldLength"]);
            }

            if (dr.IsNotNull("ParseUntilValue"))
            {
                ParseUntil = Convert.ToString(dr["ParseUntilValue"]);
            }

            if (dr.IsNotNull("DependentField"))
            {
                DependentField = Convert.ToString(dr["DependentField"]);
            }

            if (dr.IsNotNull("DependentValue"))
            {
                SetDependentValues(Convert.ToString(dr["DependentValue"]));
            }

            if (dr.IsNotNull("ExclusiveDependency"))
            {
                ExclusiveDependency = Convert.ToBoolean(dr["ExclusiveDependency"]);
            }

            if (dr.IsNotNull("RejectionCodeIfInvalid"))
            {
                RejectionCode = Convert.ToString(dr["RejectionCodeIfInvalid"]);
            }

            if (dr.IsNotNull("Repetitions"))
            {
                Repetitions = Convert.ToString(dr["Repetitions"]);
            }

            if (dr.IsNotNull("StaticRepetitions"))
            {
                StaticRepetitions = Convert.ToBoolean(dr["StaticRepetitions"]);
            }

            if (dr.IsNotNull("SkipUntilValid"))
            {
                SkipUntil = Convert.ToBoolean(dr["SkipUntilValid"]);
            }

            if (dr.IsNotNull("AllowNotFoundValidValue"))
            {
                AllowNotFoundValid = Convert.ToBoolean(dr["AllowNotFoundValidValue"]);
            }

            if (dr.IsNotNull("OptionValue"))
            {
                OptionValues.Add(Convert.ToString(dr["OptionValue"]));
            }

            if (dr.IsNotNull("ValidValue"))
            {
                ValidValues.Add(Convert.ToString(dr["ValidValue"]));
            }
        }

        public void SetDependentValues (string text)
        {
            var split = text.Split(new[] {','});
            DependentValues.Clear();
            foreach (var s in split)
            {
                DependentValues.Add(s);
            }
        }

        public Field Clone()
        {
            return new Field
                       {
                           AllowNotFoundValid = AllowNotFoundValid,
                           DependentField = DependentField,
                           DependentValues = DependentValues.ToList(),
                           DynamicLength = DynamicLength,
                           ExclusiveDependency = ExclusiveDependency,
                           Length = Length,
                           Name = Name,
                           OptionValues = OptionValues.ToList(),
                           ParseUntil = ParseUntil,
                           RejectionCode = RejectionCode,
                           Repetitions = Repetitions,
                           Skip = Skip,
                           SkipUntil = SkipUntil,
                           StaticRepetitions = StaticRepetitions,
                           Type = Type,
                           ValidValues = ValidValues.ToList()
                       };
        }
    }
}
