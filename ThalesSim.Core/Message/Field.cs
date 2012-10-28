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
    /// <summary>
    /// This class is used to represent an XML definition field.
    /// </summary>
    public class Field
    {
        /// <summary>
        /// Get/set the field name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Get/set the field length.
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Get/set the name of the field to lookp
        /// in order to get the length of this instance.
        /// </summary>
        public string DynamicLength { get; set; }

        /// <summary>
        /// Get/set the message value until which
        /// field parsing will continue.
        /// </summary>
        public string ParseUntil { get; set; }

        /// <summary>
        /// Get/set the field type.
        /// </summary>
        public FieldType Type { get; set; }

        /// <summary>
        /// Get/set the field upon which this field depends.
        /// </summary>
        public string DependentField { get; set; }

        /// <summary>
        /// Get/set the expected value of the dependent field.
        /// </summary>
        public List<string> DependentValues { get; set; }

        /// <summary>
        /// Get/set whether the field dependency is exclusive
        /// to this field only.
        /// </summary>
        public bool ExclusiveDependency { get; set; }

        /// <summary>
        /// Get/set a list of valid values for this field.
        /// </summary>
        public List<string> ValidValues { get; set; }

        /// <summary>
        /// Get/set a list of optional values for this field.
        /// </summary>
        public List<string> OptionValues { get; set; }

        /// <summary>
        /// Get/set the Thales response code if this
        /// field is invalid.
        /// </summary>
        public string RejectionCode { get; set; }

        /// <summary>
        /// Get/set whether to skip processing for this field.
        /// </summary>
        public bool Skip { get; set; }

        /// <summary>
        /// Get/set the number of field repetitions or the
        /// name of the field with the number of field repetitions.
        /// </summary>
        public string Repetitions { get; set; }

        /// <summary>
        /// Get/set whetherer this field demands static repetitions.
        /// </summary>
        public bool StaticRepetitions { get; set; }

        /// <summary>
        /// Get/set whether we'll continue parsing until
        /// a valid value is detected.
        /// </summary>
        public bool SkipUntil { get; set; }

        /// <summary>
        /// Get/set whether we allow a not-found condition
        /// if SkipUntil is set to True.
        /// </summary>
        public bool AllowNotFoundValid { get; set; }

        /// <summary>
        /// Default class constructor.
        /// </summary>
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

        /// <summary>
        /// Creates an instance of this class from 
        /// a data row read from XML definitions.
        /// </summary>
        /// <param name="dr">Data row with info.</param>
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

        /// <summary>
        /// Sets dependent values from a comma-separated list.
        /// </summary>
        /// <param name="text">Comma-separated list of dependent values.</param>
        public void SetDependentValues (string text)
        {
            var split = text.Split(new[] {','});
            DependentValues.Clear();
            foreach (var s in split)
            {
                DependentValues.Add(s);
            }
        }

        /// <summary>
        /// Returns a clone of this instance.
        /// </summary>
        /// <returns>Cloned copy of this instance.</returns>
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
